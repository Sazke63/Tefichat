﻿using System;
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
        public bool IsVisibleChat = false;
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
            DownloadData();
        }

        public async Task DownloadData()
        {
            //List<DialogModel> dlgs = new List<DialogModel>();
            var dlgs = await _telegramService.GetAllDialogs();
            if (dlgs != null)
                dlgs.ForEach(c => Dialogs.Add(c));
        }

        private async Task GetLastMessages()
        {
            if (!IsVisibleChat) IsVisibleChat = true;
            //var messages = await _telegramService.GetLastMessages();

            //if (messages != null)
            //{
            //    Dialogs.AsParallel().ForAll(d =>
            //    {
            //        var mes = messages.SingleOrDefault(m => m.Peer.ID == d.Peer.ID);
            //        if (mes != null)
            //        {
            //            d.Contact.Messages.Add(mes); //new TelMessage(mes));
            //            d.Contact.LastMessage = mes; //new TelMessage(mes);
            //        }
            //    });
            //}

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
