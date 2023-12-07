using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL;

namespace Tefichat.Models
{
    interface IDialogModel
    {
        public string Entry { get; set; }
        int TopMessage { get; }
    }
}
