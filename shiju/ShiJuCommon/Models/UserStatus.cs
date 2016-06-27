using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiJu.Models
{
    public enum UserStatus : long
    {
        Active = 0, // Normal user
        Inactive = 1, // The user has been invited by its phone number, but he or she has not been signed up yet.
        Disabled = 2, // Disabled
        Inexsitence=3// has not in db
    }
}
