using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiJu.Models
{
    public enum NotificationType : long
    {
        FriendRequest = 1,
        EnrollParty = 2,
        UpdateParty = 3,
        InviteUser = 4,
        Greet = 5
    }
}
