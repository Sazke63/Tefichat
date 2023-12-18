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
        private int c = 1;
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
            switch(Photos.Count())
            {
                case 1: Photos[0].ColumnSpan = 2; Photos[0].RowSpan = 2; break;
                case 2:
                    {
                        bool ultraWidth = false;
                        for(int i = 0; i < Photos.Count; i++)
                        {
                            if (Photos[i].Data is MessageMediaPhoto mediaPhoto)
                            {
                                var photo = (Photo)mediaPhoto.photo;
                                double divide = (double)photo.sizes[1].Width / (double)photo.sizes[1].Height;
                                if (divide > 2.0)
                                {
                                    ultraWidth = true;
                                    break;
                                }
                            }                              
                        }

                        if (ultraWidth)
                        {
                            R = 2;
                            Photos[1].Row = 1;
                            Photos[0].ColumnSpan = 2;
                            Photos[1].ColumnSpan = 2;
                        }                           
                        else
                        {
                            C = 2;
                            Photos[1].Column = 1;
                            Photos[0].RowSpan = 2;
                            Photos[1].RowSpan = 2;
                        }                            
                        break;
                    }
                    case > 3:
                    {
                        C = R = 2;
                        var mediaPhoto = (MessageMediaPhoto)Photos[0].Data;
                        var photo = (Photo)mediaPhoto.photo;
                        if (photo.sizes[1].Width > photo.sizes[1].Height)
                        {
                            Photos[0].ColumnSpan = 2;

                            for (int i = 1; i < Photos.Count(); i++)
                            {
                                Photos[i].Column = i - 1;
                                Photos[i].Row = 1;
                            }
                        }
                        else
                        {
                            Photos[0].RowSpan = 2;
                            for (int i = 1; i < Photos.Count(); i++)
                            {
                                Photos[i].Column = i;
                                Photos[i].Row = i - 1;
                            }
                        }
                        break; 
                    }
            }

            //if (Photos.Count() == 1) Photos[0].ColumnSpan = 2;
            //if (Photos.Count() == 2) C = 1;

            //if (Photos.Count() > 2)
            //{
            //    var mediaPhoto = (MessageMediaPhoto)Photos[0].Data;
            //    var photo = (Photo)mediaPhoto.photo;
            //    if (photo.sizes[0].Width > photo.sizes[0].Height)
            //    {
            //        Photos[0].ColumnSpan = 2;
            //        R = 2;
            //        C = 2;

            //        for (int i = 1; i < Photos.Count(); i++)
            //        {
            //            Photos[i].Column = i - 1;
            //            Photos[i].Row = 1;
            //        }
            //    }
            //    else
            //    {

            //    }
            //}
        }
    }
}
