using System;
using TL;

namespace Tefichat.Services.EventAgs
{
    public class UserStatusEventArgs : EventArgs
    {
        public UpdateUserStatus UpdateUserStatus { get; }

        public UserStatusEventArgs(UpdateUserStatus updateUserStatus)
        {
            UpdateUserStatus = updateUserStatus;
        }
    }
}
