using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace F3.Infrastructure
{
    public class AuthAccessToken : IAccessToken
    {
        public string AccessToken
        {
            get
            {
                var context = HttpContext.Current.User.Identity;
                if (context is ClaimsIdentity)
                {
                    var claimsPrinc = (ClaimsIdentity) HttpContext.Current.User.Identity;
                    var accessToken = claimsPrinc.Claims.FirstOrDefault(c => c.Type.Equals("urn:googleplus:access_token"));
                    if (accessToken != null)
                    {
                        return accessToken.Value;
                    }
                }
                throw new Exception("Access Token not found.");
            }
        }
    }
}
