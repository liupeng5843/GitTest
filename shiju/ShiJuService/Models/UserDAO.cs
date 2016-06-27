using ShiJu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WindowsServer.Caching;

namespace ShiJu.Service.Models
{
    public static class UserDAO
    {
        public static User GetUserWithCahce(ShiJuDbContext db, Guid userId)
        {
            var user = CacheManager.Get<User>(
                new CacheBaseItem("User_" + userId.ToString("N"), 5),
                () =>
                {
                    return db.Users.Find(userId);
                });
            return user;
        }

    }
}