using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL;

namespace Tefichat.Models.Media
{
    public class MediaDocumentModel : MediaModel
    {
        private ObservableCollection<DocumentModel>? documents = new ObservableCollection<DocumentModel>();
        public ObservableCollection<DocumentModel>? Documents
        {
            get => documents;
            set
            {
                documents = value;
                OnPropertyChanged(nameof(Documents));
            }
        }
    }
}
