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
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

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
        // GET: Funds/Create
        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            return View();
        }
        // POST: Funds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FundName,Fee,Units,Date,NAV")] Fund fund)
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            if (ModelState.IsValid)
            {
                fund.BuyOrSell = true;
                fund.UserID = Session["User"].ToString();
                SummaryTable ST = new SummaryTable
                {
                    UserName = Session["User"].ToString(),
                    TradeDate = fund.Date,
                    TradeType = "基金"
                };
                string UID = Session["User"].ToString();
                var ExSUM = db.CashExpense.Where(C => C.UserName == UID).Sum(C => C.ExAmount);
                var InSUM = db.CashIncome.Where(C => C.UserName == UID).Sum(C => C.InAmount);
                if (fund.RelateCash == true&&InSUM-ExSUM> (fund.NAV * fund.Units) * (1 + fund.Fee / 100))
                {
                    db.SummaryTable.Add(ST);
                    db.SaveChanges();

                    fund.STID = db.SummaryTable.Select(s => s.STId).ToList().LastOrDefault();
                    fund.CashFlow = -(fund.NAV * fund.Units) * (1 + fund.Fee / 100);
                    db.Fund.Add(fund);
                    db.SaveChanges();

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
                    db.SaveChanges();

                    string value = db.fund_data.Where(D => D.FundName == fund.FundName).Select(D => D.FundNAV).SingleOrDefault();
                    var User = db.UsersData.Where(U => U.UserName == fund.UserID).Select(U => U).SingleOrDefault();
                    User.FundValue += fund.Units * double.Parse(value);
                    User.CashValue -= fund.Units * fund.NAV;
                    db.SaveChanges();
                }
                else
                {
                    db.SummaryTable.Add(ST);
                    db.SaveChanges();

                    fund.STID = db.SummaryTable.Select(s => s.STId).ToList().LastOrDefault();
                    fund.CashFlow = 0;
                    db.Fund.Add(fund);
                    db.SaveChanges();

                    string value = db.fund_data.Where(D => D.FundName == fund.FundName).Select(D => D.FundNAV).SingleOrDefault();
                    var User = db.UsersData.Where(U => U.UserName == fund.UserID).Select(U => U).SingleOrDefault();
                    User.FundValue += fund.Units * double.Parse(value);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(fund);
        }
        //GET:Funds/Sell
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
        //POST:Funds/Sell
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sell( Fund fund)
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            if (ModelState.IsValid)
            {
                Fund olddata = db.Fund.Find(fund.SerialNumber);
                olddata.Units = olddata.Units - fund.Units;
                db.SaveChanges();

                fund.BuyOrSell = false;

                SummaryTable ST = new SummaryTable
                {
                    UserName = Session["User"].ToString(),
                    TradeDate = fund.Date,
                    TradeType = "基金"
                };

                if (fund.RelateCash == true)
                {
                    fund.CashFlow = (fund.SellNAV * fund.Units) * (1 + fund.Fee / 100);
                    db.SummaryTable.Add(ST);
                    db.SaveChanges();

                    fund.SerialNumber = 0;
                    fund.UserID = Session["User"].ToString();
                    fund.STID = db.SummaryTable.Select(s => s.STId).ToList().LastOrDefault();
                    db.Fund.Add(fund);
                    db.SaveChanges();

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

                    string value = db.fund_data.Where(D => D.FundName == fund.FundName).Select(D => D.FundNAV).SingleOrDefault();
                    var User = db.UsersData.Where(U => U.UserName == fund.UserID).Select(U => U).SingleOrDefault();
                    User.FundValue -= fund.Units * double.Parse(value);
                    User.CashValue += fund.SellNAV * fund.Units;
                }
                else
                {
                    fund.CashFlow =0;
                    db.SummaryTable.Add(ST);
                    db.SaveChanges();

                    fund.SerialNumber = 0;
                    fund.UserID = Session["User"].ToString();
                    fund.STID = db.SummaryTable.Select(s => s.STId).ToList().LastOrDefault();
                    db.Fund.Add(fund);
                    db.SaveChanges();

                    string value = db.fund_data.Where(D => D.FundName == fund.FundName).Select(D => D.FundNAV).SingleOrDefault();
                    var User = db.UsersData.Where(U => U.UserName == fund.UserID).Select(U => U).SingleOrDefault();
                    User.FundValue -= fund.Units * double.Parse(value);
                }
                return RedirectToAction("Index");
            }
            return View(fund);
        }
        //取得基金公司名單
        public JsonResult GetCompanyList()
        {

            var data = db.fund_data.Where(D => D.Currancy == "TWD").Select(D => new SelectListViewModel
            {
                CompanyName = D.CompanyName,
            }).Distinct().ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //取得基金名單
        public JsonResult GetFundList(SelectListViewModel company)
        {
            string companyname = company.CompanyName;
            var data = db.fund_data.Where(D => D.Currancy == "TWD" && D.CompanyName == companyname).Select(D => new SelectListViewModel
            {
                FundName = D.FundName,
            }).Distinct().ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //取得基金資訊
        public JsonResult GetFundDetails(SelectListViewModel fund)
        {
            string fundname = fund.FundName;
            var data = db.fund_data.Where(D => D.FundName == fundname).Select(D => new
            {
                D.CompanyName,
                D.FundName,
                D.FundID,
                D.Currancy,
                D.FundNAV
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
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
        //畫未實現報酬圖表
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
        //畫已實現報酬圖表
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

        //生成交易紀錄
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
        //Edit
        public ActionResult EditIinsurance(Fund fund)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fund).State = EntityState.Modified;


                var stdata = db.SummaryTable.Find(fund.STID);
                if (fund.BuyOrSell == true)
                {
                    stdata.TradeDate = fund.Date;
                    var arr = db.CashExpense.Where(C => C.OID == fund.STID).Select(C => C).SingleOrDefault();
                    arr.ExAmount = Convert.ToInt32(fund.CashFlow);
                    arr.ExDate = fund.Date;
                }
                else
                {
                    stdata.TradeDate = fund.Date;
                    var arr = db.CashIncome.Where(C => C.OID == fund.STID).Select(C => C).SingleOrDefault();
                    arr.InAmount = -Convert.ToInt32(fund.CashFlow);
                    arr.InDate = fund.Date;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fund);
        }
        //Delete
        public ActionResult DeleteInsurances(int? id)
        {
            Fund obj = db.Fund.Find(id);
            var STdata = db.SummaryTable.Where(c => c.STId == obj.STID).Select(c => c).SingleOrDefault();

            var Relate = obj.RelateCash;
            var STID = obj.STID;


            db.Fund.Remove(obj);

            if (obj.BuyOrSell == true)
            {
                if (Relate == true)
                {
                    var arr = db.CashExpense.Where(C => C.OID == STID).Select(C => C).SingleOrDefault();
                    db.CashExpense.Remove(arr);

                }
                else
                {
                    db.SummaryTable.Remove(STdata);
                }
            }
            else
            {
                if (Relate == true)
                {
                    var arr = db.CashIncome.Where(C => C.OID == STID).Select(C => C).SingleOrDefault();
                    db.CashIncome.Remove(arr);
                    db.SummaryTable.Remove(STdata);
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
