using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using F1005.Areas.InsurancesandFund.Models;
using F1005.Models;


namespace F1005.Areas.InsurancesandFund.Controllers
{
    public class InsurancesController : Controller
    {
        private MyInvestEntities db = new MyInvestEntities();

        public ActionResult Withdraw(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurances insurances = db.Insurances.Find(id);
            if (insurances == null)
            {
                return HttpNotFound();
            }
            return View(insurances);
        }

        [HttpPost]
        public ActionResult Withdraw(Insurances insurances)
        {
            Insurances olddata = db.Insurances.Find(insurances.SerialNumber);
            olddata.Withdrawed = true;
            db.SaveChanges();

            insurances.CashFlow = insurances.Withdrawal;
            insurances.PurchaseOrWithdraw = false;
            insurances.Withdrawed = true;
            insurances.RelateCash = true;
            insurances.UserID = Session["User"].ToString();
            SummaryTable ST = new SummaryTable { TradeType = "保險", TradeDate = insurances.WithdrawDate, UserName = insurances.UserID };
            db.SummaryTable.Add(ST);
            db.SaveChanges();
            var id = db.SummaryTable.Select(c => c.STId).ToList().LastOrDefault();
            insurances.STID = id;
            db.Insurances.Add(insurances);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: Insurances
        public ActionResult Index()
        {
            return View(db.Insurances.ToList());
        }

        public JsonResult GetData()
        {
            var insurances = db.Insurances.ToList();

            //組成jqGrid要讀取的資料
            var jsonData = new
            {
                rows = insurances   //jqGrid呈現在表格中的資料
            };

            //回傳
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // GET: Insurances/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurances insurances = db.Insurances.Find(id);
            if (insurances == null)
            {
                return HttpNotFound();
            }
            return View(insurances);
        }

        // GET: Insurances/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insurances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SerialNumber,UserID,InsuranceName,PurchaseDate,WithdrawDate,PaymentPerYear,PayYear,PurchaseOrWithdraw,CashFlow,Withdrawal,RelateCash")] Insurances insurances)
        {
            if (insurances.RelateCash == true)
            {
                insurances.CashFlow = -insurances.PaymentPerYear * insurances.PayYear;
            }
            else
            {
                insurances.CashFlow = 0;
            }
            insurances.PurchaseOrWithdraw = true;
            insurances.Withdrawed = false;

            //insurances.UserID = Session["User"].ToString();
            if (ModelState.IsValid)
            {
                insurances.UserID = Session["User"].ToString();
                SummaryTable ST = new SummaryTable { TradeType = "保險", TradeDate = insurances.PurchaseDate, UserName = insurances.UserID };
                db.SummaryTable.Add(ST);
                db.SaveChanges();
                var id = db.SummaryTable.Select(c => c.STId).ToList().LastOrDefault();
                insurances.STID = id;
                db.Insurances.Add(insurances);
                db.SaveChanges();

                CashExpense CE = new CashExpense();
                CE.ExAmount = -insurances.CashFlow;
                CE.UserName = insurances.UserID;
                CE.ExDate = ST.TradeDate;
                CE.ExCashType = ST.TradeType;
                CE.OID = id;

                db.CashExpense.Add(CE);
                db.SaveChanges();
                return RedirectToAction("Index");
              
            }

            return View(insurances);
        }

        // GET: Insurances/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurances insurances = db.Insurances.Find(id);
            if (insurances == null)
            {
                return HttpNotFound();
            }
            return View(insurances);
        }

        // POST: Insurances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SerialNumber,UserID,InsuranceName,PurchaseDate,WithdrawDate,PaymentPerYear,PayYear,PurchaseOrWithdraw,CashFlow,Withdrawal")] Insurances insurances)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insurances).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insurances);
        }

        // GET: Insurances/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurances insurances = db.Insurances.Find(id);
            if (insurances == null)
            {
                return HttpNotFound();
            }
            return View(insurances);
        }

        // POST: Insurances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insurances insurances = db.Insurances.Find(id);
            db.Insurances.Remove(insurances);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult CalculateIRR(IRRViewModel model)
        {
            IRRCalculater calculater = new IRRCalculater();
            return Content(calculater.IRR(model));
        }

        public JsonResult GetCurrentDoughnut()
        {
            string UID = Session["User"].ToString();
            var query = db.Insurances.Where(I => I.PurchaseOrWithdraw == true && I.UserID == UID&&I.Withdrawed==false).Select(I => new

            {
                Name = I.InsuranceName,
                Money = I.PaymentPerYear*I.PayYear
            }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWithdrawedDoughnut()
        {
            string UID = Session["User"].ToString();
            var query = db.Insurances.Where(I => I.PurchaseOrWithdraw == false && I.UserID == UID).Select(I => new

            {
                Name = I.InsuranceName,
                Money = I.Withdrawal
            } ).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
    }
}
