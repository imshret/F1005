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

        public JsonResult Getlist()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = from c in db.CurrencyRate
                      select c;
            dynamic retObject = new { data = ret.ToList() };
            return Json(retObject, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Getmoney()
        {
            var db = new MyInvestEntities();
            var ret = from c in db.CurrencyRate
                      select c;
            dynamic retObject = new { data = ret.ToList() };
            return Json(retObject, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.HttpGet]
        public ActionResult Getnowfc()
        {
            var rec = this.Request.QueryString.ToString();
            var db = new MyInvestEntities();
            var ret = from c in db.CurrencyRate
                      where c.CurrencyClass.Contains(rec)
                      select c.OnlineSell;
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
    }
}