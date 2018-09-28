using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using F1005.Models;

namespace F1005.Areas.ForeignExchange.Controllers
{
    public class TradeTablesController : Controller
    {
        private MyInvestEntities db = new MyInvestEntities();
        // GET: TradeTables
        public ActionResult Index()
        {
            return View();
        }
        //public ActionResult Index()
        //{
        //    return RedirectToAction("About","Home");
        //}

        // GET: TradeTables/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FXtradeTable tradeTable = await db.FXtradeTable.FindAsync(id);
            if (tradeTable == null)
            {
                return HttpNotFound();
            }
            return View(tradeTable);
        }

        // GET: TradeTables/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TradeTables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create( FXtradeTable tradeTable, SummaryTable summaryTable)
        {
            if (ModelState.IsValid)
            {
                //var x = Session["User"].ToString();
                //var x = "5566";
                //summaryTable.UserName = x; 
                db.SummaryTable.Add(summaryTable);
                db.SaveChanges();

                //summaryTable.UserName = x;
                var uid = db.SummaryTable.Select(c => c.STId).ToList().LastOrDefault();
                tradeTable.SummaryId = uid;
                db.FXtradeTable.Add(tradeTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tradeTable);
        }

        // GET: TradeTables/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FXtradeTable tradeTable = await db.FXtradeTable.FindAsync(id);
            if (tradeTable == null)
            {
                return HttpNotFound();
            }
            return View(tradeTable);
        }

        // POST: TradeTables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserName,MClass,NTD,USD,FX,tax,Ttime,TClass")] FXtradeTable tradeTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tradeTable).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tradeTable);
        }

        // GET: TradeTables/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FXtradeTable tradeTable = await db.FXtradeTable.FindAsync(id);
            if (tradeTable == null)
            {
                return HttpNotFound();
            }
            return View(tradeTable);
        }

        // POST: TradeTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            FXtradeTable tradeTable = await db.FXtradeTable.FindAsync(id);
            db.FXtradeTable.Remove(tradeTable);
            await db.SaveChangesAsync();
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
