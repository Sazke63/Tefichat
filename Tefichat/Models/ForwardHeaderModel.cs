using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tefichat.Base;

namespace Tefichat.Models
{
    public class ForwardHeaderModel : ObservableObject
    {
        public IPeerInfoModel From { get; set; }
        public int ChannelPost { get; set; }

        public ForwardHeaderModel(IPeerInfoModel from, int channelPost)
        {
            From = from;
            ChannelPost = channelPost;
        }
    }
}
