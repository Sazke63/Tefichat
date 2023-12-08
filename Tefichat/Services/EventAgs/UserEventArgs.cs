using System;
using TL;

namespace Tefichat.Services.EventAgs
{
    public class UserEventArgs : EventArgs
    {
        public UpdateUser UpdateUser { get; }

        public UserEventArgs(UpdateUser updateUser)
        {
            UpdateUser = updateUser;
        }
    }
}
