using F1005.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace F1005.Areas.ForeignExchange.Controllers
{
    public class MoneyController : Controller
    {
        // GET: ForeignExchange/Money
        public ActionResult Index()
        {
            return View();
        }
        //讀取現在匯率("History","FX")
        public JsonResult Getlist()
        {
            var db = new F1005.Models.MyInvestEntities();
            //var ret = from c in db.CurrencyRate
            //          select c;
            var ret = db.CurrencyRate.ToList().Select(c => new GetCurrencyViewModel
            {
                ID = c.ID,
                CurrencyClass = c.CurrencyClass,
                CurrencyClassName=c.Name,
                OnlineBuy =c.OnlineBuy,
                OnlineSell=c.OnlineSell,
                Date=c.Date.Value.ToShortDateString(),
            });
            //dynamic retObject = new { data = ret.ToList() };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        //
        public JsonResult Getmoney()
        {
            var db = new MyInvestEntities();
            var ret = from c in db.CurrencyRate
                      select c;
            dynamic retObject = new { data = ret.ToList() };
            return Json(retObject, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.HttpGet]
        public JsonResult Getnowfc()
        {
            var rec = this.Request.QueryString.ToString();
            var db = new MyInvestEntities();
            var ret = db.CurrencyRate.Where(c => c.CurrencyClass == rec).Select(c => c.OnlineSell);
            //var ret = from c in db.CurrencyRate
            //          where c.CurrencyClass.Contains(rec)
            //          select c.OnlineSell;
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
    }
}