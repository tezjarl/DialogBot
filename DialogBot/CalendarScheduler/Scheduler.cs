using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;
using Microsoft.Bot.Builder.Luis;
using Newtonsoft.Json;

namespace DialogBot.CalendarScheduler
{
    [Serializable]
    public class Scheduler : IDialog<EventDate>
    {
        EventDate date;

        public async Task StartAsync(IDialogContext context)
        {
            //if (date == null) date = new EventDate();
            context.Wait(MessageReceivedStartAsync);
        }

        private async Task MessageReceivedStartAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            await context.PostAsync(
                "Hello! I am a CalendarBot. I can help you to manage your events. Because I use Google Calendar, firstly u need to authorize me in your Google Account. Just Send an \"authorize\" command to proceed or, if you need help send a \"help\" command");
            context.Wait(MessageReceivedCommandChoice);
            /* var message = await result;
             var repl =await Reply(message.Text, context);
             await context.PostAsync(repl);
             context.Wait(MessageReceivedAsync);*/
        }

        private async Task MessageReceivedCommandChoice(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            switch (message.Text)
            {
                case "authorize":
                    using (var client = new GoogleCalendarClient())
                    {
                        var authResult = await client.Authorize();
                        await context.PostAsync(authResult);
                    }
                    context.Wait(MessageReceivedCommandChoice);
                    break;
                case "help":
                    await context.PostAsync("@\"This is a simple event bot.\r\nExample of commands include:\r\n  event today\r\n  event tomorrow\r\n  do i have smth at 10.09.2016\r\nYour date input must be in format dd.mm.yyyy");
                    context.Wait(MessageReceivedCommandChoice);
                    break;
                default:
                    //context.Call<object>(Chain.From(()=>new CalendarDialog()), AfterChildDialogIsDone);
                    //context.Wait(MessageReceivedCommandChoice);
                    using (var client = new HttpClient())
                    {
                        string uri = @"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/06847bdf-356f-429d-acb8-65c550af1944?subscription-key=b4aa72fd0ff44a0aa12edf3e108821fc&timezoneOffset=0.0&verbose=true&q="+ message.Text;
                        HttpResponseMessage msg = await client.GetAsync(uri);
                        if (msg.IsSuccessStatusCode)
                        {
                            var jsonResponse = await msg.Content.ReadAsStringAsync();
                            var _Data = JsonConvert.DeserializeObject<LUISResponse>(jsonResponse);
                            await HandleEventQuery(context, _Data);
                        }


                    }
                    break;
            }
        }

        private async Task HandleEventQuery(IDialogContext context, LUISResponse _Data)
        {
            var topIntent = _Data.intents[0].intent;
            switch (topIntent)
            {
                case "GetEventsForSpecifiedDate":
                    using (var calendar = new GoogleCalendarClient())
                    {
                        var eventDate =
                            _Data.entities.FirstOrDefault(e => e.type == "builtin.datetime.date");
                        var events = await calendar.GetEvents(eventDate.resolution.date.Value);
                        await context.PostAsync(events);
                    }
                    break;
                case "SetUpEvent":
                    using (var calendar = new GoogleCalendarClient())
                    {
                        var eventDate =
                            _Data.entities.FirstOrDefault(e => e.entity == "builtin.datetime.date");
                        var eventName = _Data.entities.FirstOrDefault(e => e.type == "EventName");
                        var isCreated = await calendar.SetUpEvent(eventDate.resolution.date.Value, eventName.entity);
                        await context.PostAsync(isCreated);
                    }
                    break;
                default:
                    await context.PostAsync("Doesn't recognize a command");
                    break;
            }
            context.Wait(MessageReceivedCommandChoice);
        }


        public async Task AfterChildDialogIsDone(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedCommandChoice);
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
                    return await client.GetEvents(new DateTime(2016, 1, 1));
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