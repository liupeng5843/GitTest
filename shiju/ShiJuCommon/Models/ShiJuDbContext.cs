using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiJu.Models
{
    public class ShiJuDbContext : DbContext
    {
        public DbSet<VerificationCode> VerificationCodes { get; set; }
        //public DbSet<Preference> Preferences { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Party> Parties { get; set; }
        public DbSet<PartyComment> PartyComments { get; set; }
        public DbSet<PartyLike> PartyLikes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Greeting> Greetings { get; set; }
        public DbSet<UserGreeting> UserGreetings { get; set; }


        public ShiJuDbContext()
        {
        }

        public ShiJuDbContext(string connectionString)
            : base(connectionString)
        {
        }
    }
}
