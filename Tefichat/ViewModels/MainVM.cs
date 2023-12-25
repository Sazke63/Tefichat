using Microsoft.Win32;
using Stfu.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Tefichat.Base;
using Tefichat.Models;
using Tefichat.Models.Media;
using Tefichat.Services;
using Tefichat.Services.EventAgs;
using Tefichat.Views.Controls;
using TL;

namespace Tefichat.ViewModels
{
    public class MainVM : ObservableObject
    {
        private ITelegramService _telegramService;
        private long GroupID;
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
            get => isMenuVisible;
            set
            {
                isMenuVisible = value;
                OnPropertyChanged(nameof(IsMenuVisible));
            }
        }

        // Видимость окна отправки медиа
        private bool isSendMediaVisible = false;
        public bool IsSendMediaVisible
        {
            get => isSendMediaVisible;
            set
            {
                isSendMediaVisible = value;
                OnPropertyChanged(nameof(IsSendMediaVisible));
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
                        switch (selectedDialog.Entity)
                        {
                            case ChannelModel channel: MaxID = SelectedDialog.Read_inbox_max_id; break;
                            default: MaxID = SelectedDialog.Read_outbox_max_id; break;
                        }

                        if (value.ID > MaxID)
                        {
                            ReadMessageCommand.CanExecute(null);
                            ReadMessageCommand.Execute(null);
                        }
                    }

                    if (SelectMessage.ID == selectedDialog.Messages.Last().ID && MaxID < selectedDialog.TopMessage - 1)
                    {
                        GetNextMessagesCommand.CanExecute(null);
                        GetNextMessagesCommand.Execute(null);
                    }
                    if (SelectMessage.ID == selectedDialog.Messages[2].ID)
                    {
                        GetPrevMessagesCommand.CanExecute(null);
                        //GetPrevMessagesCommand.Execute(null);
                        //Task.Delay(1);
                    }

                    OnPropertyChanged(nameof(SelectMessage));
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
                //SearchCollectionViewSource.Filter = DoesCollectionContainName;
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

        private string path;
        public string Path
        {
            get => path;
            set
            {
                path = value;
                OnPropertyChanged(nameof(Path));
            }
        }

        private MessageModel replyTo;
        public MessageModel ReplyTo
        {
            get => replyTo;
            set
            {
                replyTo = value;
                OnPropertyChanged(nameof(ReplyTo));
            }
        }

        // Команды
        public ICommand GetMessagesCommand { get; set; }
        public ICommand GetPrevMessagesCommand { get; set; }
        public ICommand GetNextMessagesCommand { get; set; }
        public ICommand SendMediaMessageCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand ReadMessageCommand { get; set; }
        public ICommand ReplyToCommand { get; set; }
        public ICommand CancelReplyToCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public ICommand CancelSelectFileCommand { get; set; }
        public ICommand ShowMenuCommand { get; set; }
        public ICommand HideMenuCommand { get; set; }

        public MainVM()
        {
            _telegramService = TelegramService.GetInstance();
            Dialogs = new ObservableCollection<DialogModel>();
            SearchCollectionViewSource = new CollectionViewSource { Source = Dialogs }.View;
            //SearchCollectionViewSource.SortDescriptions.Add(new SortDescription("LastMessage.Date", ListSortDirection.Ascending));
            //GetMessagesCommand = new RelayCommand(async(o) => await GetMessages(o));
            SendMediaMessageCommand = new RelayCommand(async (o) => await SendMediaMessage(o));
            SendMessageCommand = new RelayCommand(async (o) => await SendMessage(o));
            ReadMessageCommand = new RelayCommand(async (o) => await ReadMessage(o));
            GetMessagesCommand = new RelayCommand(async (o) => await GetMessages(o));
            GetPrevMessagesCommand = new RelayCommand(async (o) => await GetPrevMessages(o));
            GetNextMessagesCommand = new RelayCommand(async (o) => await GetNextMessages(o));
            SelectFileCommand = new RelayCommand((o) => SelectFile());
            CancelSelectFileCommand = new RelayCommand((o) => CancelSelectFile());
            ReplyToCommand = new RelayCommand((o) => ReplyToSet());
            CancelReplyToCommand = new RelayCommand((o) => CancelReplyToSet());
            ShowMenuCommand = new RelayCommand((o) => ShowMenu());
            HideMenuCommand = new RelayCommand((o) => HideMenu());

            // Подписка на обновления
            _telegramService.NewMessage += NewMessage;
            _telegramService.ReadChannelInbox += ReadChannelInbox;
            _telegramService.ReadChannelOutbox += ReadChannelOutbox;
            _telegramService.ReadHistoryInbox += ReadHistoryInbox;
            _telegramService.ReadHistoryOutbox += ReadHistoryOutbox;
        }

        public async Task DownloadData()
        {
            var path = Directory.GetCurrentDirectory();
            if (!Directory.Exists($"{path}\\Data"))
            {
                Directory.CreateDirectory($"{path}\\Data");
                Directory.CreateDirectory($"{path}\\Data\\UserData");
                Directory.CreateDirectory($"{path}\\Data\\UserData\\Media");
            }
            else if(!Directory.Exists($"{path}\\Data\\UserData"))
            {
                Directory.CreateDirectory($"{path}\\Data\\UserData");
                Directory.CreateDirectory($"{path}\\Data\\UserData\\Media");
            }
            else if (!Directory.Exists($"{path}\\Data\\UserData\\Media"))
            {
                Directory.CreateDirectory($"{path}\\Data\\UserData\\Media");
            }

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
                    var mes = messages.FirstOrDefault(m => m.Peer.ID == d.Entity.ID);
                    if (mes != null)
                    {
                        //d.Messages.Add(mes);
                        d.LastMessage = mes;
                    }
                });
            }
        }

        private async Task GetMessages(object o)
        {
            if (!IsChatVisible) IsChatVisible = true;

            if (SelectedDialog.Messages.Count() >= 20) return;

            //var count = selectedDialog.Messages.Count();
            List<MessageBaseModel> messages = new List<MessageBaseModel>();
            int lastMsgID = selectedDialog.Entity is ChannelModel ? SelectedDialog.Read_inbox_max_id + 1 : SelectedDialog.Read_outbox_max_id + 1;

            if (selectedDialog != null && lastMsgID != 0)
            {
                //var unread = selectedDialog.Unread_count;
                messages = await _telegramService.GetMessagesHistoryDialog(selectedDialog, lastMsgID, count: 30, mode: true);
            }

            messages.ForEach(m => SelectedDialog.Messages.Insert(0, m));

            SelectedDialog.Messages.AsParallel().Where(m => m is MessageModel).ForAll(async m =>
            {
                if (m is MessageModel mes)
                    if (mes.Data.reply_to is MessageReplyHeader mrh)
                    {
                        if (mrh != null)
                        {
                            var mesReply = SelectedDialog.Messages.SingleOrDefault(ms => ms.ID == mrh.reply_to_msg_id);
                            if (mesReply != null)
                            {
                                mes.ReplyTo = mesReply;
                            }
                            else
                            {
                                var mesGet = await _telegramService.GetMessagesHistoryDialog(selectedDialog, mrh.reply_to_msg_id, 1);
                                if (mesGet != null)
                                    mesReply = mesGet[0];
                            }
                        }
                    }
            });

            // Получение фото к сообщениям
            SelectedDialog.Messages.AsParallel().ForAll(async m =>
            {
                if (m is MessageModel mm)
                {
                    if (mm.Data.media != null)
                    {
                        foreach(var media in mm.Media)
                        {
                            if (media is PhotoModel photo)
                            {
                                photo.Picture = await _telegramService.DownloadPhoto(photo.MediaData);
                            }
                            if (media is DocumentModel doc)
                            {
                                var document = await _telegramService.DownloadPhoto(doc.MediaData);
                                var path = Directory.GetCurrentDirectory();
                                path += $"\\Data\\UserData\\Media\\{doc.Data.ID}.mp4";
                                File.WriteAllBytes($"{path}", document);
                                doc.Picture = path;
                            }
                        }

                        mm.GetGrid();

                        //if (mm.Media is MediaPhotoModel mpm)
                        //{
                            //mpm.GetGrid();
                            //mpm.Photos.ForAll(async (p) =>
                            //{
                            //    p.Picture = await _telegramService.DownloadPhoto(p.Data);
                            //    //p.Picture2 = await _telegramService.DownloadMedia(p.Data);
                            //    string Path = @"C:\Users\$dmin\Pictures\Test2\";
                            //    long ID = 0;
                            //    if (p.Data is MessageMediaPhoto mmp)
                            //    {
                            //        ID = mmp.photo.ID;
                            //        File.WriteAllBytes(Path + ID + ".jpg", p.Picture);
                            //    }
                            //    if (p.Data is MessageMediaDocument mmd)
                            //    {
                            //        ID = mmd.document.ID;
                            //        File.WriteAllBytes(Path + ID + ".mp4", p.Picture);
                            //    }                               
                            //});

                            //foreach (var photo in mpm.Photos)
                            //{
                            //    photo = await _telegramService.DownloadPhoto(photo.MediaData);
                            //    string Path = @"C:\Users\$dmin\Pictures\Test\";
                            //    File.WriteAllBytes(Path + photo.ID + ".jpg", photo.Picture);
                            //}
                        //}
                        //m.Photos.Add(new PictureModel(m.ID, 0, await telegramService.DownloadPhoto(m.media)));
                    }
                }
            });

            //SelectMessage = SelectedDialog.Messages.SingleOrDefault(m => m.ID == lastMsgID);
            //SelectedDialog.Messages.ForAll(m =>
            //{
            //    string path = @"C:\Users\Sazke\Documents\meshistiryafter.txt";
            //    File.AppendAllTextAsync(path, m.ID.ToString() + "\n");
            //});
        }

        private async Task GetProfilePhoto()
        {
            //Dialogs.AsParallel().ForAll(async c =>
            //{
            //    switch (c.Entity)
            //    {
            //        case UserModel user:
            //            {
            //                if (user.Avatar != null)
            //                {
            //                    var photo = await _telegramService.GetProfilePhoto(new TL.User
            //                    {
            //                        id = user.ID,
            //                        access_hash = user.AccessHash,
            //                        photo = user.avatar
            //                    });
            //                    user.Avatar = photo;
            //                }
            //                break;
            //            }
            //        case ChatModel chat:
            //            {
            //                if (chat.Photo != null)
            //                {
            //                    var photo = await _telegramService.GetProfilePhoto(new TL.Chat
            //                    {
            //                        id = chat.ID,
            //                        photo = chat.Photo
            //                    });
            //                    chat.Photo = photo;
            //                }
            //                break;
            //            }
            //        case ChannelModel channel:
            //            {
            //                if (channel.Photo != null)
            //                {
            //                    var photo = await _telegramService.GetProfilePhoto(new TL.Channel
            //                    {
            //                        id = channel.ID,
            //                        access_hash = channel.AccessHash,
            //                        photo = channel.Photo
            //                    });
            //                    channel.Photo = photo;
            //                }
            //                break;
            //            }
            //    }
            //});

            await Task.Delay(1);
        }

        private async Task GetPrevMessages(object o)
        {
            var fstMsgID = selectedDialog.Messages.FirstOrDefault().ID;
            var messages = await _telegramService.GetMessagesHistoryDialog(selectedDialog, fstMsgID, 10);
            messages.ForAll(m => SelectedDialog.Messages.Insert(0, m));
            //SelectMessage = SelectedDialog.Messages.SingleOrDefault(m => m.ID == fstMsgID);
        }

        private async Task GetNextMessages(object o)
        {
            var lastMsgID = selectedDialog.Messages.Last().ID + 1;

            if (selectedDialog.Messages.Last().ID == selectedDialog.TopMessage) return;

            var messages = await _telegramService.GetMessagesHistoryDialog(selectedDialog, lastMsgID, -20, 20);
            var index = SelectedDialog.Messages.Count();
            messages.ForEach(m =>
            {
                var find = SelectedDialog.Messages.AsParallel().SingleOrDefault(mes => mes.ID == m.ID);
                if (find == null)
                {
                    SelectedDialog.Messages.Insert(index, m);
                }
                else if (m is MessageModel mes)
                {
                    if (mes.Message == "" && mes.Media != null)
                    {
                        // группировка
                    }
                }
            });

            // Получение фото к сообщениям
            SelectedDialog.Messages.AsParallel().ForAll(async m =>
            {
                if (m is MessageModel mm)
                {
                    if (mm.Data.media != null)
                    {
                        mm.Media.ForAll(async (media) =>
                        {
                            if (media is PhotoModel photo)
                            {
                                photo.Picture = await _telegramService.DownloadPhoto(photo.MediaData);
                            }
                            if (media is DocumentModel doc)
                            {
                                var document = await _telegramService.DownloadPhoto(doc.MediaData);
                                var path = Directory.GetCurrentDirectory();
                                path += $"\\Data\\UserData\\Media\\{doc.Data.ID}.mp4";
                                File.WriteAllBytes($"{path}", document);
                                doc.Picture = path;
                            }
                        });
                        //if (mm.Media is MediaPhotoModel mpm)
                        //{
                        //    mpm.Photos.ForAll(async (p) =>
                        //    {
                        //        p.Picture = await _telegramService.DownloadPhoto(p.Data);
                        //    });
                        //}
                    }
                }
            });

            //SelectedDialog.Messages.ForAll(m =>
            //{
            //    string path = @"C:\Users\Sazke\Documents\meshistiryafter.txt";
            //    File.AppendAllTextAsync(path, m.ID.ToString() + "\n");
            //});
        }

        private async Task SendMediaMessage(object o)
        {
            if (message == null || Path == "") return;

            var MesSend = await _telegramService.SendMediaMessage(selectedDialog, message, Path);
            if (MesSend.id != 0)
            {
                Message = "";
                SelectedDialog.Messages.Add(new MessageModel(MesSend, isOriginNative: true));
            }

            if (IsSendMediaVisible) IsSendMediaVisible = false;
        }

        private async Task SendMessage(object o)
        {
            if (message is null) return;

            var MesSend = await _telegramService.SendMessage(selectedDialog, message);
            if (MesSend.id != 0)
            {
                Message = "";
                SelectedDialog.Messages.Add(new MessageModel(MesSend, isOriginNative: true));
            }
        }

        private async Task ReadMessage(object o)
        {
            var result = await _telegramService.ReadMessage(SelectedDialog, selectMessage.ID);

            if (result && SelectedDialog.Entity is ChannelModel)
            {
                SelectedDialog.Read_inbox_max_id = selectMessage.ID;
                //SelectedDialog.Unread_count -= 1;
                SelectedDialog.Unread_count = SelectedDialog.TopMessage - SelectedDialog.Read_inbox_max_id;
            }
            else if (result)
            {
                SelectedDialog.Read_outbox_max_id = selectMessage.ID;
                SelectedDialog.Unread_count -= 1;
            }
        }

        private void ReplyToSet()
        {
            ReplyTo = SelectMessage;
        }

        private void CancelReplyToSet()
        {
            ReplyTo = null;
        }

        private void SelectFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.Title = "Выбор файлов";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                Path = openFileDialog.FileName;
            }

            IsSendMediaVisible = true;
        }

        private void CancelSelectFile()
        {
            IsSendMediaVisible = false;
        }

        // Методы для обработки обновлений
        private void NewMessage(object Sender, NewMessageEventArgs e)
        {
            if (Dialogs.Count == 0) return;

            var dlg = Dialogs.SingleOrDefault(d =>d.Entity.ID == e.Message.Peer.ID);

            if (dlg != null)
            {
                if (dlg.Unread_count == 0)
                {
                    if (e.Message.GroupedId == 0)
                    {
                        dlg.Messages.Add(e.Message);
                    }
                    else
                    {
                        if (e.Message is MessageModel mm)
                        {
                            if (mm.Message != "")
                            {
                                GroupID = e.Message.GroupedId;
                                dlg.Messages.Add(e.Message);
                            }
                        }
                        else
                        {
                            var mes = dlg.Messages.SingleOrDefault(m => m.GroupedId == GroupID);
                            //if (mes != null)
                            //    mes.Photos.Add(e.Message.Photos[0]);
                        }
                    }
                }
                if (e.Message is MessageModel mmi)
                    if (!mmi.IsOriginNative)
                        dlg.Unread_count += 1;

                dlg.LastMessage = e.Message;
            }
        }

        private void ReadChannelInbox(object sender, ReadChannelInboxEventArgs e)
        {
            if (Dialogs.Count == 0) return;

            var dlgs = Dialogs.SingleOrDefault(x => x.Entity.ID == e.ReadChannelInbox.channel_id);
            if (dlgs != null)
            {
                dlgs.Read_inbox_max_id = e.ReadChannelInbox.max_id;
                dlgs.Unread_count = e.ReadChannelInbox.still_unread_count;
            }
        }

        private void ReadChannelOutbox(object sender, ReadChannelOutboxEventArgs e)
        {
            if (Dialogs.Count == 0) return;

            var dlgs = Dialogs.SingleOrDefault(x => x.Entity.ID == e.ReadChannelOutbox.channel_id);
            if (dlgs != null)
                dlgs.Read_outbox_max_id = e.ReadChannelOutbox.max_id;
        }

        private void ReadHistoryInbox(object sender, ReadHistoryInboxEventArgs e)
        {
            if (Dialogs.Count == 0) return;

            var dlgs = Dialogs.SingleOrDefault(x => x.Entity.ID == e.ReadHistoryInbox.peer.ID);
            if (dlgs != null)
            {
                dlgs.Read_inbox_max_id = e.ReadHistoryInbox.max_id;
                dlgs.Unread_count = e.ReadHistoryInbox.still_unread_count;
            }
        }

        private void ReadHistoryOutbox(object sender, ReadHistoryOutboxEventArgs e)
        {
            if (Dialogs.Count == 0) return;

            var dlgs = Dialogs.SingleOrDefault(x => x.Entity.ID == e.ReadHistoryOutbox.peer.ID);
            if (dlgs != null)
                dlgs.Read_outbox_max_id = e.ReadHistoryOutbox.max_id;
        }

        private void UpdChannel(object sender, ChannelEventArgs e)
        {
            if (Dialogs.Count == 0) return;

            var dlgs = Dialogs.SingleOrDefault(x => x.Entity.ID == e.UpdateChannel.channel_id);
            if (dlgs != null)
                dlgs.Entity.ID = e.UpdateChannel.channel_id;
        }

        private void UpdChat(object sender, ChatEventArgs e)
        {
            if (Dialogs.Count == 0) return;

            var dlgs = Dialogs.SingleOrDefault(x => x.Entity.ID == e.UpdateChat.chat_id);
            if (dlgs != null)
                dlgs.Entity.ID = e.UpdateChat.chat_id;
        }

        private void UpdUser(object sender, UserEventArgs e)
        {
            if (Dialogs.Count == 0) return;

            var dlgs = Dialogs.SingleOrDefault(x => x.Entity.ID == e.UpdateUser.user_id);
            if (dlgs != null)
                dlgs.Entity.ID = e.UpdateUser.user_id;
        }

        private bool DoesCollectionContainName(object dialogName)
        {
            DialogModel dlgName = dialogName as DialogModel;
            switch (dlgName?.Entity)
            {
                case UserModel user: return user.Name.ToLower().Contains(SearchText.ToLower());
                case ChatModel chat: return chat.Name.ToLower().Contains(SearchText.ToLower());
                case ChannelModel channel: return channel.Name.ToLower().Contains(SearchText.ToLower());
            }

            return false;
        }

        private void ShowMenu()
        {
            IsMenuVisible = true;
        }

        private void HideMenu()
        {
            IsMenuVisible = false;
        }
    }
}
