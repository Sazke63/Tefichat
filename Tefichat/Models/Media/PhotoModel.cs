﻿using TL;

namespace Tefichat.Models.Media
{
    public class PhotoModel : MediaModel //ObservableObject
    {
        public override MessageMedia? MediaData { get; set; }
        public override int ColumnSpan { get; set; } = 1;
        public override int RowSpan { get; set; } = 1;
        public override int Column { get; set; }
        public override int Row { get; set; }
        public Photo Data { get; set; }

        private byte[]? picture;
        public byte[]? Picture
        {
            get => picture;
            set
            {
                picture = value;
                OnPropertyChanged(nameof(Picture));
            }
        }

        public PhotoModel(MessageMedia mediaData, byte[]? pic = null)
        {
            MediaData = mediaData;
            Picture = pic;
            if (Data == null)
            {
                if (MediaData is MessageMediaPhoto p)
                    Data = (Photo)p.photo;
            }
        }
    }
}
