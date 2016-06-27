using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACommon.Models
{
    public static class AccessTokenManager
    {
        //by default: ConfigurationManager.ConnectionStrings[0] is {data source=.\SQLEXPRESS;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|aspnetdb.mdf;User Instance=true}
        private static readonly string _accessTokenDbConnectionString = (ConfigurationManager.ConnectionStrings["AccessTokenDb"]??
            ConfigurationManager.ConnectionStrings[1]).ConnectionString;
        private static readonly long _accessTokenExpireSeconds = long.Parse(ConfigurationManager.AppSettings["AccessTokenExpireSeconds"] ?? "86400000"); // default, 8 hours

        public static AuthenticationAccessToken AddAccessToken(Guid userId, string ipAddress, string userAgent)
        {
            var accessToken = new AuthenticationAccessToken()
            {
                AccessToken = GenerateAccessToken(userId),
                Content = string.Empty,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };

            using (var db = GetSADbContext())
            {
                db.AccessTokens.Add(accessToken);
                db.SaveChanges();
            }

            return accessToken;
        }

        public static AuthenticationAccessToken GetAccessToken(string accessToken)
        {
            using (var db = GetSADbContext())
            {
                var at = db.AccessTokens.Where(a => a.AccessToken == accessToken).FirstOrDefault();
                if (at == null || at.CreatedDate.AddSeconds(_accessTokenExpireSeconds) < DateTime.UtcNow)
                {
                    return null;
                }
                return at;
            }
        }

        public static void RemoveAccessToken(string accessToken)
        {
            using (var db = GetSADbContext())
            {
                var at = db.AccessTokens.Where(a => a.AccessToken == accessToken).FirstOrDefault();
                if (at != null)
                {
                    db.AccessTokens.Remove(at);
                    db.SaveChanges();
                }
            }
        }

        private static string GenerateAccessToken(Guid userId)
        {
            var accessToken = userId.ToString("N") + "_" + Guid.NewGuid().ToString("N");
            return accessToken;
        }

        private static SADbContext GetSADbContext()
        {
            return new SADbContext(_accessTokenDbConnectionString);
        }
    }
}
