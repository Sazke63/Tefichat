using Tefichat.Base;
using TL;

namespace Tefichat.Models.Media
{
    public abstract class MediaModel : ObservableObject
    {
        public virtual MessageMedia? MediaData { get; set; }
        public virtual int ColumnSpan { get; set; } = 1;
        public virtual int RowSpan { get; set; } = 1;
        public virtual int Column { get; set; }
        public virtual int Row { get; set; }
    }
}
