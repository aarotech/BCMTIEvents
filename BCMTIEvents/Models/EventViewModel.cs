using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace BCMTIEvents.Models
{
    public class EventViewModel
    {
        public static string defaultBcMTIOpen511Uri = "https://api.open511.gov.bc.ca/";

        public EventViewModel()
        {
            eventIdList = GetListOfEventIds();
            severityList = GetListOfSeveritys();
            eventTypeList = GetListOfEventTypes();
            StartDate = default(DateTime);
            EventList = new List<Event>();
        }

        [DisplayName("EventList")]
        public List<Event> EventList { get; set; }

        [DisplayName("AreaName")]
        public string AreaName { get; set; }


        public string AreaId { get; set; }

        [DisplayName("Severity")]
        public string Severity { get; set; }

        [DisplayName("Type")]
        public string Type { get; set; }

        [DisplayName("Date")]
        public DateTime StartDate { get; set; }

        [DisplayName("Message")]
        public string Messsage { get; set; }

        public List<SelectListItem> eventIdList { get; set; }
        public List<SelectListItem> severityList { get; set; }
        public List<SelectListItem> eventTypeList { get; set; }

        public List<SelectListItem> GetListOfEventIds()
        {
            eventIdList = new List<SelectListItem>();

            //Retrieve the list of all Event IDs and parse it into xml document
            string eventJSon = GetXmlDataFromBCMTIOpen511API("areas");

            //Parse the response into a selection list
            JObject details = JObject.Parse(eventJSon);
            eventIdList.Add(new SelectListItem { Text = "", Value = null });
            foreach (var area in details["areas"])
            {
                eventIdList.Add(new SelectListItem { Text = area["name"].ToString(), Value = area["id"].ToString() });
            }

            return eventIdList;
        }

        public List<SelectListItem> GetListOfSeveritys()
        {
            severityList = new List<SelectListItem>();
            severityList.Add(new SelectListItem { Text = "", Value = null });
            severityList.Add(new SelectListItem { Text = "MAJOR", Value = "MAJOR" });
            severityList.Add(new SelectListItem { Text = "MINOR", Value = "MINOR" });
            return severityList;
        }

        public List<SelectListItem> GetListOfEventTypes()
        {
            eventTypeList = new List<SelectListItem>();
            eventTypeList.Add(new SelectListItem { Text = "", Value = null });
            eventTypeList.Add(new SelectListItem { Text = "Road Condition", Value = "ROAD_CONDITION" });
            eventTypeList.Add(new SelectListItem { Text = "Weather Condition", Value = "WEATHER_CONDITION" });
            eventTypeList.Add(new SelectListItem { Text = "Incident", Value = "INCIDENT" });
            eventTypeList.Add(new SelectListItem { Text = "Construction", Value = "CONSTRUCTION" });
            return eventTypeList;
        }

        public static string GetXmlDataFromBCMTIOpen511API(string service, string queryString = null)
        {
            //Establish parameters of ssl connection
            Uri bcMTIOpen511Uri = new Uri(defaultBcMTIOpen511Uri);
            ServicePoint pointOfService = ServicePointManager.FindServicePoint(bcMTIOpen511Uri);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string httpResponseBody;
            using (HttpClient clientInstance = new HttpClient())
            {
                //Transmit GET to BCMTI api service
                Task<HttpResponseMessage> messageTask = clientInstance.GetAsync(defaultBcMTIOpen511Uri + service + "?" + queryString);
                System.Runtime.CompilerServices.TaskAwaiter<HttpResponseMessage> messageProcessor = messageTask.GetAwaiter();
                HttpResponseMessage responseMessage = messageProcessor.GetResult();

                //Capture reply of BCMTI api service
                Task<string> replyTask = responseMessage.Content.ReadAsStringAsync();
                System.Runtime.CompilerServices.TaskAwaiter<string> replyProcessor = replyTask.GetAwaiter();
                httpResponseBody = replyProcessor.GetResult();
            }
            return httpResponseBody;
        }

        public override string ToString()
        {
            string queryString = "";
            if (!string.IsNullOrEmpty(this.Type))
            {
                if (!string.IsNullOrEmpty(queryString)) { queryString += "&"; }
                queryString += "event_type=" + this.Type;
            }

            if (!string.IsNullOrEmpty(this.Severity))
            {
                if (!string.IsNullOrEmpty(queryString)) { queryString += "&"; }
                queryString += "severity=" + this.Severity;
            }

            if (!string.IsNullOrEmpty(this.AreaId))
            {
                if (!string.IsNullOrEmpty(queryString)) { queryString += "&"; }
                queryString += "area_id=" + this.AreaId;
            }

            if (this.StartDate != default(DateTime))
            {
                if (!string.IsNullOrEmpty(queryString)) { queryString += "&"; }
                queryString += "start_date=" + this.StartDate;
            }
            return queryString;
        }
    }

    public class Event
    {
        public Event()
        {
            Areas = new List<string>();
        }

        [DisplayName("Areas")]
        public List<string> Areas { get; set; }

        [DisplayName("Severity")]
        public string Severity { get; set; }

        [DisplayName("Type")]
        public string Type { get; set; }

        [DisplayName("Date")]
        public string Date { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

    }
}