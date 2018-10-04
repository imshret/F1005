using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            return View(db.CashExpense.ToList());
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
            var query = db.CashExpense.ToList().Where(c => c.UserName == username).OrderBy(c=>c.ExCashID).Select(c => new GetExpenseViewModel
            {
                ExCashID = c.ExCashID,
                UserName = c.UserName,
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
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            if (ModelState.IsValid)
            {
                var username = Convert.ToString(Session["User"]);
                var Esum = (db.CashExpense.Sum(c => c.ExAmount) + Convert.ToInt32(cashExpense.ExAmount)).HasValue? db.CashExpense.Sum(c => c.ExAmount) + Convert.ToInt32(cashExpense.ExAmount):0;
                var Isum = db.CashIncome.Sum(c => c.InAmount).HasValue? db.CashIncome.Sum(c => c.InAmount):0;
                var net = Isum - Esum;
                //如果淨資產小於零,不給新增支出項目
                if (net - cashExpense.ExAmount < 0)
                {
                    return Json(net - cashExpense.ExAmount, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //更新userdata的cashvalue
                    var userdata = db.UsersData.Where(c => c.UserName == username).Select(c => c).SingleOrDefault();
                    userdata.CashValue = (double)net;
                    db.Entry(userdata).State = EntityState.Modified;
                    //新增總表資料
                    summaryTable.TradeType = cashExpense.ExCashType;
                    summaryTable.TradeDate = cashExpense.ExDate;
                    summaryTable.UserName = cashExpense.UserName;
                    db.SummaryTable.Add(summaryTable);
                    //新增支出表資料
                    cashExpense.OID = summaryTable.STId;
                    db.CashExpense.Add(cashExpense);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception er)
                    {
                        throw er;
                    }
                    return RedirectToAction("Index");
                }
            }
             return View(cashExpense);
        }

        //Edit Expense
        public ActionResult EditExpense([Bind(Include = "ExCashID,UserName,ExCashType,ExAmount,ExDate,ExNote")] CashExpense cashExpense)
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }
            var username = Convert.ToString(Session["User"]);
            if (ModelState.IsValid)
            {
                //更新支出表資料
                db.Entry(cashExpense).State = EntityState.Modified;
                //更新userdata的cashvalue
                var Esum = (db.CashExpense.Sum(c => c.ExAmount) + Convert.ToInt32(cashExpense.ExAmount)).HasValue ? db.CashExpense.Sum(c => c.ExAmount) + Convert.ToInt32(cashExpense.ExAmount) : 0;
                var Isum = db.CashIncome.Sum(c => c.InAmount).HasValue ? db.CashIncome.Sum(c => c.InAmount) : 0;
                var net = Isum - Esum;
                var userdata = db.UsersData.Where(c => c.UserName == username).Select(c => c).SingleOrDefault();
                userdata.CashValue = (double)net;
                db.Entry(userdata).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cashExpense);
        }

        //Delete Expense
        public ActionResult DeleteExpense(int? id)
        {
            CashExpense obj = db.CashExpense.Find(id);
            var stData = db.SummaryTable.Where(c => c.STId == obj.OID).Select(c => c).SingleOrDefault();
            //刪除總表資料
            db.SummaryTable.Remove(stData);
            //刪除收入表資料
            db.CashExpense.Remove(obj);

            //更新userdata的cashvalue
            var username = Convert.ToString(Session["User"]);
            var Esum = db.CashExpense.Sum(c => c.ExAmount).HasValue? db.CashExpense.Sum(c => c.ExAmount):0;
            var Isum = db.CashIncome.Sum(c => c.InAmount).HasValue ? db.CashIncome.Sum(c => c.InAmount):0;
            var net = Isum - Esum;
            var userdata = db.UsersData.Where(c => c.UserName == username).Select(c => c).SingleOrDefault();
            userdata.CashValue = (double)net;
            db.Entry(userdata).State = EntityState.Modified;
            
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //==========================================

        //Draw Expense History
        public ActionResult GetExpenseHis()
        {
            var username = Convert.ToString(Session["User"]);
            var month = DateTime.Now.Month;
            var query = db.CashExpense.ToList().Where(c => c.UserName == username && c.ExDate.Month==month).OrderBy(c=>c.ExCashID).Select(c => new ExpenseHisViewModel
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
            var query = db.CashExpense.Where(c => c.UserName == username && c.ExDate.Year==year && c.ExDate.Month == month).OrderBy(c => c.ExCashID).ToList().Select(c => new ExpenseHisViewModel
            {
                Amount = c.ExAmount,
                MyDate = c.ExDate.ToShortDateString()
            });
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //收入餘額
        public ActionResult GetIncomeBalance()
        {
            var username = Convert.ToString(Session["User"]);
            var query = Convert.ToInt32(db.CashIncome.ToList().ToList().Where(c => c.UserName == username).Sum(c => c.InAmount)).ToString("c2");
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //支出餘額
        public ActionResult GetExpenseBalance()
        {
            var username = Convert.ToString(Session["User"]);
            var query = Convert.ToInt32(db.CashExpense.ToList().ToList().Where(c => c.UserName == username).Sum(c => c.ExAmount)).ToString("c2");
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //收支差額
        public ActionResult GetDiff()
        {
            var username = Convert.ToString(Session["User"]);
            var Income = Convert.ToInt32(db.CashIncome.ToList().ToList().Where(c => c.UserName == username).Sum(c => c.InAmount));
            var Expense = Convert.ToInt32(db.CashExpense.ToList().ToList().Where(c => c.UserName == username).Sum(c => c.ExAmount));
            var query = ((decimal)Income - (decimal)Expense).ToString("c2");
            return Json(query, JsonRequestBehavior.AllowGet);
        }
    }
}
