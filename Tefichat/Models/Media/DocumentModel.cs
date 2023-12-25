using System;
using System.Collections.Generic;
using System.Linq;
using TL;

namespace Tefichat.Models.Media
{
    public class DocumentModel : MediaModel
    {
        public override MessageMedia? MediaData { get; set; }
        public override int ColumnSpan { get; set; } = 1;
        public override int RowSpan { get; set; } = 1;
        public override int Column { get; set; }
        public override int Row { get; set; }
        public Document Data { get; set; }

        private string? picture;
        public string? Picture
        {
            get => picture;
            set
            {
                picture = value;
                OnPropertyChanged(nameof(Picture));
            }
        }
        public DocumentModel(MessageMedia mediaData, string? pic = null)
        {
            MediaData = mediaData;
            Picture = pic;
            if (Data == null)
            {
                if (MediaData is MessageMediaDocument d)
                    Data = (Document)d.document;
            }
        }
    }
}
