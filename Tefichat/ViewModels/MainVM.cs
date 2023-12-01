using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tefichat.Base;
using Tefichat.Models;
using Tefichat.Services;

namespace Tefichat.ViewModels
{
    public class MainVM : ObservableObject
    {
        private ITelegramService _telegramService;

        // Видимость окна чата
        private bool isChatVisible = false;
        public bool IsChatVisible
        {
            get => isChatVisible;
            set
            {
                isChatVisible = value;
                OnPropertyChanged(nameof(IsChatVisible));
            }
        }

        public ObservableCollection<DialogModel> Dialogs { get; set; }

        private DialogModel selectedDialog;
        public DialogModel SelectedDialog
        {
            get => selectedDialog;
            set
            {
                selectedDialog = value;
                OnPropertyChanged("SelectedDialog");
            }
        }

        public ICommand GetMessagesCommand { get; set; }

        public MainVM()
        {
            _telegramService = TelegramService.GetInstance();
            Dialogs = new ObservableCollection<DialogModel>();
            GetMessagesCommand = new RelayCommand(async(o) => await GetLastMessages(o));
        }

        public async Task DownloadData()
        {
            var dlgs = await _telegramService.GetAllDialogs();
            if (dlgs != null)
                dlgs.ForEach(c => Dialogs.Add(c));
            await GetLastMessages(this);
        }

        private async Task GetLastMessages(object o)
        {
            if (!IsChatVisible) IsChatVisible = true;

            var messages = await _telegramService.GetLastMessages();

            if (messages != null)
            {
                Dialogs.AsParallel().ForAll(d =>
                {
                    var mes = messages.SingleOrDefault(m => m.Peer.ID == d.Peer.ID);
                    if (mes != null)
                    {
                        d.Messages.Add(mes);
                        d.LastMessage = mes;
                    }
                });
            }

            //Dialogs.AsParallel().ForAll(d =>
            //{
            //    d.Contact.Messages.AsParallel().ForAll(async m =>
            //    {
            //        if (m.media != null)
            //        {
            //            if (m.media is TL.MessageMediaPhoto)
            //                m.Photos.Add(new PictureModel(m.ID, 0, await telegramService.DownloadPhoto(m.media)));
            //        }
            //    });
            //});
        }
    }
}
