using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using F1005.Models;

namespace F1005.Areas.Cash.Controllers
{
    public class CashIncomesController : Controller
    {
        private MyInvestEntities db = new MyInvestEntities();

        // GET: Cash/CashIncomes
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }
            //var cashIncome = db.CashIncome.Include(c => c.SummaryTable);
            //return View(cashIncome.ToList());
            return View(db.CashIncome.ToList());
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
            var query = db.CashIncome.ToList().Where(c => c.UserName == username).OrderBy(c => c.InCashID).Select(c => new GetIncomeViewModel
            {
                InCashID = c.InCashID,
                UserName = c.UserName,
                InCashType = c.InCashType,
                InAmount = Convert.ToInt32(c.InAmount).ToString("c2"),
                InDate = c.InDate.ToShortDateString(),
                InNote = c.InNote
            });

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //Insert Income
        public ActionResult InsertIncome([Bind(Include = "STId,TradeType,TradeDate,UserName")] SummaryTable summaryTable, [Bind(Include = "InCashID,UserName,InCashType,InAmount,InDate,InNote")] CashIncome cashIncome)
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            if (ModelState.IsValid)
            {
                var username = Convert.ToString(Session["User"]);
                var Esum = db.CashExpense.Sum(c => c.ExAmount).HasValue? db.CashExpense.Sum(c => c.ExAmount):0;
                var Isum = (db.CashIncome.Sum(c => c.InAmount) + Convert.ToInt32(cashIncome.InAmount)).HasValue? db.CashIncome.Sum(c => c.InAmount) + Convert.ToInt32(cashIncome.InAmount):0;
                var net = Isum - Esum;
                //算出現金淨值,更新至userdata的cashvalue
                var userdata = db.UsersData.Where(c => c.UserName == username).Select(c => c).SingleOrDefault();
                userdata.CashValue = (double)net;
                db.Entry(userdata).State = EntityState.Modified;

                //新增總表資料
                summaryTable.TradeType = cashIncome.InCashType;
                summaryTable.TradeDate = cashIncome.InDate;
                summaryTable.UserName = cashIncome.UserName;
                db.SummaryTable.Add(summaryTable);
                db.SaveChanges();

                //新增現金資產
                cashIncome.OID = summaryTable.STId;
                db.CashIncome.Add(cashIncome);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cashIncome);
        }

        //Edit Income
        public ActionResult EditIncome([Bind(Include = "InCashID,UserName,InCashType,InAmount,InDate,InNote")] CashIncome cashIncome)
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }
            var username = Convert.ToString(Session["User"]);
            if (ModelState.IsValid)
            {

                //var CI = db.CashIncome.Where(c => c.UserName == username && c.InCashID == cashIncome.InCashID).Select(c => c).SingleOrDefault();
                //var stData = db.SummaryTable.Where(c => c.UserName == username && c.STId == CI.OID).Select(c => c).SingleOrDefault();
                //stData.TradeDate = cashIncome.InDate;
                //db.Entry(stData).State = EntityState.Modified;
                //var id = GetData(cashIncome.InCashID);
                //cashIncome.OID = id[0].OID;
                //cashIncome.InCashType = id[0].InCashType;
                //cashIncome.InDate = id[0].InDate;


                //更新userdata的cashvalue
                var Esum = db.CashExpense.Sum(c => c.ExAmount).HasValue ? db.CashExpense.Sum(c => c.ExAmount):0;
                var Isum = db.CashIncome.Sum(c => c.InAmount);
                var net = (decimal)Isum - (decimal)Esum;
                var userdata = db.UsersData.Where(c => c.UserName == username).Select(c => c).SingleOrDefault();
                userdata.CashValue = (double)net;
                db.Entry(userdata).State = EntityState.Modified;
                //更新收入表資料
                db.Entry(cashIncome).State = EntityState.Modified;    
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cashIncome);
        }

        public List<CashIncome> GetData(int id)
        {
            var username = Convert.ToString(Session["User"]);
            var CI = db.CashIncome.Where(c => c.UserName == username && c.InCashID == id).Select(c => c).ToList();
            return CI;
        }

        //Delete Income
        public ActionResult DeleteIncome(int? id)
        {
            CashIncome obj = db.CashIncome.Find(id);
            var stData = db.SummaryTable.Where(c => c.STId == obj.OID).Select(c => c).SingleOrDefault();
            //刪除總表資料
            db.SummaryTable.Remove(stData);
            //刪除收入表資料
            db.CashIncome.Remove(obj);

            //更新userdata的cashvalue
            var username = Convert.ToString(Session["User"]);
            var Esum = db.CashExpense.Sum(c => c.ExAmount).HasValue? db.CashExpense.Sum(c => c.ExAmount):0;
            var Isum = db.CashIncome.Sum(c => c.InAmount).HasValue? db.CashIncome.Sum(c => c.InAmount):0;
            var net = Isum - Esum;
            var userdata = db.UsersData.Where(c => c.UserName == username).Select(c => c).SingleOrDefault();
            userdata.CashValue = (double)net;
            db.Entry(userdata).State = EntityState.Modified;

            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //====================================

        //Get Income History
        public ActionResult GetIncomeHis()
        {
            var username = Convert.ToString(Session["User"]);
            var month = DateTime.Now.Month;
            var query = db.CashIncome.Where(c => c.UserName == username && c.InDate.Month == month).OrderBy(c => c.InCashID).ToList().Select(c => new IncomeHisViewModel
            {
                Amount = c.InAmount,
                MyDate = c.InDate.ToShortDateString()
            });
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //Get Income History by Month
        [HttpGet]
        public ActionResult GetIncomeHisByMonth(int? year, int? month)
        {
            var username = Convert.ToString(Session["User"]);
            var query = db.CashIncome.Where(c => c.UserName == username && c.InDate.Year == year && c.InDate.Month == month).OrderBy(c => c.InCashID).ToList().Select(c => new IncomeHisViewModel
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
            var query = Convert.ToInt32(db.CashIncome.ToList().Where(c => c.UserName == username).Sum(c => c.InAmount)).ToString("c2");
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //支出餘額
        public ActionResult GetExpenseBalance()
        {
            var username = Convert.ToString(Session["User"]);
            var query = Convert.ToInt32(db.CashExpense.ToList().Where(c => c.UserName == username).Sum(c => c.ExAmount)).ToString("c2");
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //收支差額
        public ActionResult GetDiff()
        {
            var username = Convert.ToString(Session["User"]);
            var Income = Convert.ToInt32(db.CashIncome.ToList().Where(c => c.UserName == username).Sum(c => c.InAmount));
            var Expense = Convert.ToInt32(db.CashExpense.ToList().Where(c => c.UserName == username).Sum(c => c.ExAmount));
            var query = ((decimal)Income - (decimal)Expense).ToString("c2");
            return Json(query, JsonRequestBehavior.AllowGet);
        }
    }
}
