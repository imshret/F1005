using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using F1005.Models;

namespace F1005.Areas.ForeignExchange.Controllers
{
    public class CurrencyRatesController : Controller
    {
        private MyInvestEntities db = new MyInvestEntities();

        // GET: ForeignExchange/CurrencyRates
        public ActionResult Index()
        {
            return View(db.CurrencyRate.ToList());
        }

        // GET: ForeignExchange/CurrencyRates/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrencyRate currencyRate = db.CurrencyRate.Find(id);
            if (currencyRate == null)
            {
                return HttpNotFound();
            }
            return View(currencyRate);
        }

        // GET: ForeignExchange/CurrencyRates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ForeignExchange/CurrencyRates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CurrencyClass,CashBuy,CashSell,OnlineBuy,OnlineSell,Date")] CurrencyRate currencyRate)
        {
            if (ModelState.IsValid)
            {
                db.CurrencyRate.Add(currencyRate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(currencyRate);
        }

        // GET: ForeignExchange/CurrencyRates/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrencyRate currencyRate = db.CurrencyRate.Find(id);
            if (currencyRate == null)
            {
                return HttpNotFound();
            }
            return View(currencyRate);
        }

        // POST: ForeignExchange/CurrencyRates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CurrencyClass,CashBuy,CashSell,OnlineBuy,OnlineSell,Date")] CurrencyRate currencyRate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(currencyRate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(currencyRate);
        }

        // GET: ForeignExchange/CurrencyRates/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrencyRate currencyRate = db.CurrencyRate.Find(id);
            if (currencyRate == null)
            {
                return HttpNotFound();
            }
            return View(currencyRate);
        }

        // POST: ForeignExchange/CurrencyRates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            CurrencyRate currencyRate = db.CurrencyRate.Find(id);
            db.CurrencyRate.Remove(currencyRate);
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
