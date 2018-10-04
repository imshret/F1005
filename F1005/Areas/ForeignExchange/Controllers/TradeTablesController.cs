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
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            return View();
        }


        // GET: TradeTables/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

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
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

            return View();
        }

        // POST: TradeTables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create( FXtradeTable tradeTable, SummaryTable summaryTable,UsersData usersData)
        {
            if (ModelState.IsValid)
            {
                //存總表
                db.SummaryTable.Add(summaryTable);
                db.SaveChanges();

                //存外匯交易表
                var uid = db.SummaryTable.Select(c => c.STId).ToList().LastOrDefault();
                tradeTable.SummaryId = uid;
                db.FXtradeTable.Add(tradeTable);
                db.SaveChanges();
                if (tradeTable.TradeClass == "賣出")
                {
                    CashIncome cashincome = new CashIncome();
                    cashincome.UserName = Session["User"].ToString();
                    cashincome.OID = uid;
                    cashincome.InCashType = summaryTable.TradeType;
                    cashincome.InDate = summaryTable.TradeDate;
                    cashincome.InNote = tradeTable.TradeClass;
                    var change = tradeTable.NTD * -1;
                    cashincome.InAmount = Convert.ToInt32(change);
                    db.CashIncome.Add(cashincome);
                    db.SaveChanges();
                    //更新個人資產表
                    TestHomeController th = new TestHomeController();
                    var name = summaryTable.UserName;
                    var ud = th.GetUsersData(name);

                    usersData.FXValue = th.SaveUserdata(name);
                    usersData.UserName = ud[0].UserName;
                    var cv = Math.Abs(tradeTable.NTD.Value);
                    usersData.CashValue = ud[0].CashValue + cv;
                    usersData.InsuranceValue = ud[0].InsuranceValue;
                    usersData.StockValue = ud[0].StockValue;
                    usersData.FundValue = ud[0].FundValue;
                    usersData.Email = ud[0].Email;
                    usersData.Password = ud[0].Password;
                    db.Entry(usersData).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    CashExpense cashexpense = new CashExpense();
                    cashexpense.UserName = Session["User"].ToString();
                    cashexpense.OID = uid;
                    cashexpense.ExCashType = summaryTable.TradeType;
                    cashexpense.ExDate = summaryTable.TradeDate;
                    cashexpense.ExNote = tradeTable.TradeClass;
                    if (tradeTable.TradeClass == "買入(不要連動新臺幣帳戶)")
                    {
                        cashexpense.ExAmount = 0;
                    }
                    else
                    {
                        cashexpense.ExAmount = Convert.ToInt32(tradeTable.NTD);
                    }
                    db.CashExpense.Add(cashexpense);
                    db.SaveChanges();
                    //更新個人資產表
                    TestHomeController th = new TestHomeController();
                    var name = summaryTable.UserName;
                    var ud = th.GetUsersData(name);

                    usersData.FXValue = th.SaveUserdata(name);
                    usersData.UserName = ud[0].UserName;
                    var cv = Math.Abs(tradeTable.NTD.Value);
                    if (tradeTable.TradeClass == "買入(不要連動新臺幣帳戶)")
                    {
                        usersData.CashValue = ud[0].CashValue;
                    }
                    else
                    {
                        usersData.CashValue = ud[0].CashValue - tradeTable.NTD;
                    }
                    usersData.InsuranceValue = ud[0].InsuranceValue;
                    usersData.StockValue = ud[0].StockValue;
                    usersData.FundValue = ud[0].FundValue;
                    usersData.Email = ud[0].Email;
                    usersData.Password = ud[0].Password;
                    db.Entry(usersData).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            return View(tradeTable);
        }

        // GET: TradeTables/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

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
            if (Session["User"] == null)
            {
                return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
            }

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
