using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace DialogBot.CalendarScheduler
{
    [LuisModel("06847bdf-356f-429d-acb8-65c550af1944", "b4aa72fd0ff44a0aa12edf3e108821fc")]
    [Serializable]
    public class CalendarDialog : LuisDialog<object>
    {
        public const string EntityEventdate = "builtin.datetime.date";
       
        [LuisIntent("GetEventsForSpecifiedDate")]
        public async Task GetEvents(IDialogContext context, LuisResult result)
        {
            string eventDate = string.Empty;
            EntityRecommendation date;
            if (result.TryFindEntity(EntityEventdate, out date))
            {
                eventDate = date.Entity;
                using (var client = new GoogleCalendarClient())
                {
                    var events = await client.GetEvents(DateTime.Parse(eventDate));
                    await context.PostAsync(events);
                }
            }
            if (string.IsNullOrEmpty(eventDate))
            {
                await context.PostAsync("Please specify date");
            }
            context.Done<object>(new object());
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry I don't recognize your command");
            context.Done<object>(new object());
        }

    }
}