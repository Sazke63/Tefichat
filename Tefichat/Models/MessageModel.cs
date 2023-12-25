using System;
using System.Collections.ObjectModel;
using Tefichat.Models.Media;
using TL;

namespace Tefichat.Models
{
    public class MessageModel : MessageBaseModel
    {
        public Message Data { get; set; }

        //public string FromName
        //{
        //    get
        //    {
        //        if (From != null)
        //        {
        //            switch (From)
        //            {
        //                case UserModel user: return user.Name;
        //                case ChatModel chat: return chat.Name;
        //                case ChannelModel channel: return channel.Name;
        //            }                    
        //        }
        //        return From?.ID.ToString();
        //    }
        //}

        public string Message
        {
            get => Data.message;
            set
            {
                Data.message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public MessageMedia MediaT => Data.media;

        // Сетка
        private int r = 1;
        public int R
        {
            get => r;
            set
            {
                r = value;
                OnPropertyChanged(nameof(R));
            }
        }
        private int c = 1;
        public int C
        {
            get => c;
            set
            {
                c = value;
                OnPropertyChanged(nameof(C));
            }
        }

        private ObservableCollection<MediaModel> media = new ObservableCollection<MediaModel>();
        public ObservableCollection<MediaModel> Media
        {
            get => media;
            set
            {
                media = value;
                OnPropertyChanged(nameof(Media));
            }
        }
        public ForwardHeaderModel FwdFrom { get; set; }
        public override MessageBaseModel ReplyTo { get; set; }

        private DateTime date;
        public override DateTime Date
        {
            get => date.ToLocalTime();
            set => date = value;
        }

        public bool IsOriginNative { get; set; }

        public MessageModel(Message msg, IPeerInfoModel from = null, bool isOriginNative = false) : base(msg, from, msg.grouped_id)
        {
            Data = msg;
            Date = msg.date;
            IsOriginNative = isOriginNative;
        }

        public void GetGrid()
        {
            switch (Media.Count)
            {
                case 1: Media[0].ColumnSpan = 2; Media[0].RowSpan = 2; break;
                case 2:
                    {
                        bool ultraWidth = false;
                        for (int i = 0; i < Media.Count; i++)
                        {
                            double divide = 0;
                            if (Media[i] is PhotoModel mediaPhoto)
                            //if (Media[i].Data is MessageMediaPhoto mediaPhoto)
                            {
                                var photo = (Photo)mediaPhoto.Data;
                                divide = (double)photo.sizes[1].Width / (double)photo.sizes[1].Height;
                            }
                            if (Media[i] is DocumentModel mediaDoc)
                            {
                                var doc = (Document)mediaDoc.Data;
                                divide = (double)doc.thumbs[1].Width / (double)doc.thumbs[1].Height;
                            }

                            if (divide > 2.0)
                            {
                                ultraWidth = true;
                                break;
                            }
                        }

                        if (ultraWidth)
                        {
                            R = 2;
                            Media[1].Row = 1;
                            Media[0].ColumnSpan = 2;
                            Media[1].ColumnSpan = 2;
                        }
                        else
                        {
                            C = 2;
                            Media[1].Column = 1;
                            Media[0].RowSpan = 2;
                            Media[1].RowSpan = 2;
                        }
                        break;
                    }
                case > 3:
                    {
                        C = R = 2;
                        //var mediaPhoto = (MessageMediaPhoto)Media[0].Data;
                        if (Media[0].MediaData is MessageMediaPhoto mediaPhoto)
                        {
                            var photo = (Photo)mediaPhoto.photo;
                            if (photo.sizes[1].Width > photo.sizes[1].Height)
                            {
                                Media[0].ColumnSpan = 2;

                                for (int i = 1; i < Media.Count; i++)
                                {
                                    Media[i].Column = i - 1;
                                    Media[i].Row = 1;
                                }
                            }
                            else
                            {
                                Media[0].RowSpan = 2;
                                for (int i = 1; i < Media.Count; i++)
                                {
                                    Media[i].Column = i;
                                    Media[i].Row = i - 1;
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
