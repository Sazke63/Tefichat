using Stfu.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Security.Principal;
using System.Security.RightsManagement;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using Tefichat.Models;
using Tefichat.Services.EventAgs;
using TL;
using WTelegram;

namespace Tefichat.Services
{
    public class TelegramService : ITelegramService
    {
        private static readonly TelegramService telegramService = new TelegramService();
        private readonly string api_hash = "664bff894d30f72a263cd927928c4bfb";
        private readonly int api_id = 29217125;
        private readonly Client telegramClient;
        private string phoneNumber;

        // Данные
        private User meAccount;
        private Messages_Dialogs data;
        private readonly Dictionary<long, User> Users = new Dictionary<long, User>();
        private readonly Dictionary<long, ChatBase> Chats = new Dictionary<long, ChatBase>();

        // Оповещения
        public event EventHandler<EventArgs> Login;
        public event EventHandler<NewMessageEventArgs> NewMessage;
        public event EventHandler<ReadChannelInboxEventArgs> ReadChannelInbox;
        public event EventHandler<ReadChannelOutboxEventArgs> ReadChannelOutbox;
        public event EventHandler<ReadHistoryInboxEventArgs> ReadHistoryInbox;
        public event EventHandler<ReadHistoryOutboxEventArgs> ReadHistoryOutbox;
        public event EventHandler<ChannelEventArgs> UpdChannel;
        public event EventHandler<ChatEventArgs> UpdChat;
        public event EventHandler<UserStatusEventArgs> UpdUserStatus;
        public event EventHandler<UserEventArgs> UpdUser;

        public bool HasLogin { get; set; } = false;

        public TelegramService()
        {
            phoneNumber = Properties.Settings.Default.PhoneNumber;
            telegramClient = new Client(api_id, api_hash);
            telegramClient.OnUpdate += TelegramClient_OnUpdate;
        }

        public static TelegramService GetInstance() => telegramService;

        public void Dispose() => telegramClient.Dispose();

        public async Task CheckLogin()
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                await Authorization(phoneNumber);
            }
        }

        // Авторизацию по номеру и коду
        public async Task Authorization(string loginInfo)
        {
            try
            {
                string what = await telegramClient.Login(loginInfo);

                if (what == null) 
                {
                    HasLogin = true;
                    meAccount = telegramClient.User;
                }

                if (loginInfo.Length == 11 && phoneNumber == "")
                {
                    Properties.Settings.Default.PhoneNumber = loginInfo;
                    Properties.Settings.Default.Save();
                }

                Login.Invoke(this, new EventArgs());

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }              
        }

        // Загрузка списка Диалогов
        public async Task<List<DialogModel>> GetAllDialogs()
        {
            data = await telegramClient.Messages_GetAllDialogs();
            data.CollectUsersChats(Users, Chats);
            return data.dialogs.AsParallel().Select(GetDialog).Where(d => d.Entity.IsActive).ToList();
        }

        // Загрузка последнего сообщения всех чатов
        public async Task<List<MessageBaseModel>> GetLastMessages()
        {
            return data.Messages.AsParallel().Select(m =>
            {
                MessageBaseModel messageBaseModel = null;
                if (m is Message msg)
                {
                    if (meAccount != null && msg.from_id != null && msg.from_id.ID == meAccount.ID)
                        return messageBaseModel = new MessageModel(msg, isOriginNative: true);
                    else
                        return new MessageModel(msg);
                }
                else if (m is MessageService ms)
                {
                    return messageBaseModel = new MessageServiceModel(ms);
                }
                return new MessageModel(new Message { id = 666 });
            }).Where(m => m.ID != 666).ToList();
        }

        // Чтение сообщений
        public async Task<bool> ReadMessage(DialogModel dialog, int max_id)
        {
            return await telegramClient.ReadHistory(GetInputPeer(dialog.Entity), max_id);
        }

        // Отправка сообщения
        public async Task<Message> SendMessage(DialogModel dialog, string text)
        {
            InputPeer inputPeer = GetInputPeer(dialog.Entity);
            var answer = await telegramClient.SendMessageAsync(inputPeer, text);
            return answer;
        }

        // Отправка медиа сообщения
        public async Task<Message> SendMediaMessage(DialogModel dialog, string caption, string path)
        {
            InputPeer inputPeer = GetInputPeer(dialog.Entity);
            var inputFile = await telegramClient.UploadFileAsync(path);
            var answer = await telegramClient.SendMediaAsync(inputPeer, caption, inputFile);
            return answer;
        }

        // Загрузка истории чата
        public async Task<List<MessageBaseModel>> GetMessagesHistoryDialog(DialogModel dialog, int offset_id = 0, int add_offset = 0, int count = 30, bool mode = false)
        {
            if (dialog == null) return null;

            List<MessageBaseModel> messages = new List<MessageBaseModel>();
            InputPeer inputPeer = GetInputPeer(dialog.Entity);
            var limit = count > 100 ? 100 : count;
            MessageModel message = null;
            MessageServiceModel messageService = null;
            bool NoFoundGroup = false;
            bool AddPlus = false;

            for (; ; ) //int offset_id = 0
            {
                var mes = await telegramClient.Messages_GetHistory(inputPeer, offset_id, add_offset: add_offset, limit: limit);
                mes.CollectUsersChats(Users, Chats);
                if (mes.Messages.Length == 0) break;

                var i = 0;
                foreach (var msgBase in mes.Messages)
                {                   
                    if (msgBase is Message msg)
                    {
                        if (msg.grouped_id != 0)
                        {
                            var groupMes = (MessageModel)messages.SingleOrDefault(m => m.GroupedId == msg.grouped_id);
                            if (groupMes != null)
                            {
                                //groupMes.Photos.Insert(0, new PictureModel(msg.ID, msg.grouped_id, null, msg.media));
                                if (msg.message != "")
                                    groupMes.Message = msg.message;
                            }
                            else
                            {
                                NoFoundGroup = true;
                            }
                        }

                        if (msg.grouped_id == 0 || NoFoundGroup)
                        {
                            message = new MessageModel(msg, GetPeerInfo(msg.From ?? msg.Peer));
                            //if (msg.media != null)
                            //    message.Photos.Add(new PictureModel(msg.ID, msg.grouped_id, null, msg.media));
                            if (meAccount != null && msg.from_id != null && msg.from_id.ID == meAccount.ID)
                                message.IsOriginNative = true;
                            if (msg.fwd_from != null)
                                message.FwdFrom = new ForwardHeaderModel(GetPeerInfo(msg.fwd_from.from_id), msg.fwd_from.channel_post);
                                
                            AddMessage(message);
                                
                            NoFoundGroup = false;
                            message = null;
                        }
                    }
                    else if (msgBase is MessageService ms)
                    {
                        messageService = new MessageServiceModel(ms);
                        AddMessage(messageService);
                        messageService = null;
                    }

                    void AddMessage(MessageBaseModel mesAdd)
                    {
                        //if (dialog.Entity is ChannelModel || dialog.Entity is ChatModel)
                        //    mesAdd.From = dialog.Entity;

                        if (mode || !AddPlus)
                            messages.Add(mesAdd);
                        else
                        {
                            messages.Insert(i, mesAdd);
                            i++;
                        }
                    }
                }
                if (messages.Count() >= count || mes.Count > count) break;
                else AddPlus = true;
                offset_id = (mode) ? offset_id - mes.Messages.Length : offset_id + mes.Messages.Length;                
            }

            //messages.ForAll(m =>
            //{
            //    string path = @"C:\Users\Sazke\Documents\meshistirybefore.txt";
            //    File.AppendAllTextAsync(path, m.ID.ToString() + "\n");
            //});

            return messages;
        }

        // Получение InputPeer
        private InputPeer GetInputPeer(IPeerInfoModel entity)
        {
            switch (entity)
            {
                case UserModel user: return new InputPeerUser(user.ID, user.AccessHash);
                case ChatModel chat: return new InputPeerChat(chat.ID);
                case ChannelModel channel: return new InputPeerChannel(channel.ID, channel.AccessHash);
            }
            return null;
        }

        // Получение диалога
        private DialogModel GetDialog(DialogBase dialog)
        {
            switch (data.UserOrChat(dialog))
            {
                case User user when user.IsActive:
                    {
                        return new DialogModel((Dialog)dialog, new UserModel(user));
                    }
                case Chat chat when chat.IsActive:
                    {
                        return new DialogModel((Dialog)dialog, new ChatModel(chat));
                    }
                case Channel channel when channel.IsActive:
                    {
                        return new DialogModel((Dialog)dialog, new ChannelModel(channel));
                    }
            }
            return new DialogModel(new Dialog(), new ChannelModel(new Channel()));
        }

        private IPeerInfoModel GetPeerInfo(Peer peer)
        {
            //InputPeer inputPeer = null;
            if (peer == null) return null;

            switch(peer)
            {
                case PeerUser puser:
                    {
                        var user = Users.SingleOrDefault(u => u.Value.ID == puser.ID).Value;
                        return new UserModel(user);
                    }
                case PeerChat pchat:
                    {
                        var chat = (Chat)Chats.SingleOrDefault(ch => ch.Value.ID == pchat.ID).Value;
                        return new ChatModel(chat);
                    }
                case PeerChannel pchannel:
                    {
                        var channel = (Channel)Chats.SingleOrDefault(ch => ch.Value.ID == pchannel.ID).Value;
                        return new ChannelModel(channel);
                    }
                default:
                    {
                        MessageBox.Show("Not found");
                        return null;
                    }
            }

            //var peerDialogs = await telegramClient.Messages_GetPeerDialogs(inputPeer);

            //if (peerDialogs.chats.Count > 0)
            //    return peerDialogs.chats.Select(ch => new (ch));
            //var res = peerDialogs.dialogs.Select(GetDialog).Where(d => d.Peer != null).ToList();

            //return res.Count > 0 ? res[0] : null;
        }

        // Проверка обновлений
        private async Task TelegramClient_OnUpdate(IObject arg)
        {
            if (!(arg is UpdatesBase updates) || telegramClient.User == null) return;
            updates.CollectUsersChats(Users, Chats);

            foreach (var update in updates.UpdateList)
            {
                switch (update)
                {
                    case UpdateNewMessage unm:
                        {
                            if (unm.message is Message msg)
                            {
                                var telmes = new MessageModel(msg);

                                //if (msg.media != null)
                                //{
                                //    switch (msg.media)
                                //    {
                                //        case TL.MessageMediaPhoto mmp:
                                //            {
                                //                var photo = await DownloadPhoto(msg.media);
                                //                telmes.Photos.Add(new PictureModel(msg.ID, msg.grouped_id, photo, msg.media));
                                //                break;
                                //            }
                                //        default: break;
                                //    }
                                //}
                                if (meAccount != null && msg.from_id != null && msg.from_id.ID == meAccount.ID)
                                    telmes.IsOriginNative = true;

                                NewMessage?.Invoke(this, new NewMessageEventArgs(telmes));
                            }
                            break;
                        }
                    case UpdateReadChannelInbox urci: ReadChannelInbox(this, new ReadChannelInboxEventArgs(urci)); break;
                    case UpdateReadChannelOutbox urco: ReadChannelOutbox(this, new ReadChannelOutboxEventArgs(urco)); break;
                    case UpdateReadHistoryInbox urhi: ReadHistoryInbox(this, new ReadHistoryInboxEventArgs(urhi)); break;
                    case UpdateReadHistoryOutbox urho: ReadHistoryOutbox(this, new ReadHistoryOutboxEventArgs(urho)); break;
                    case UpdateChannelMessageViews ucmv: break;
                    case UpdateChannel upch: UpdChannel(this, new ChannelEventArgs(upch)); break;
                    case UpdateChat upct: UpdChat(this, new ChatEventArgs(upct)); break;
                    case UpdateUserStatus uus: break;
                    case UpdateUserName uun: break;
                    case UpdateUser upus: UpdUser(this, new UserEventArgs(upus)); break;
                    default: break;
                }
            }
        }

    }
}
