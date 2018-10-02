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
    public class CashExpensesController : Controller
    {
        private MyInvestEntities db = new MyInvestEntities();

        // GET: Cash/CashExpenses
        public ActionResult Index()
        {
            //var cashExpense = db.CashExpense.Include(c => c.SummaryTable);
            //return View(cashExpense.ToList());

            if(Session["User"]==null)
            {
                Session["User"] = "msit119_one";
            }
            return View();
        }

        //// GET: Cash/CashExpenses/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CashExpense cashExpense = db.CashExpense.Find(id);
        //    if (cashExpense == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(cashExpense);
        //}

        //// GET: Cash/CashExpenses/Create
        //public ActionResult Create()
        //{
        //    ViewBag.OID = new SelectList(db.SummaryTable, "STId", "TradeType");
        //    return View();
        //}

        //// POST: Cash/CashExpenses/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ExCashID,OID,UserName,ExCashType,ExAmount,ExDate,ExNote")] CashExpense cashExpense)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.CashExpense.Add(cashExpense);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.OID = new SelectList(db.SummaryTable, "STId", "TradeType", cashExpense.OID);
        //    return View(cashExpense);
        //}

        //// GET: Cash/CashExpenses/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CashExpense cashExpense = db.CashExpense.Find(id);
        //    if (cashExpense == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.OID = new SelectList(db.SummaryTable, "STId", "TradeType", cashExpense.OID);
        //    return View(cashExpense);
        //}

        //// POST: Cash/CashExpenses/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ExCashID,OID,UserName,ExCashType,ExAmount,ExDate,ExNote")] CashExpense cashExpense)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(cashExpense).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.OID = new SelectList(db.SummaryTable, "STId", "TradeType", cashExpense.OID);
        //    return View(cashExpense);
        //}

        //// GET: Cash/CashExpenses/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CashExpense cashExpense = db.CashExpense.Find(id);
        //    if (cashExpense == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(cashExpense);
        //}

        //// POST: Cash/CashExpenses/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    CashExpense cashExpense = db.CashExpense.Find(id);
        //    db.CashExpense.Remove(cashExpense);
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

        //==============================

        //Load Expense Table
        public ActionResult GetAllExpense()
        {
            var username = Convert.ToString(Session["User"]);
            var query = db.CashExpense.ToList().Where(c => c.UserName == username).OrderBy(c=>c.ExDate).Select(c => new GetExpenseViewModel
            {
                ExCashID = c.ExCashID,
                UserID = c.UserName,
                ExCashType = c.ExCashType,
                ExAmount = Convert.ToInt32(c.ExAmount).ToString("c2"),
                ExDate = c.ExDate.ToShortDateString(),
                ExNote = c.ExNote
            });
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //Insert Expense
        public ActionResult InsertExpense([Bind(Include = "STId,TradeType,TradeDate,UserName")] SummaryTable summaryTable, [Bind(Include = "ExCashID,UserName,ExCashType,ExAmount,ExDate,ExNote")] CashExpense cashExpense)
        {
            summaryTable.TradeType = cashExpense.ExCashType;
            summaryTable.TradeDate = cashExpense.ExDate;
            summaryTable.UserName = cashExpense.UserName;
            db.SummaryTable.Add(summaryTable);
            if (ModelState.IsValid)
            {
                db.CashExpense.Add(cashExpense);
                try
                {
                    db.SaveChanges();
                }
                catch(Exception er)
                {
                    throw er;
                }
                return RedirectToAction("Index");
            }

            return View(cashExpense);
        }

        //Edit Expense
        public ActionResult EditExpense([Bind(Include = "ExCashID,UserID,ExCashType,ExAmount,ExDate,ExNote")] CashExpense cashExpense)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cashExpense).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cashExpense);
        }

        //Delete Expense
        public ActionResult DeleteExpense(int? id)
        {
            CashExpense obj = db.CashExpense.Find(id);
            var username = db.SummaryTable.Where(c => c.STId == obj.OID).Select(c => c).SingleOrDefault();
            db.SummaryTable.Remove(username);
            db.CashExpense.Remove(obj);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //==========================================

        //Draw Expense History
        public ActionResult GetExpenseHis()
        {
            var username = Convert.ToString(Session["User"]);
            var month = DateTime.Now.Month;
            var query = db.CashExpense.ToList().Where(c => c.UserName == username && c.ExDate.Month==month).OrderBy(c=>c.ExDate).Select(c => new ExpenseHisViewModel
            {
                Amount = c.ExAmount,
                MyDate = c.ExDate.ToShortDateString()
            });
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //Get Income History by Month
        [HttpGet]
        public ActionResult GetExpenseHisByMonth(int? year, int? month)
        {
            var username = Convert.ToString(Session["User"]);
            var query = db.CashExpense.Where(c => c.UserName == username && c.ExDate.Year==year && c.ExDate.Month == month).OrderBy(c => c.ExDate).ToList().Select(c => new ExpenseHisViewModel
            {
                Amount = c.ExAmount,
                MyDate = c.ExDate.ToShortDateString()
            });
            return Json(query, JsonRequestBehavior.AllowGet);
        }


        //支出餘額
        public ActionResult GetExpenseBalance()
        {
            var username = Convert.ToString(Session["User"]);
            var query = Convert.ToInt32(db.CashExpense.ToList().ToList().Where(c => c.UserName == username).Sum(c => c.ExAmount)).ToString("c2");
            return Json(query, JsonRequestBehavior.AllowGet);
        }
    }
}
