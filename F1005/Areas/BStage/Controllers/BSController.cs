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
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            return View();
        }

        public ActionResult IndexX()
        {
            Session["User"] = "msit119";           

            return View();
        }


        public ActionResult IndexStock()        {  

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
                          stockNote = c.stockNote,
                          stockNetincome = c.stockNetincome
                      };
            dynamic retObject = new { data = ret.ToList() };
            return Json(retObject, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFX()
        {
            var db = new F1005.Models.MyInvestEntities();   
            var ret = db.FXtradeTable.ToList().Where(c=>c.TradeClass =="買入").Select(c => new BSViewModel     
                      {
                          UserName = c.SummaryTable.UserName,
                          CurrencyClass = c.CurrencyClass,                      
                          TradeDate = c.SummaryTable.TradeDate.ToShortDateString(),        
                          NTD = c.NTD,
                          note = c.note
                      });
            //dynamic retObject = new { data = ret.ToList() };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetIs()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = db.Insurances.ToList().Where(c=>c.CashFlow < 0).Select(c => new BSViewModel
            {
                UserName = c.SummaryTable.UserName,
                InsuranceName = c.InsuranceName,
                PurchaseDate = c.PurchaseDate.ToShortDateString(),
                WithdrawDate = c.WithdrawDate.ToShortDateString(),
                PaymentPerYear = c.PaymentPerYear,
                PayYear = c.PayYear,
                CashFlow = c.CashFlow *-1,
                Withdrawal = c.Withdrawal
            });
            //dynamic retObject = new { data = ret.ToList() };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFund()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = db.Fund.ToList().Where(c=>c.CashFlow>0).Select(c=> new BSViewModel
                      {
                          UserName = c.SummaryTable.UserName,
                          FundName = c.FundName,
                          Date = c.Date.ToShortDateString(),
                          NAV = c.NAV,
                          Units = c.Units,
                          CashFlowX = c.CashFlow,
             
    });
            //dynamic retObject = new { data = ret.ToList() };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        //會員清單
        public ActionResult Users()
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            return View();
        }


        //會員清單
        public JsonResult GetUsers()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = db.UsersData.ToList().Where(c => c.UserName != "msit119").Select(c => new BSViewModel {
                UserName = c.UserName,
                Email = c.Email,
            });
            //dynamic retObject = new { data = ret.ToList() };
            return Json(ret, JsonRequestBehavior.AllowGet);
        } 



        //後台NAV
        public ActionResult Recent()
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            return View();
        }

        public ActionResult AllSum()
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            return View();
        }

        //後台NAV繪圖
        public ActionResult chartpieX()
        {
            var db = new F1005.Models.MyInvestEntities();

            var cashCount = db.CashIncome.Count().ToString();
            var stockCount = db.StockHistory.Count().ToString();
            var FXCount = db.FXtradeTable.Count().ToString();
            var InsuranceCount = db.Insurances.Count().ToString();
            var FundCount = db.Fund.Count().ToString();

            var query = db.UsersData.Select(c => new OverallViewModel
            {
                cash = cashCount,
                stock = stockCount,
                FX = FXCount,
                Insurance = InsuranceCount,
                fund = FundCount

            }).First();
            return Json(query,JsonRequestBehavior.AllowGet);
        }

        public ActionResult chartpieXX()
        {
            var db = new F1005.Models.MyInvestEntities();
            var cashCount = db.UsersData.Select(c => c.CashValue).Sum().ToString();
            var stockCount = db.UsersData.Select(c => c.StockValue).Sum().ToString();
            var FXCount = db.UsersData.Select(c => c.FXValue).Sum().ToString();
            var InsuranceCount = db.UsersData.Select(c => c.InsuranceValue).Sum().ToString();
            var FundCount = db.UsersData.Select(c => c.FundValue).Sum().ToString();
            //var total = cashCount + stockCount + FXCount + InsuranceCount + FundCount;

            //var stockCount = db.StockHistory.Select(c => c.stockNetincome * -1).Sum().ToString(); ;
            //var FXCount = db.FXtradeTable.Where(c=>c.TradeClass == "買入").Select(c=>c.NTD).Sum().ToString();
            //var InsuranceCount = db.Insurances.Where(c => c.PurchaseOrWithdraw == true).Select(c => c.CashFlow * -1).Sum().ToString();
            //var FundCount = db.Fund.Where(c => c.BuyOrSell == true).Select(c => c.CashFlow).Sum().ToString();

            var query = db.UsersData.Select(c => new OverallViewModel
            {               
                cash= cashCount,
                stock = stockCount,
                FX = FXCount,
                Insurance= InsuranceCount,
                fund = FundCount
            }).First();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
    }
}