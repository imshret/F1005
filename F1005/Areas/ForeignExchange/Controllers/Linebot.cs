using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace F1005.Areas.ForeignExchange.Controllers
{
    //public class Linebot: ApiController
    //{
    //    [HttpGet]
    //    public HttpResponseMessage Index()
    //    {
    //        return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Service is LIVE!!");
    //    }

    //    [HttpPost]
    //    public async Task<HttpResponseMessage> Post()
    //    {
    //        var rawData = Request.Content.ReadAsStringAsync().Result;
    //        //var botHelper = BotHelper
    //        Trace.TraceInformation($"rawData:{rawData}");
    //        try
    //        {
    //            //var result = await 
    //        }
    //        catch (Exception exp)
    //        {
    //            Trace.TraceError(exp.Message);
    //        }
    //        return Request.CreateResponse(System.Net.HttpStatusCode.OK);
    //    }
    //}
    public class Linebotclass
    {
        public List<Event> events;
        public Linebotclass()
        {
            events = new List<Event>();
        }
    }
    public class Event
    {

        public string type { get; set; }
        public Source source { get; set; }
        public EventMessage message { get; set; }
        public string replyToken { get; set; }
        public Event()
        {
            source = new Source();
            message = new EventMessage();
        }
    }
    public class Source
    {
        public string userId { get; set; }
    }
    public class EventMessage
    {
        public string id { get; set; }
        public string type { get; set; }
        public string text { get; set; }
    }
}