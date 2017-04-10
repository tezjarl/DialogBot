using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;

namespace DialogBot.CalendarScheduler
{
    [Serializable]
    public class Scheduler : IDialog<EventDate>
    {
        EventDate date;
        
        public async Task StartAsync(IDialogContext context)
        {
            if (date == null) date = new EventDate();
            context.Wait(MessageReceivedAsync);

        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var repl =await Reply(message.Text, context);
            await context.PostAsync(repl);
            context.Wait(MessageReceivedAsync);
        }

        private async Task<string> Reply(string text, IDialogContext context)
        {
               
           if (text.Contains("help"))
            {
                return @"This is a simple event bot.
Example of commands include:
  event today
  event tomorrow
  do i have smth at 10.09.2016
Your date input must be in format dd.mm.yyyy";
            }
            if (text.Contains("hi") || text.Contains("hello"))
                return "Hello! I am an event bot.I can tell you if you have any event at the specific date";
            if (text.Contains("bye"))
                return "Good bye! Have a good day!";
            if (text.Contains("who are you"))
                return
                    "I am an event bot. If you want to know about any of your events at the specific date, you can ask me about it";
            if (text.Contains("develop") || text.Contains("create") || text.Contains("invent"))
                return "I was developed by Alex Popovkin and Alex Chertov";
            if (text.Contains("authorize"))
            {
               
                using (var client = new GoogleCalendarClient())
                {
                    return await client.Authorize();
                }
            }
            if (text.Contains("check"))
            {
                using (var client = new GoogleCalendarClient())
                {
                    return await client.GetEvents(new DateTime(2016,1,1));
                }
              


            }
            return string.Empty;
            /*if (text.Contains("event") && text.Contains("today"))
                return await date.BuildResult(Dates.Today);
            if (text.Contains("event") && text.Contains("tomorrow"))
                return await date.BuildResult(Dates.Tomorrow);
            if (text.Contains("remind me about"))
                return date.AddDate(text.Replace("remind me about","").Replace("at",""));
            CultureInfo culture=new CultureInfo("ru-RU");
            var match = Regex.Match(text,"[0-9]{1,2}.{1}[0-9]{1,2}.20[0-9]{2}");
            if (match.Success)
            {
                DateTime input;
                bool isCorrectDate = DateTime.TryParse(match.Value, culture.DateTimeFormat,DateTimeStyles.None, out input);
                if (!isCorrectDate) return "You enter an incorrect date";
                else date.date = input;
            } 
            
            return await date.BuildResult();*/
        }
    }
}