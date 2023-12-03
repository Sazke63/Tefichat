using Stfu.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Security.RightsManagement;
using System.Threading.Tasks;
using System.Windows;
using Tefichat.Models;
using Tefichat.Views.Controls;
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

        // Оповещения
        public event EventHandler<EventArgs> Login;

        public bool HasLogin { get; set; } = false;

        public TelegramService()
        {
            phoneNumber = Properties.Settings.Default.PhoneNumber;
            telegramClient = new Client(api_id, api_hash);
        }

        public static TelegramService GetInstance() => telegramService;

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

            return data.dialogs.AsParallel().Select(d =>
            {
                switch (data.UserOrChat(d))
                {
                    case TL.User user when user.IsActive:
                        {
                            return new UserDialogModel((TL.Dialog)d, user);
                        }
                    case TL.Chat chat when chat.IsActive:
                        {
                            return new ChatDialogModel((TL.Dialog)d, chat);
                        }
                    case TL.Channel channel when channel.IsActive:
                        {
                            return new ChannelDialogModel((TL.Dialog)d, channel);
                        }
                }
                return new DialogModel(new TL.Dialog());
            }).Where(d => d.Peer != null).ToList();
        }

        // Загрузка последнего сообщения всех чатов
        public async Task<List<MessageModel>> GetLastMessages()
        {
            return data.Messages.AsParallel().Select(m =>
            {
                if (m is TL.Message msg)
                {
                    if (meAccount != null && msg.from_id != null && msg.from_id.ID == meAccount.ID)
                        return new MessageModel(msg, isOriginNative: true);
                    else
                        return new MessageModel(msg);
                }
                else if (m is TL.MessageService ms)
                {
                    return new MessageModel(ms);
                }
                return new MessageModel(new Message { id = 666 });
            }).Where(m => m.ID != 666).ToList();
        }

        // Чтение сообщений
        public async Task<bool> ReadMessage(DialogModel dialog, int max_id)
        {
            return await telegramClient.ReadHistory(GetInputPeer(dialog), max_id);
        }

        // Отправка сообщения
        public async Task<bool> SendMessage(DialogModel dialog, string text)
        {
            InputPeer inputPeer = GetInputPeer(dialog);
            var answer = await telegramClient.SendMessageAsync(inputPeer, text);
            return answer.ID != 0;
        }

        // Загрузка истории чата
        public async Task<List<MessageModel>> GetMessagesHistoryDialog(DialogModel dialog, int offset_id = 0, int add_offset = 0, int count = 30, bool mode = false)
        {
            if (dialog == null) return null;

            List<MessageModel> messages = new List<MessageModel>();
            TL.InputPeer inputPeer = GetInputPeer(dialog);
            var limit = count > 100 ? 100 : count;
            MessageModel message = null;
            bool NoFoundGroup = false;
            bool AddPlus = false;

            for (; ; ) //int offset_id = 0
            {
                var mes = await telegramClient.Messages_GetHistory(inputPeer, offset_id, add_offset: add_offset, limit: limit);

                if (mes.Messages.Length == 0) break;

                var i = 0;
                foreach (var msgBase in mes.Messages)
                {                   
                    if (msgBase is Message msg)
                    {
                        if (msg.grouped_id != 0)
                        {
                            var groupMes = messages.SingleOrDefault(m => m.Grouped_id == msg.grouped_id);
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
                            message = new MessageModel(msg);
                            //if (msg.media != null)
                            //    message.Photos.Add(new PictureModel(msg.ID, msg.grouped_id, null, msg.media));
                            if (meAccount != null && msg.from_id != null && msg.from_id.ID == meAccount.ID)
                                message.IsOriginNative = true;
                            AddMessage();
                                
                            NoFoundGroup = false;
                            message = null;
                        }
                    }
                    else if (msgBase is TL.MessageService ms)
                    {
                        message = new MessageModel(ms);
                        AddMessage();
                        message = null;
                    }

                    void AddMessage()
                    {
                        if (mode || !AddPlus)
                            messages.Add(message);
                        else
                        {
                            messages.Insert(i, message);
                            i++;
                        }
                    }
                }
                if (messages.Count() >= count) break;
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
        private InputPeer GetInputPeer(DialogModel dialog)
        {
            switch (dialog)
            {
                case UserDialogModel user: return new InputPeerUser(dialog.Peer.ID, user.AccessHash);
                case ChatDialogModel chat: return new InputPeerChat(dialog.Peer.ID);
                case ChannelDialogModel channel: return new InputPeerChannel(dialog.Peer.ID, channel.AccessHash);
            }
            return null;
        }
    }
}
