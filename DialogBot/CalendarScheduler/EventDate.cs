using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DialogBot.CalendarScheduler
{
    [Serializable]
    public class EventDate
    {
        public DateTime?  date{get;set;}
        public async Task<string> BuildResult(Dates specifiedDate=Dates.Default)
        {
            CalendarClient client = new CalendarClient();
            if (specifiedDate==Dates.Today) return await client.CheckDate(DateTime.Today);
            if (specifiedDate == Dates.Tomorrow) return await client.CheckDate((DateTime.Today).AddDays(-1));
            return await client.CheckDate(date);
            
        }

        public string AddDate(string text)
        {
            DateTime date;
            CalendarClient client =new CalendarClient();
            Regex regex=new Regex("[0-9]{1,2}.{1}[0-9]{1,2}.20[0-9]{2}");
            var dateText = regex.Match(text).Value;
            var eventName = regex.Replace(text, "");

            var isDateParsed = DateTime.TryParse(dateText,new CultureInfo("ru-RU").DateTimeFormat,DateTimeStyles.None,out date);
            if (!isDateParsed) return "You have entered an incorrect date for the event";
            client.AddDateToDictionary(eventName, date);
            return "Date succesfully added";
            
        }
        
    }
    public enum Dates { Today,Tomorrow,Default,
        Added
    }
}