using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Tefichat.Base;
using Tefichat.Models;
using Tefichat.Services;
using TL;

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

        public ICollectionView SearchCollectionViewSource { get; set; }

        private string searchText;
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                SearchCollectionViewSource.Filter = DoesCollectionContainName;
            }
        }

        private string message;
        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        public ICommand GetMessagesCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }

        public MainVM()
        {
            _telegramService = TelegramService.GetInstance();
            Dialogs = new ObservableCollection<DialogModel>();
            SearchCollectionViewSource = new CollectionViewSource { Source = Dialogs }.View;
            //SearchCollectionViewSource.SortDescriptions.Add(new SortDescription("LastMessage.Date", ListSortDirection.Ascending));
            //GetMessagesCommand = new RelayCommand(async(o) => await GetMessages(o));
            SendMessageCommand = new RelayCommand(async (o) => await SendMessage(o));
        }

        public async Task DownloadData()
        {
            var dlgs = await _telegramService.GetAllDialogs();
            if (dlgs != null)
                dlgs.ForEach(c => Dialogs.Add(c));
            await GetLastMessages();
        }

        private async Task GetLastMessages()
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

        private async Task SendMessage(object o)
        {
            if (message is null) return;

            var result = await _telegramService.SendMessage(selectedDialog, message);
            if (result)
                Message = "";
        }

        private bool DoesCollectionContainName(object dialogName)
        {
            DialogModel dlgName = dialogName as DialogModel;
            switch (dlgName)
            {
                case UserDialogModel user: return user.Name.ToLower().Contains(SearchText.ToLower());
                case ChatDialogModel chat: return chat.Name.ToLower().Contains(SearchText.ToLower());
                case ChannelDialogModel channel: return channel.Name.ToLower().Contains(SearchText.ToLower());
            }

            return false;
        }
    }
}
