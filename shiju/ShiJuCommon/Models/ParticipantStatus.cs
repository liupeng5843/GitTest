using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiJu.Models
{
    public enum ParticipantStatus : long
    {
        Created = 0,
        Accepted = 1, // The user accept to participate the party after he or she is at-ed.
        QuitAfterAccepting = 2,
        Enrolled = 3,  // The user enroll the party  
        QuitAfterEnroll = 4,
        Refused=5,
    }

}
