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
        //解約
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
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

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

        // 保險首頁
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            return View(db.Insurances.ToList());
        }

        
        //新增保單
        // GET: Insurances/Create
        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            return View();
        }

        // POST: Insurances/Create
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
        //編輯保單
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

        //計算IRR
        public ActionResult CalculateIRR(IRRViewModel model)
        {
            IRRCalculater calculater = new IRRCalculater();
            return Content(calculater.IRR(model));
        }

        //生成未實現圖表
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
        //生成已實現圖表
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

        public ActionResult loadalldata()
        {
            string UID = Session["User"].ToString();
            var items = db.Insurances.ToList().Where(I => I.UserID == UID).Select(I => new InsurancesViewModel
            {
                SerialNumber = I.SerialNumber,
                STID = I.STID,
                UserID = I.UserID,
                InsuranceName = I.InsuranceName,
                PurchaseDate = I.PurchaseDate.ToShortDateString(),
                WithdrawDate = I.WithdrawDate.ToShortDateString(),
                PaymentPerYear = I.PaymentPerYear,
                PayYear = I.PayYear,
                PurchaseOrWithdraw = I.PurchaseOrWithdraw,
                Withdrawal = I.Withdrawal,
                Withdrawed = I.Withdrawed,
                CashFlow = I.CashFlow,
            });
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        //Edit 
        public ActionResult EditIinsurance(Insurances insurance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insurance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insurance);
        }

        //Delete 
        public ActionResult DeleteInsurances(int? id)
        {
            Insurances obj = db.Insurances.Find(id);
            var STdata = db.SummaryTable.Where(c => c.STId == obj.STID).Select(c => c).SingleOrDefault();
            db.SummaryTable.Remove(STdata);
            db.Insurances.Remove(obj);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
