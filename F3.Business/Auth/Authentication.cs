using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace F3.Business.Auth
{
    public interface IAuthentication
    {
        Task<string> GetToken(string username, string password);
    }

    public class Authentication : IAuthentication
    {
        public async Task<string> GetToken(string username, string password)
        {
            //var tokenGenerator = new Firebase.TokenGenerator(ConfigurationManager.AppSettings.Get(""));
            //var authPayload = new Dictionary<string, object>()
            //{
            //  { "uid", "1" },
            //  { "some", "arbitrary" },
            //  { "data", "here" }
            //};
            //string token = tokenGenerator.CreateToken(authPayload);
            //return token;
            throw new NotImplementedException("");
        }
    }
}
