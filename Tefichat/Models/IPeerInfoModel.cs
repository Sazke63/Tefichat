using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tefichat.Models
{
    public interface IPeerInfoModel
    {
        long ID { get; set; }
        bool IsActive { get; set; }
        public long AccessHash { get; set; }
    }
}
