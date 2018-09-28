using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace F1005.Areas.Main.Controllers
{
    public class HomeController : Controller
    {
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                Session["User"] = "msit119_one";
            }


            XDocument doc = XDocument.Load("https://udn.com/rssfeed/news/2/6645?ch=news");

            var query = (from f in doc.Descendants("item")
                         where DateTime.Parse(f.Element("pubDate").Value) > DateTime.Now.AddHours(-2)
                         select f).Select(c => new RssViewModel
                         {
                             Title = c.Element("title").Value,
                             Link = c.Element("link").Value,
                             Description = c.Element("description").Value,
                             PubDate = DateTime.Parse(c.Element("pubDate").Value).ToLongTimeString()
                         });
            return View(query);
        }
    }
}