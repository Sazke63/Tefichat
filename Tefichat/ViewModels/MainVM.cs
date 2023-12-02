using Stfu.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Tefichat.Base;
using Tefichat.Models;
using Tefichat.Services;
using Tefichat.Views.Controls;
using TL;

namespace Tefichat.ViewModels
{
    public class MainVM : ObservableObject
    {
        private ITelegramService _telegramService;
        private int MaxID;

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

        // Видимость окна меню
        private bool isMenuVisible = false;
        public bool IsMenuVisible
        {
            get => isChatVisible;
            set
            {
                isChatVisible = value;
                OnPropertyChanged(nameof(IsMenuVisible));
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

        private MessageModel selectMessage;
        public MessageModel SelectMessage
        {
            get => selectMessage;
            set
            {
                if (value != null)
                {
                    selectMessage = value;
                    if (!value.IsOriginNative)
                    {
                        switch (selectedDialog)
                        {
                            case ChannelDialogModel channel when channel.IsChannel: MaxID = SelectedDialog.Read_inbox_max_id; break;
                            default: MaxID = SelectedDialog.Read_outbox_max_id; break;
                        }

                        if (value.ID > MaxID)
                        {
                            //ReadMessageCommand.CanExecute(null);
                            //ReadMessageCommand.Execute(null);
                        }
                        OnPropertyChanged(nameof(SelectMessage));
                    }

                    if (SelectMessage.ID == selectedDialog.Messages.Last().ID && MaxID < selectedDialog.TopMessage - 1)
                    {
                        GetNextMessagesCommand.CanExecute(null);
                        GetNextMessagesCommand.Execute(null);
                    }
                }
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
        public ICommand GetNextMessagesCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand ShowMenuCommand { get; set; }

        public MainVM()
        {
            _telegramService = TelegramService.GetInstance();
            Dialogs = new ObservableCollection<DialogModel>();
            SearchCollectionViewSource = new CollectionViewSource { Source = Dialogs }.View;
            //SearchCollectionViewSource.SortDescriptions.Add(new SortDescription("LastMessage.Date", ListSortDirection.Ascending));
            //GetMessagesCommand = new RelayCommand(async(o) => await GetMessages(o));
            SendMessageCommand = new RelayCommand(async (o) => await SendMessage(o));
            GetMessagesCommand = new RelayCommand(async (o) => await GetMessages(o));
            GetNextMessagesCommand = new RelayCommand(async (o) => await GetNextMessages(o));
            ShowMenuCommand = new RelayCommand((o) => ShowMenu());
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
            var messages = await _telegramService.GetLastMessages();

            if (messages != null)
            {
                Dialogs.AsParallel().ForAll(d =>
                {
                    var mes = messages.SingleOrDefault(m => m.Peer.ID == d.Peer.ID);
                    if (mes != null)
                    {
                        //d.Messages.Add(mes);
                        d.LastMessage = mes;
                    }
                });
            }
        }

        private async Task GetPrevMessages(object o)
        {
            var fstMsgID = selectedDialog.Messages.First().ID;
            var messages = await _telegramService.GetMessagesHistoryDialog(selectedDialog, fstMsgID, 10);
            messages.AsParallel().ForAll(m => SelectedDialog.Messages.Insert(0, m));
        }

        private async Task GetMessages(object o)
        {
            if (!IsChatVisible) IsChatVisible = true;

            if (SelectedDialog.Messages.Count() >= 20) return;

            var count = selectedDialog.Messages.Count();
            List<MessageModel> messages = new List<MessageModel>();
            int lastMsgID = selectedDialog is ChannelDialogModel ? SelectedDialog.Read_inbox_max_id + 1 : SelectedDialog.Read_outbox_max_id + 1;

            if (selectedDialog != null)
            {
                //var unread = selectedDialog.Unread_count;
                messages = await _telegramService.GetMessagesHistoryDialog(selectedDialog, lastMsgID, count: Math.Abs(20 - count), mode: true);
            }

            messages.ForEach(m => SelectedDialog.Messages.Insert(0, m));

            //SelectedDialog.Messages.ForAll(m =>
            //{
            //    string path = @"C:\Users\Sazke\Documents\meshistiryafter.txt";
            //    File.AppendAllTextAsync(path, m.ID.ToString() + "\n");
            //});
        }

        private async Task GetNextMessages(object o)
        {
            var lastMsgID = selectedDialog.Messages.Last().ID + 1;

            if (selectedDialog.Messages.Last().ID == selectedDialog.TopMessage) return;

            var messages = await _telegramService.GetMessagesHistoryDialog(selectedDialog, lastMsgID, -10, 10);
            var index = SelectedDialog.Messages.Count();
            messages.ForEach(m =>
            {
                var find = SelectedDialog.Messages.AsParallel().SingleOrDefault(mes => mes.ID == m.ID);
                if (find == null)
                {
                    SelectedDialog.Messages.Insert(index, m);
                }
                else if (m.Message == "" && m.media != null)
                {
                    // группировка
                }
            });

            SelectedDialog.Messages.ForAll(m =>
            {
                string path = @"C:\Users\Sazke\Documents\meshistiryafter.txt";
                File.AppendAllTextAsync(path, m.ID.ToString() + "\n");
            });
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

        private void ShowMenu()
        {
            IsMenuVisible = true;
        }
    }
}
