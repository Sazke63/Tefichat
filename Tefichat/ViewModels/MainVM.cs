using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tefichat.Base;
using Tefichat.Models;
using Tefichat.Services;

namespace Tefichat.ViewModels
{
    public class MainVM : ObservableObject
    {
        private ITelegramService _telegramService;
        public ObservableCollection<DialogModel> Dialogs { get; set; }

        public MainVM()
        {
            _telegramService = TelegramService.GetInstance();
            Dialogs = new ObservableCollection<DialogModel>();
        }

        public async Task DownloadData()
        {
            List<DialogModel> dlgs = new List<DialogModel>();
            dlgs = await _telegramService.GetAllDialogs();
            if (dlgs != null)
                dlgs.ForEach(c => Dialogs.Add(c));
        }
    }
}
