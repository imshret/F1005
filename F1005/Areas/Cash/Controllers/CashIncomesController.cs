using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using F1005.Models;

namespace F1005.Areas.Cash.Controllers
{
    public class CashIncomesController : Controller
    {
        private MyInvestEntities db = new MyInvestEntities();

        // GET: Cash/CashIncomes
        public ActionResult Index()
        {
            if(Session["User"]==null)
            {
                Session["User"] = "msit119_one";
            }
            //var cashIncome = db.CashIncome.Include(c => c.SummaryTable);
            //return View(cashIncome.ToList());
            return View();
        }

        // GET: Cash/CashIncomes/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CashIncome cashIncome = db.CashIncome.Find(id);
        //    if (cashIncome == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(cashIncome);
        //}

        //// GET: Cash/CashIncomes/Create
        //public ActionResult Create()
        //{
        //    ViewBag.OID = new SelectList(db.SummaryTable, "STId", "TradeType");
        //    return View();
        //}

        //// POST: Cash/CashIncomes/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "InCashID,OID,UserName,InCashType,InAmount,InDate,InNote")] CashIncome cashIncome)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.CashIncome.Add(cashIncome);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.OID = new SelectList(db.SummaryTable, "STId", "TradeType", cashIncome.OID);
        //    return View(cashIncome);
        //}

        //// GET: Cash/CashIncomes/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CashIncome cashIncome = db.CashIncome.Find(id);
        //    if (cashIncome == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.OID = new SelectList(db.SummaryTable, "STId", "TradeType", cashIncome.OID);
        //    return View(cashIncome);
        //}

        //// POST: Cash/CashIncomes/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "InCashID,OID,UserName,InCashType,InAmount,InDate,InNote")] CashIncome cashIncome)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(cashIncome).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.OID = new SelectList(db.SummaryTable, "STId", "TradeType", cashIncome.OID);
        //    return View(cashIncome);
        //}

        //// GET: Cash/CashIncomes/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CashIncome cashIncome = db.CashIncome.Find(id);
        //    if (cashIncome == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(cashIncome);
        //}

        //// POST: Cash/CashIncomes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    CashIncome cashIncome = db.CashIncome.Find(id);
        //    db.CashIncome.Remove(cashIncome);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //=======================================
        //Load Income Table
        public ActionResult GetAllIncome()
        {
            var username = Convert.ToString(Session["User"]);
            var query = db.CashIncome.ToList().Where(c => c.UserName == username).Select(c => new GetIncomeViewModel
            {
                InCashID = c.InCashID,
                UserName = c.UserName,
                InCashType = IncomeType.現金,
                InAmount = c.InAmount,
                InDate = c.InDate.ToShortDateString(),
                InNote = c.InNote
            });
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //Insert Income
        public ActionResult InsertIncome([Bind(Include = "STId,TradeType,TradeDate,UserName")] SummaryTable summaryTable, [Bind(Include = "InCashID,UserName,InCashType,InAmount,InDate,InNote")] CashIncome cashIncome)
        {
            summaryTable.TradeType = cashIncome.InCashType;
            summaryTable.TradeDate = cashIncome.InDate;
            summaryTable.UserName = cashIncome.UserName;
            db.SummaryTable.Add(summaryTable);
            db.SaveChanges();

            cashIncome.OID = summaryTable.STId;
            if (ModelState.IsValid)
            {
                db.CashIncome.Add(cashIncome);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cashIncome);
        }

        //Edit Income
        public ActionResult EditIncome([Bind(Include = "InCashID,UserName,InCashType,InAmount,InDate,InNote")] CashIncome cashIncome)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cashIncome).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cashIncome);
        }

        //Delete Income
        public ActionResult DeleteIncome(int? id)
        {

            CashIncome obj = db.CashIncome.Find(id);
            var username = db.SummaryTable.Where(c => c.STId == obj.OID).Select(c => c).SingleOrDefault();
            db.SummaryTable.Remove(username);
            db.CashIncome.Remove(obj);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //====================================

        //Get Income History
        public ActionResult GetIncomeHis()
        {
            var username = Convert.ToString(Session["User"]);
            var query = db.CashIncome.Where(c => c.UserName == username).OrderBy(c => c.InDate).ToList().Select(c => new IncomeHisViewModel
            {
                Amount = c.InAmount,
                MyDate = c.InDate.ToShortDateString()
            });
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //Get Income History by Month
        [HttpGet]
        public ActionResult GetIncomeHisByMonth(int? month)
        {
            var username = Convert.ToString(Session["User"]);
            var query = db.CashIncome.Where(c => c.UserName == username && c.InDate.Month== month).OrderBy(c=>c.InDate).ToList().Select(c => new IncomeHisViewModel
            {
                Amount = c.InAmount,
                MyDate = c.InDate.ToShortDateString()
            });
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //支出收入百分比
        public ActionResult GetPie()
        {
            var username = Convert.ToString(Session["User"]);
            var ISum = (decimal)db.CashIncome.Where(c => c.UserName == username).Select(c => c.InAmount).DefaultIfEmpty(0).Sum();
            var ESum = (decimal)db.CashExpense.Where(c => c.UserName == username).Select(c => c.ExAmount).DefaultIfEmpty(0).Sum();

            var total = ISum + ESum;
            if (ISum == 0)
            {
                var query = db.CashExpense.ToList().Select(c => new
                {
                    IncomeP = (ISum / total * 100).ToString("f2"),
                    ExpenseP = (ESum / total * 100).ToString("f2")
                }).FirstOrDefault();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else if (ESum == 0)
            {
                var query = db.CashIncome.ToList().Select(c => new
                {
                    IncomeP = (ISum / total * 100).ToString("f2"),
                    ExpenseP = (ESum / total * 100).ToString("f2")
                }).FirstOrDefault();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var query = db.CashIncome.ToList().Select(c => new
                {
                    IncomeP = (ISum / total * 100).ToString("f2"),
                    ExpenseP = (ESum / total * 100).ToString("f2")
                }).FirstOrDefault();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
        }

        //收入餘額
        public ActionResult GetIncomeBalance()
        {
            var username = Convert.ToString(Session["User"]);
            var query = db.CashIncome.ToList().Where(c => c.UserName == username).Sum(c => c.InAmount);
            return Json(query, JsonRequestBehavior.AllowGet);
        }


    }
}
