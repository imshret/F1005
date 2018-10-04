using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace F1005.Areas.Main.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            XDocument doc = XDocument.Load("https://udn.com/rssfeed/news/2/6645?ch=news");

            var p = (from c in doc.Descendants("item")
                     select c).Select(pp => new
                     {
                         des = pp.Element("description").Value
                     }).ToList();


            for (int i = 0; i < p.Count; i++)
            {
                Match m = Regex.Match(p[i].des, @"<p>\s*(.+?)\s*</p>");
                if (m.Success)
                {
                    Console.WriteLine(m.Groups[1].Value);
                }
            }

            var query = (from f in doc.Descendants("item")
                         where DateTime.Parse(f.Element("pubDate").Value) > DateTime.Now.AddHours(-24)
                         select f).Select(c => new RssViewModel
                         {
                             Title = c.Element("title").Value,
                             Link = c.Element("link").Value,
                             Description = c.Element("description").Value,
                             PubDate = DateTime.Parse(c.Element("pubDate").Value).ToLongTimeString()
                         }).Take(10);
            return View(query);
        }


        public ActionResult IndexX()
        {
            Session["User"] = "msit119_one";

            XDocument doc = XDocument.Load("https://udn.com/rssfeed/news/2/6645?ch=news");

            var p = (from c in doc.Descendants("item")
                     select c).Select(pp => new
                     {
                         des = pp.Element("description").Value
                     }).ToList();


            for (int i = 0; i < p.Count; i++)
            {
                Match m = Regex.Match(p[i].des, @"<p>\s*(.+?)\s*</p>");
                if (m.Success)
                {
                    Console.WriteLine(m.Groups[1].Value);
                }
            }

            var query = (from f in doc.Descendants("item")
                         where DateTime.Parse(f.Element("pubDate").Value) > DateTime.Now.AddHours(-24)
                         select f).Select(c => new RssViewModel
                         {
                             Title = c.Element("title").Value,
                             Link = c.Element("link").Value,
                             Description = c.Element("description").Value,
                             PubDate = DateTime.Parse(c.Element("pubDate").Value).ToLongTimeString()
                         }).Take(10);
            return View(query);
        }

    }
}