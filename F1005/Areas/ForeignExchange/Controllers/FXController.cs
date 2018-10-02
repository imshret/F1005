using F1005.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace F1005.Areas.ForeignExchange.Controllers
{
    public class FXController : Controller
    {
        private MyInvestEntities db = new MyInvestEntities();
        // GET: FX
        //外匯買賣首頁
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            //MyInvestEntities dc = new MyInvestEntities();
            //傳給外匯買賣的外幣DropDownList
            var cr = db.CurrencyRate
                  .Select(s => new SelectListItem
                  {
                      Value = s.OnlineSell,
                      Text = s.CurrencyClass + " (" + s.Name + ")"
                  });
            ViewBag.CurrencyRate = new SelectList(cr, "Value", "Text");
            //ViewBag.CurrencyRate = new SelectList(db.CurrencyRate, "OnlineSell", "Name");
            return View();
        }
        //現在匯率
        public ActionResult History()
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            return View();
        }
        //匯率轉換
        public ActionResult ExchangeRates()
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            return View();
        }
    }
}