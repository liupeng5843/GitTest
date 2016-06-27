using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SACommon.Models
{
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string accessToken = httpContext.Request.Headers["AccessToken"];
            if (accessToken == null)
            {
                return false;
            }

            var items = accessToken.Split('_');
            if (items.Length != 2)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid AccessToken format.");
            }
            httpContext.Items.Add("UserId", new Guid(items[0]));
            return ApiValidatorService.IsValid(accessToken);
        }
    }

    public static class ApiValidatorService
    {
        public static bool IsValid(string accessToken)
        {
            var at = AccessTokenManager.GetAccessToken(accessToken);
            if (at != null)
            {
                return true;
            }
            return false;
        }
    }
}
