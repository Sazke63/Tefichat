using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Security.RightsManagement;
using System.Threading.Tasks;
using System.Windows;
using Tefichat.Models;
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
        private Messages_Dialogs data = null;

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
            List<DialogModel> dialogs = new List<DialogModel>();
            data = await telegramClient.Messages_GetAllDialogs();

            foreach (var dialog in data.dialogs)
            {
                switch (data.UserOrChat(dialog))
                {
                    case TL.User user when user.IsActive:
                        {
                            dialogs.Add(new DialogModel((TL.Dialog)dialog)); //, new ContactUser(user, null)));
                            break;
                        }
                    case TL.Chat chat when chat.IsActive:
                        {
                            dialogs.Add(new DialogModel((TL.Dialog)dialog)); //, new ContactChat(chat)));
                            break;
                        }
                    case TL.Channel channel when channel.IsActive:
                        {
                            dialogs.Add(new DialogModel((TL.Dialog)dialog)); //, new ContactChannel(channel)));
                            break;
                        }
                }
            }

            return dialogs;
        }
    }
}
