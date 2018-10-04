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
        [ValidateAntiForgeryToken]
        public ActionResult Withdraw(Insurances insurances)
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            if (ModelState.IsValid)
            {
                SummaryTable ST = new SummaryTable
                {
                    TradeType = "保險",
                    TradeDate = insurances.WithdrawDate,
                    UserName = Session["User"].ToString()
                };



                Insurances olddata = db.Insurances.Find(insurances.SerialNumber);
                olddata.Withdrawed = true;
                db.SaveChanges();

                insurances.PurchaseOrWithdraw = false;
                insurances.Withdrawed = true;

                insurances.UserID = Session["User"].ToString();
                if (insurances.RelateCash == true)
                {

                    insurances.CashFlow = insurances.Withdrawal;
                    db.SummaryTable.Add(ST);
                    db.SaveChanges();

                    var id = db.SummaryTable.Select(c => c.STId).ToList().LastOrDefault();
                    insurances.STID = id;
                    db.Insurances.Add(insurances);
                    db.SaveChanges();

                    var User = db.UsersData.Where(U => U.UserName == insurances.UserID).Select(U => U).SingleOrDefault();
                    User.InsuranceValue -= insurances.Withdrawal;
                    User.CashValue += insurances.Withdrawal;
                    db.SaveChanges();

                    CashIncome CI = new CashIncome
                    {
                        OID = id,
                        UserName = insurances.UserID,
                        InCashType = "保險",
                        InAmount = insurances.CashFlow,
                        InDate = insurances.WithdrawDate,
                        InNote = insurances.InsuranceName + "解約金"
                    };

                    db.CashIncome.Add(CI);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    insurances.CashFlow = 0;
                    db.SummaryTable.Add(ST);
                    db.SaveChanges();

                    var id = db.SummaryTable.Select(c => c.STId).ToList().LastOrDefault();
                    insurances.STID = id;
                    db.Insurances.Add(insurances);
                    db.SaveChanges();

                    var User = db.UsersData.Where(U => U.UserName == insurances.UserID).Select(U => U).SingleOrDefault();
                    User.InsuranceValue -= insurances.Withdrawal;
                    User.CashValue += insurances.Withdrawal;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(insurances);
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
            if (ModelState.IsValid)
            {
                insurances.UserID = Session["User"].ToString();
                insurances.PurchaseOrWithdraw = true;
                insurances.Withdrawed = false;

                SummaryTable ST = new SummaryTable
                {
                    TradeType = "保險",
                    TradeDate = insurances.PurchaseDate,
                    UserName = insurances.UserID
                };
                string UID = Session["User"].ToString();
                var ExSUM = db.CashExpense.Where(C => C.UserName == UID).Sum(C => C.ExAmount);
                var InSUM = db.CashIncome.Where(C => C.UserName == UID).Sum(C => C.InAmount);

                if (insurances.RelateCash == true&&InSUM-ExSUM> insurances.PaymentPerYear * insurances.PayYear)
                {
                    db.SummaryTable.Add(ST);
                    db.SaveChanges();

                    var id = db.SummaryTable.Select(c => c.STId).ToList().LastOrDefault();
                    insurances.STID = id;
                    insurances.CashFlow = -insurances.PaymentPerYear * insurances.PayYear;

                    db.Insurances.Add(insurances);
                    db.SaveChanges();

                    CashExpense CE = new CashExpense
                    {
                        ExAmount = -insurances.CashFlow,
                        UserName = insurances.UserID,
                        ExDate = ST.TradeDate,
                        ExCashType = ST.TradeType,
                        OID = id,
                        ExNote = insurances.InsuranceName + "支出"
                    };
                    db.CashExpense.Add(CE);

                    var User = db.UsersData.Where(U => U.UserName == insurances.UserID).Select(U => U).SingleOrDefault();
                    User.InsuranceValue += insurances.Withdrawal;
                    User.CashValue -= insurances.PaymentPerYear * insurances.PayYear;
                    db.SaveChanges();
                }
                else
                {
                    db.SummaryTable.Add(ST);
                    db.SaveChanges();

                    var id = db.SummaryTable.Select(c => c.STId).ToList().LastOrDefault();
                    insurances.STID = id;
                    insurances.CashFlow = 0;
                    db.Insurances.Add(insurances);
                    db.SaveChanges();

                    var User = db.UsersData.Where(U => U.UserName == insurances.UserID).Select(U => U).SingleOrDefault();
                    User.InsuranceValue += insurances.Withdrawal;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(insurances);
        }
        //Dispose
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
            var query = db.Insurances.Where(I => I.PurchaseOrWithdraw == true && I.UserID == UID && I.Withdrawed == false).Select(I => new

            {
                Name = I.InsuranceName,
                Money = I.PaymentPerYear * I.PayYear
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
            }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        //load data to 交易紀錄
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
                PaymentPerYear = I.PaymentPerYear.Value.ToString("C0"),
                PayYear = I.PayYear,
                PurchaseOrWithdraw = I.PurchaseOrWithdraw,
                Withdrawal = I.Withdrawal.Value.ToString("C0"),
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

                var stdata = db.SummaryTable.Find(insurance.STID);
                if (insurance.PurchaseOrWithdraw == true)
                {
                    stdata.TradeDate = insurance.PurchaseDate;
                    var arr = db.CashExpense.Where(C => C.OID == insurance.STID).Select(C => C).SingleOrDefault();
                    arr.ExAmount = insurance.CashFlow;
                    arr.ExDate = insurance.PurchaseDate;
                }
                else
                {
                    stdata.TradeDate = insurance.WithdrawDate;
                    var arr = db.CashIncome.Where(C => C.OID == insurance.STID).Select(C => C).SingleOrDefault();
                    arr.InAmount = -insurance.CashFlow;
                    arr.InDate = insurance.PurchaseDate;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insurance);
        }
        //Delete 
        public ActionResult DeleteInsurances(int? id)
        {
            Insurances obj = db.Insurances.Find(id);
            var STdata = db.SummaryTable.ToList().Where(c => c.STId == obj.STID).Select(c => c).SingleOrDefault();

            var STID = obj.STID;
            var RelateCash = obj.RelateCash;
            db.Insurances.Remove(obj);
            if (obj.PurchaseOrWithdraw == true)
            {
                if (RelateCash == true)
                {
                    var arr = db.CashExpense.Where(C => C.OID == STID).Select(C => C).SingleOrDefault();
                    db.CashExpense.Remove(arr);
                }
                if (RelateCash == false)
                {
                    db.SummaryTable.Remove(STdata);
                }
            }
            else
            {
                if (RelateCash == true)
                {
                    var arr = db.CashIncome.Where(C => C.OID == STID).Select(C => C).SingleOrDefault();
                    db.CashIncome.Remove(arr);
                }
                else
                {
                    db.SummaryTable.Remove(STdata);
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
