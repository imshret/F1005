using F1005.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace F1005.Areas.Stock.Controllers
{
    public class HomeController : Controller
    {   
        public ActionResult Index()
        {      
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]

        public ActionResult Create([Bind(Include = "TradeType,date,userID")]SummaryTable summaryTable, [Bind(Include = "stockID,stockPrice,stockAmount,stockTP,stockFee,stockTax,stockNetincome,stockNote")]StockHistory stockHistory)
        // public ActionResult Create([Bind(Include = "TradeType,userID,date")]SummaryTable summaryTable)
        {
            MyInvestEntities db = new MyInvestEntities();
            if (ModelState.IsValid)
            {
                summaryTable.UserName = Session["User"].ToString();

                db.SummaryTable.Add(summaryTable);
                db.SaveChanges();

                var id = db.SummaryTable.Select(c => c.STId).ToList().LastOrDefault();
                stockHistory.STId = id;
                db.StockHistory.Add(stockHistory);
                db.SaveChanges();
                ViewBag.state = "ok";
                return RedirectToAction("Index");

            }            return RedirectToAction("Index");
        }

    }
}
