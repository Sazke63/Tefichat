﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tefichat.Base;
using TL;

namespace Tefichat.Models
{
    public class DialogModel : ObservableObject
    {
        public Peer Peer { get; }

        private int _topMessage;
        public int TopMessage
        {
            get => _topMessage;
            set
            {
                _topMessage = value;
                OnPropertyChanged(nameof(TopMessage));
            }
        }

        public int Top_message { get; set; }
        public int Read_inbox_max_id { get; set; }
        public int Read_outbox_max_id { get; set; }
        public int Unread_mentions_count { get; set; }
        public int Unread_reactions_count { get; set; }
        public PeerNotifySettings Notify_settings { get; set; }
        public DraftMessageBase Draft { get; set; }
        public int Folder_id { get; set; }
        public int Ttl_period { get; set; }

        private int _unread_count;
        public int Unread_count
        {
            get => _unread_count;
            set
            {
                if (_unread_count > 0)
                {
                    _unread_count = value;
                    OnPropertyChanged(nameof(Unread_count));
                }
            }
        }

        public DialogModel(Dialog dialog) //, Contact contact)
        {
            //Contact = contact;
            Peer = dialog.Peer;
            _topMessage = dialog.top_message;
            Read_inbox_max_id = dialog.read_inbox_max_id;
            Read_outbox_max_id = dialog.read_outbox_max_id;
            _unread_count = dialog.unread_count;
            Unread_mentions_count = dialog.unread_mentions_count;
            Unread_reactions_count = dialog.unread_reactions_count;
            Notify_settings = dialog.notify_settings;
            Draft = dialog.draft;
            Folder_id = dialog.folder_id;
            Ttl_period = dialog.ttl_period;
        }
    }
}
