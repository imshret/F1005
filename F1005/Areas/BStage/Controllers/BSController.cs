using F1005.Areas.ForeignExchange.Controllers;
using F1005.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace F1005.Areas.BStage.Controllers
{
    public class BSController : Controller
    {
        // GET: BackStage/BS

        public ActionResult Index()
        {
            Session["User"] = "msit119";           

            return View();
        }


        public ActionResult IndexStock()
        {  
            MyInvestEntities db = new MyInvestEntities();
                       
            return View(db.StockHistory.ToList());
        }

        public ActionResult IndexFX()
        {
            MyInvestEntities db = new MyInvestEntities();

            return View(db.FXtradeTable.ToList());
        }

        public ActionResult IndexCashI()
        {
            MyInvestEntities db = new MyInvestEntities();

            return View(db.CashIncome.ToList());
        }

        public ActionResult IndexCashE()
        {
            MyInvestEntities db = new MyInvestEntities();

            return View();
        }


        public ActionResult IndexIs()
        {
            MyInvestEntities db = new MyInvestEntities();

            return View(db.Insurances.ToList());
        }


        public ActionResult IndexFund()
        {
            MyInvestEntities db = new MyInvestEntities();

            return View(db.Fund.ToList());
        }

        public JsonResult GetCashI()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = from c in db.FXtradeTable
                      select new GetDataViewModel{
                          NTD=c.NTD,
                          USD=c.USD
                      };
            dynamic retObject = new { data = ret.ToList() };
            return Json(retObject, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCashE()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = from c in db.FXtradeTable
                      select new GetDataViewModel
                      {
                          NTD = c.NTD,
                          USD = c.USD
                      };
            dynamic retObject = new { data = ret.ToList() };
            return Json(retObject, JsonRequestBehavior.AllowGet);
        }
    }
}