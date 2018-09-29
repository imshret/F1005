using F1005.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

            return View(db.CashExpense.ToList());
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

    }
}