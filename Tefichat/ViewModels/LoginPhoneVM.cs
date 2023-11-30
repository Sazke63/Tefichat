using Tefichat.Base;
using Tefichat.Services;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Tefichat.ViewModels
{
    public class LoginPhoneVM : ObservableObject
    {
        private ITelegramService _telegramService;

        private string? phoneNumber;
        public string? PhoneNumber
        {
            get => phoneNumber;
            set
            {
                if (value?.Length == 10)
                {
                    phoneNumber = value;
                    OnPropertyChanged("PhoneNumber");
                }
            }
        }

        public ICommand LoginCommand { get; set; }

        public LoginPhoneVM(ITelegramService telegramService)
        {
            _telegramService = telegramService;
            LoginCommand = new RelayCommand(async (o) => await Login(o));
        }

        private async Task Login(object o)
        {
            if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length <= 10)
            {
                await _telegramService.Authorization("7" + phoneNumber);

                if (_telegramService.HasLogin)
                {

                }
                //Properties.Settings.Default.PhoneNumber = phoneNumber;
            }
        }
    }
}
