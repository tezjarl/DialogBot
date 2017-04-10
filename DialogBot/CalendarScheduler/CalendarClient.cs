using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace DialogBot.CalendarScheduler
{
    public class CalendarClient
    {
        private static CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
    CloudConfigurationManager.GetSetting("StorageConnectionString"));
        private static Dictionary<DateTime, string> forbidenDates = new Dictionary<DateTime, string>
        {
            {new DateTime(2016,11,1),"Devcon School Future Technologies"  },
            {new DateTime(2016,9,17),"DevCon School NN and Presentations Day" },
            {new DateTime(2016,12,31),"New Year's Eve" },
            {new DateTime(2016,9,23),"Dev&Test Forum" },
            {new DateTime(2016,12,9),"DotNext 2016 Msc" }
        };

        private static string[] suitableDateVariants = {
            "The date is suitable for you", "You don't busy at this day",
            "You don't plan anything for this day"
        };
        public async Task<string> CheckDate(DateTime? inputDate)
        {
            if (!inputDate.HasValue) return "Date is not set";
            if (!forbidenDates.ContainsKey(inputDate.Value))
            {
                Random rnd=new Random();
                return suitableDateVariants[rnd.Next(0,2)];
            }
            return $"You have {forbidenDates[inputDate.Value]} at this day";
        }

        public void AddDateToDictionary(string eventName, DateTime date)
        {
            forbidenDates.Add(date,eventName);
        }

        static CalendarClient()
        {
          /*  CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("events");

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();
            TableQuery<EventDate> query=new TableQuery<EventDate>();
            foreach (var element in table.ExecuteQuery(query))
            {
                
            }*/
        }
    }
   
}
public class EventDate:TableEntity
{
public DateTime date { get; set; }
public string eventName { get; set; }
}