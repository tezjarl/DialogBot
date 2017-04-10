using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace DialogBot.CalendarScheduler
{
    
    class GoogleCalendarClient: IDisposable
    {
        private static UserCredential User;

        public async Task<string> Authorize()
        {
            
            using (var stream = new FileStream(HttpContext.Current.Server.MapPath("~/client_secret.json"), FileMode.Open, FileAccess.Read))
            {

                 var userCredential =  await GoogleWebAuthorizationBroker.AuthorizeAsync(
                   GoogleClientSecrets.Load(stream).Secrets,
                   new[] { CalendarService.Scope.Calendar },
                   "user", CancellationToken.None, new NullDataStore(), new LocalServerCodeReceiver());
                GoogleCalendarClient.User = userCredential;
                return "success";
            }
        }

        public async Task<string> GetEvents(DateTime eventDate)
        {
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = User,
                ApplicationName = "BotApp"
            });
            EventsResource.ListRequest request = service.Events.List("primary");
            Events events = await request.ExecuteAsync();
            StringBuilder sb =new StringBuilder();
            foreach (var calendarEvents in events.Items)
            {
                sb.AppendLine(calendarEvents.Id);
            }
            return sb.ToString();
        }

        public void Dispose()
        {
        }
    }
}
