using Tefichat.Base;
using Tefichat.Services;

namespace Tefichat.ViewModels
{
    public class RootVM : ObservableObject
    {
        private ITelegramService telegramService;

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
            telegramService = new TelegramService();
            if (!telegramService.HasLogin)
            {
                CurrentVM = new LoginPhoneVM(telegramService);
            }
            else
                CurrentVM = new MainVM();
        }
    }
}
