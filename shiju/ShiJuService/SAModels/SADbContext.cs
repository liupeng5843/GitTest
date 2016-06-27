using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACommon.Models
{
    internal class SADbContext : DbContext
    {
        public DbSet<AuthenticationAccessToken> AccessTokens { get; set; }
        public SADbContext(string connectionString)
            : base(connectionString)
        { }
    }
}
