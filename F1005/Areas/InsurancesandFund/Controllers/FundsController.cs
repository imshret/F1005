using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using F1005.Models;


namespace F1005.Areas.InsurancesandFund.Controllers
{
    public class FundsController : Controller
    {
        private MyInvestEntities db = new MyInvestEntities();

        // GET: Funds
        public ActionResult Index()
        {
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
            SummaryTable ST = new SummaryTable { UserName = Session["User"].ToString(), TradeDate = fund.Date, TradeType = "基金" };
            db.SummaryTable.Add(ST);
            fund.UserID = Session["User"].ToString();
            fund.STID = db.SummaryTable.Select(s => s.STId).ToList().LastOrDefault();
            if (ModelState.IsValid)
            {
                db.Fund.Add(fund);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fund);
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
    }
}
