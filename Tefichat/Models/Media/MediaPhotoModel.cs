using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL;

namespace Tefichat.Models.Media
{
    public class MediaPhotoModel : MediaModel
    {
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
        private int c = 0;
        public int C 
        {
            get => c; 
            set
            {
                c  = value;
                OnPropertyChanged(nameof(C));
            }
        }

        private ObservableCollection<PhotoModel>? photos = new ObservableCollection<PhotoModel>();
        public ObservableCollection<PhotoModel>? Photos 
        {
            get => photos;
            set
            {
                photos = value;
                OnPropertyChanged(nameof(Photo));
            }
        }

        public void GetGrid()
        {
            if (Photos.Count() == 2) C = 1;

            if (Photos.Count() > 2)
            {
                var mediaPhoto = (MessageMediaPhoto)Photos[0].Data;
                var photo = (Photo)mediaPhoto.photo;
                if (photo.sizes[0].Width > photo.sizes[0].Height)
                {
                    Photos[0].ColumnSpan = 2;
                    R = 2;
                    C = 2;

                    for (int i = 1; i < Photos.Count(); i++)
                    {
                        Photos[i].Column = i - 1;
                        Photos[i].Row = 1;
                    }
                }
                else
                {

                }
            }
        }
    }
}
