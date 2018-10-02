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
    public class FundsController : Controller
    {
        private MyInvestEntities db = new MyInvestEntities();

        // GET: Funds
        public ActionResult Index()
        {
            var UID = Session["User"].ToString();
            var query = db.Fund.Where(F => F.UserID == UID);
            foreach (var item in query)
            {
                Fund fund = db.Fund.Find(item.SerialNumber);
                string FundName = item.FundName;
                var CurrentNAVstr = db.fund_data.Where(D => D.FundName == FundName).Select(D => D.FundNAV).ToArray();
                double CurrentNAV = Convert.ToDouble(CurrentNAVstr[0]);
                fund.CurrentNAV = CurrentNAV;
            }
            db.SaveChanges();
            return View(db.Fund.ToList());
        }


            // GET: Funds/Details/5
            public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fund fund = db.Fund.Find(id);
            if (fund == null)
            {
                return HttpNotFound();
            }
            return View(fund);
        }

        // GET: Funds/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Funds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FundName,Fee,Units,Date,NAV")] Fund fund)
        {
         

            fund.BuyOrSell = true;
            fund.CashFlow = (fund.NAV * fund.Units) * (1 + fund.Fee / 100);
            SummaryTable ST = new SummaryTable { UserName = Session["User"].ToString(), TradeDate = fund.Date, TradeType = "基金" };
            db.SummaryTable.Add(ST);
            fund.UserID = Session["User"].ToString();
            fund.STID = db.SummaryTable.Select(s => s.STId).ToList().LastOrDefault();

            CashExpense CE = new CashExpense
            {
                UserName = fund.UserID,
                OID = fund.STID,
                ExCashType = "基金",
                ExDate = fund.Date,
                ExAmount = Convert.ToInt32(fund.CashFlow),
                ExNote = "基金申購"
            };
            db.CashExpense.Add(CE);
            if (ModelState.IsValid)
            {
                db.Fund.Add(fund);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fund);
        }

      

        public JsonResult GetCompanyList()
        {

            var data = db.fund_data.Where(D => D.Currancy == "TWD").Select(D => new    SelectListViewModel{
                CompanyName = D.CompanyName,
            }).Distinct().ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFundList(SelectListViewModel company)
        {
            string companyname = company.CompanyName;
            var data = db.fund_data.Where(D => D.Currancy == "TWD"&&D.CompanyName== companyname).Select(D => new SelectListViewModel
            {
                FundName = D.FundName,
            }).Distinct().ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Funds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fund fund = db.Fund.Find(id);
            if (fund == null)
            {
                return HttpNotFound();
            }
            return View(fund);
        }

        // POST: Funds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SerialNumber,UserID,FundName,BuyOrSell,Fee,Units,Date,NAV,CashFlow")] Fund fund)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fund).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fund);
        }

        // GET: Funds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fund fund = db.Fund.Find(id);
            if (fund == null)
            {
                return HttpNotFound();
            }
            return View(fund);
        }

        // POST: Funds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Fund fund = db.Fund.Find(id);
            db.Fund.Remove(fund);
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

        public ActionResult Sell(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fund fund = db.Fund.Find(id);
            if (fund == null)
            {
                return HttpNotFound();
            }
            return View(fund);
        }

        [HttpPost]
        public ActionResult Sell( Fund fund)
        {
           
            if (ModelState.IsValid)
            {
                Fund olddata = db.Fund.Find(fund.SerialNumber);
                olddata.Units = olddata.Units - fund.Units;
                db.SaveChanges();
                fund.BuyOrSell = false;
                fund.CashFlow = (fund.SellNAV * fund.Units) * (1 + fund.Fee / 100);
                SummaryTable ST = new SummaryTable { UserName = Session["User"].ToString(), TradeDate = fund.Date, TradeType = "基金" };
                db.SummaryTable.Add(ST);
                fund.SerialNumber = 0;
                fund.UserID = Session["User"].ToString();
                fund.STID = db.SummaryTable.Select(s => s.STId).ToList().LastOrDefault();
                db.Fund.Add(fund);

                CashIncome CI = new CashIncome
                {
                    UserName = fund.UserID,
                    InCashType = "基金",
                    OID = fund.STID,
                    InAmount = Convert.ToInt32(fund.CashFlow),
                    InDate = fund.Date,
                    InNote = "基金贖回"
                };
                db.CashIncome.Add(CI);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fund);
        }

        public JsonResult GetCurrentDoughnut()
        {
            string UID = Session["User"].ToString();
            var query = db.Fund.Where(F => F.UserID == UID && F.BuyOrSell == true && F.Units > 0).Select(F => new
            {
                Name = F.FundName,
                Money = F.Units*F.CurrentNAV
            }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSoldDoughnut()
        {
            string UID = Session["User"].ToString();
            var query = db.Fund.Where(F => F.UserID == UID && F.BuyOrSell == false && F.Units > 0).Select(F => new
            {
                Name = F.FundName,
                Money = F.Units * F.SellNAV
            }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult loadalldata()
        {
            string UID = Session["User"].ToString();
            var items = db.Fund.ToList().Where(I => I.UserID == UID).Select(I => new FundViewModel
            {
                SerialNumber = I.SerialNumber,
                STID = I.STID,
                UserID = I.UserID,
                FundName = I.FundName,
                BuyOrSell = I.BuyOrSell,
                Fee = I.Fee,
                Units = I.Units,
                Date = I.Date.ToShortDateString(),
                NAV = I.NAV,
                CashFlow = I.CashFlow,
                SellNAV = I.SellNAV,
                CurrentNAV = I.CurrentNAV,
            });
            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditIinsurance(Fund fund)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fund).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fund);
        }

        public ActionResult DeleteInsurances(int? id)
        {
            Fund obj = db.Fund.Find(id);
            var STdata = db.SummaryTable.Where(c => c.STId == obj.STID).Select(c => c).SingleOrDefault();
            db.SummaryTable.Remove(STdata);
            db.Fund.Remove(obj);

            if (obj.BuyOrSell == true)
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
