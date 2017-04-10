using System;
using System.IO;
using System.Web;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;

namespace DialogBot.CalendarScheduler
{
    [Serializable]
    public class SimpleGoogleCredential
    {
        public TokenResponse Token { get; set; }
        public string UserID { get; set; }
        public IAuthorizationCodeFlow AuthFlow { get; set; }

        public SimpleGoogleCredential(TokenResponse token, string userId)
        {
            UserID = userId;
            Token = token;
            
        }
    }
}