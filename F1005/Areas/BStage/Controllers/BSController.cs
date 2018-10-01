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
            //var ret = from c in db.CashIncome
            //          select new BSViewModel{
            //              UserName = c.SummaryTable.UserName,
            //              InCashType = c.InCashType,
            //              InAmount = c.InAmount,
            //              InDate = c.InDate,
            //              InNote = c.InNote                        
            //          };

            var ret = db.CashIncome.Select(c => new BSViewModel
            {
                UserName = c.SummaryTable.UserName,
                InCashType = c.InCashType,
                InAmount = c.InAmount,
                InDate = c.InDate,
                InNote = c.InNote
            });
                     

            dynamic retObject = new { data = ret.ToList() };
            return Json(retObject, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCashE()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = db.CashExpense.Select(c => new BSViewModel

            {
                          UserName = c.SummaryTable.UserName,
                          ExCashType = c.ExCashType,
                          ExAmount = c.ExAmount,
                          ExDate = c.ExDate,
                          ExNote = c.ExNote           
            });
            dynamic retObject = new { data = ret.ToList() };
            return Json(retObject, JsonRequestBehavior.AllowGet);
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
            var ret = from c in db.FXtradeTable
                      select new BSViewModel
                      {
                          UserName = c.SummaryTable.UserName,
                          CurrencyClass = c.CurrencyClass,
                          Tradetime = c.Tradetime,
                          TradeClass = c.TradeClass,
                          note = c.note
                      };
            dynamic retObject = new { data = ret.ToList() };
            return Json(retObject, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetIs()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = from c in db.Insurances
                      select new BSViewModel
                      {
                          UserName = c.SummaryTable.UserName,
                          InsuranceName = c.InsuranceName,
                          PurchaseDate = c.PurchaseDate,
                          WithdrawDate = c.WithdrawDate,
                          PaymentPerYear = c.PaymentPerYear,
                          PayYear = c.PayYear,
                          Withdrawal = c.Withdrawal
                      };
            dynamic retObject = new { data = ret.ToList() };
            return Json(retObject, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetFund()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = from c in db.Fund
                      select new BSViewModel
                      {
                          UserName = c.SummaryTable.UserName,
                          FundName = c.FundName,
                          Date = c.Date,
                          NAV = c.NAV,
                          Units = c.Units
                      };
            dynamic retObject = new { data = ret.ToList() };
            return Json(retObject, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Recent()
        {
            return View();
        }

        public JsonResult chartpie()
        {
            var db = new F1005.Models.MyInvestEntities();
            var x = Session["User"].ToString();
            var result = db.FXtradeTable.Where(c => c.SummaryTable.UserName == x).GroupBy(c => c.CurrencyClass, c => c.NTD, (CurrencyClass, NTD) => new
            {
                Currency = CurrencyClass,
                NTD = NTD.Sum()
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}