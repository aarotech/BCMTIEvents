using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCMTIEvents.Models;
using Newtonsoft.Json.Linq;

namespace BCMTIEvents.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            EventViewModel model = new EventViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(EventViewModel model)
        {
            model.EventList = new List<Event>();

            string httpResponseBody = Models.EventViewModel.GetXmlDataFromBCMTIOpen511API("events", model.ToString());
            //Parse the response into a event list
            JObject details = JObject.Parse(httpResponseBody);
            foreach (var eventItem in details["events"])
            {
                model.Messsage += "" + eventItem["id"] + eventItem["description"];
                Event eventToAdd = new Event()
                {
                    Description = eventItem["description"].ToString(),
                    Severity = eventItem["severity"].ToString(),
                    Type = eventItem["event_type"].ToString(),
                    Date = eventItem["created"].ToString()
                };
                foreach (var area in eventItem["areas"])
                {
                    eventToAdd.Areas.Add(area["name"].ToString());
                }
                model.EventList.Add(eventToAdd);
            }

            //model.Messsage += "model: " + model.ToString();
            //model.Messsage += httpResponseBody;

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}