using System.Threading.Tasks;
using Tefichat.Base;
using Tefichat.Services;

namespace Tefichat.ViewModels
{
    public class RootVM : ObservableObject
    {
        private ITelegramService _telegramService;

        private object currentVM;
        public object CurrentVM 
        { 
            get => currentVM;
            set 
            { 
                currentVM = value; 
                OnPropertyChanged(nameof(CurrentVM));
            } 
        }

        public RootVM()
        {
            _telegramService = TelegramService.GetInstance();
            _telegramService.Login += _telegramService_Login;
        }

        public async Task Start()
        {
            await _telegramService.CheckLogin();

            if (!_telegramService.HasLogin)
            {
                CurrentVM = new LoginPhoneVM();
            }
            else
            {
                MainVM mainVM = new MainVM();
                await mainVM.DownloadData();
                CurrentVM = mainVM;
            } 
        }

        private void _telegramService_Login(object? sender, System.EventArgs e)
        {
            if (_telegramService.HasLogin)
                CurrentVM = new MainVM();
            else
                CurrentVM = new SendCodeVM();
        }
    }
}
