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

            CashIncome CI = new CashIncome
            {
                OID = id,
                UserName = insurances.UserID,
                InCashType = "保險",
                InAmount = insurances.CashFlow,
                InDate = insurances.WithdrawDate,
                InNote = insurances.InsuranceName+"解約金"
            };
            db.CashIncome.Add(CI);
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
                CE.ExNote = insurances.InsuranceName + "支出";

                db.CashExpense.Add(CE);
                db.SaveChanges();
                return RedirectToAction("Index");
              
            }

            return View(insurances);
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
            var stdata = db.SummaryTable.Find(insurance.STID);
            if (insurance.PurchaseOrWithdraw == true)
            {
                stdata.TradeDate = insurance.PurchaseDate;
                var arr = db.CashExpense.Where(C => C.OID == insurance.STID).Select(C => C).ToArray();
                arr[0].ExAmount = insurance.CashFlow;
                arr[0].ExDate = insurance.PurchaseDate;
            }
            else
            {
                stdata.TradeDate = insurance.WithdrawDate;
                var arr = db.CashIncome.Where(C => C.OID == insurance.STID).Select(C => C).ToArray();
                arr[0].InAmount = -insurance.CashFlow;
                arr[0].InDate = insurance.PurchaseDate;
            }
          

            return View(insurance);
        }

        //Delete 
        public ActionResult DeleteInsurances(int? id)
        {
            Insurances obj = db.Insurances.Find(id);
            var STdata = db.SummaryTable.ToList().Where(c => c.STId == obj.STID).Select(c => c).SingleOrDefault();

            db.Insurances.Remove(obj);
       

            if (obj.PurchaseOrWithdraw == true)
            {
                var arr = db.CashExpense.Where(C => C.OID == obj.STID).Select(C => C).ToArray();
                db.CashExpense.Remove(arr[0]);
                db.SummaryTable.Remove(STdata);
            }
            else
            {
                var arr = db.CashIncome.Where(C => C.OID == obj.STID).Select(C => C).ToArray();
                db.CashIncome.Remove(arr[0]);
                db.SummaryTable.Remove(STdata);
            }
            
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
