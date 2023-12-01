using Stfu.Linq;
using System;
using System.Collections.Generic;
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

        // Data
        private User meAccount;
        private Messages_Dialogs data;

        //
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

        // Получение списка Диалогов
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

        // Получение последнего сообщения всех чатов
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
    }
}
