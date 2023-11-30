using System;
using System.Threading.Tasks;

namespace Tefichat.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly string api_hash = "664bff894d30f72a263cd927928c4bfb";
        private readonly int api_id = 29217125;

        private string phoneNumber;

        //
        public event EventHandler<EventArgs> Login;

        public bool HasLogin { get; set; } = false;

        public TelegramService()
        {
            phoneNumber = Properties.Settings.Default.PhoneNumber;
        }

        public async Task CheckLogin()
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                await Authorization("7" + phoneNumber);
            }
        }

        public async Task Authorization(string loginInfo)
        {
            //HasLogin = false;
            if (loginInfo.Length == 11 && phoneNumber == "")
            {
                Properties.Settings.Default.PhoneNumber = loginInfo;
                Login.Invoke(this, new EventArgs());
            }
                
            if (loginInfo.Length == 5)
            {
                HasLogin = true;               
                Login.Invoke(this, new EventArgs());
            }               
        }
    }
}
