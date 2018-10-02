using F1005.Areas.BStage.Models;
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
                       
            return View();
        }

        public ActionResult IndexFX()
        {
            MyInvestEntities db = new MyInvestEntities();

            return View();
        }

        public ActionResult IndexCashI()
        {
            MyInvestEntities db = new MyInvestEntities();

            return View();
        }

        public ActionResult IndexCashE()
        {
            MyInvestEntities db = new MyInvestEntities();

            return View();
        }


        public ActionResult IndexIs()
        {
            MyInvestEntities db = new MyInvestEntities();

            return View();
        }


        public ActionResult IndexFund()
        {
            MyInvestEntities db = new MyInvestEntities();

            return View();
        }

        public JsonResult GetCashI()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = db.CashIncome.ToList().Select(c => new BSViewModel
            {
                UserName = c.UserName,
                InCashType = c.InCashType,
                InAmount = c.InAmount,
                InDate = c.InDate.ToShortDateString(),
                InNote = c.InNote
            });

            //dynamic retObject = new { data = ret.ToList() };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCashE()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = db.CashExpense.ToList().Select(c => new BSViewModel
            {
                UserName = c.UserName,
                ExCashType = c.ExCashType,
                ExAmount = c.ExAmount,
                ExDate = c.ExDate.ToShortDateString(),
                ExNote = c.ExNote
            });
            //dynamic retObject = new { data = ret.ToList() };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStock()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = from c in db.StockHistory
                      select new BSViewModel
                      {
                          UserName = c.SummaryTable.UserName,
                          stockID = c.stockID,
                          stockPrice = c.stockPrice,
                          stockAmount = c.stockAmount,
                          stockNote = c.stockNote
                      };
            dynamic retObject = new { data = ret.ToList() };
            return Json(retObject, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFX()
        {
            var db = new F1005.Models.MyInvestEntities();   
            var ret = db.FXtradeTable.ToList().Select(c => new BSViewModel     
                      {
                          UserName = c.SummaryTable.UserName,
                          CurrencyClass = c.CurrencyClass,                      
                          TradeDate = c.SummaryTable.TradeDate.ToShortDateString(),
                          TradeClass = c.TradeClass,
                          note = c.note
                      });
            //dynamic retObject = new { data = ret.ToList() };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetIs()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = db.Insurances.ToList().Select(c => new BSViewModel
            {
                UserName = c.SummaryTable.UserName,
                InsuranceName = c.InsuranceName,
                PurchaseDate = c.PurchaseDate.ToShortDateString(),
                WithdrawDate = c.WithdrawDate.ToShortDateString(),
                PaymentPerYear = c.PaymentPerYear,
                PayYear = c.PayYear,
                Withdrawal = c.Withdrawal
            });
            //dynamic retObject = new { data = ret.ToList() };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetFund()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = db.Fund.ToList().Select(c=> new BSViewModel
                      {
                          UserName = c.SummaryTable.UserName,
                          FundName = c.FundName,
                          Date = c.Date.ToShortDateString(),
                          NAV = c.NAV,
                          Units = c.Units
                      });
            //dynamic retObject = new { data = ret.ToList() };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Recent()
        {
            return View();
        }

        public ActionResult AllSum()
        {
            return View();
        }



        public ActionResult chartpieX()
        {
            var db = new F1005.Models.MyInvestEntities();
            var stockCount = db.StockHistory.Count().ToString();
            var FXCount = db.FXtradeTable.Count().ToString();
            var InsuranceCount = db.Insurances.Count().ToString();
            var FundCount = db.Fund.Count().ToString();

            var query = db.StockHistory.Select(c => new OverallViewModel
            {
                X = stockCount,
                XX = FXCount,
                XXX = InsuranceCount,
                XXXX = FundCount
            }).First();
            return Json(query,JsonRequestBehavior.AllowGet);
        }

        public ActionResult chartpieXX()
        {
            var db = new F1005.Models.MyInvestEntities();
            var stockCount = db.StockHistory.Select(c => c.stockNetincome * -1).Sum().ToString(); ;
            var FXCount = db.FXtradeTable.Count().ToString();
            var InsuranceCount = db.Insurances.Count().ToString();
            var FundCount = db.Fund.Count().ToString();

            var query = db.StockHistory.Select(c => new OverallViewModel
            {
                //123
                X = stockCount,
                XX = FXCount,
                XXX = InsuranceCount,
                XXXX = FundCount
            }).First();
            return Json(query, JsonRequestBehavior.AllowGet);
        }


    }
}