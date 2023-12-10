using Tefichat.Base;
using TL;

namespace Tefichat.Models.Media
{
    public class PhotoModel : ObservableObject
    {
        public MessageMedia? Data { get; set; }
        public int ColumnSpan { get; set; } = 1;
        public int Column { get; set; }
        public int Row { get; set; }
        public Photo Photo { get; set; }

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
            Data = mediaData;
            Picture = pic;
            if (Data != null )
            {
                if (Data is MessageMediaPhoto p)
                    Photo = (Photo)p.photo;
            }
        }
    }
}
