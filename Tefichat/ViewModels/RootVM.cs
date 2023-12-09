using System.Threading.Tasks;
using System.Windows.Input;
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

        // Видимость окна меню
        private bool isMenuVisible = false;
        public bool IsMenuVisible
        {
            get => isMenuVisible;
            set
            {
                isMenuVisible = value;
                OnPropertyChanged(nameof(IsMenuVisible));
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

        public void Close()
        {
            _telegramService.Dispose();
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
