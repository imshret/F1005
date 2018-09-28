using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using F1005.Areas.Insurances.Models;
using F1005.Models;


namespace F1005.Controllers
{
    public class InsurancesController : Controller
    {
        private ISEntities db = new ISEntities();

        // GET: Insurances
        public ActionResult Index()
        {
            //有無登入者的驗證
            string user = Session["User"] as string;
            if (String.IsNullOrEmpty(user))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(db.Insurance.ToList());
        }

        // GET: Insurances/Details/5
        public ActionResult Details(int? id)
        { 

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurance insurance = db.Insurance.Find(id);
            if (insurance == null)
            {
                return HttpNotFound();
            }
            return View(insurance);
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
        public ActionResult Create([Bind(Include = "Serial_Number,UserID,InsuranceName,BuyDate,CashFlow")] Insurance insurance)
        {
            if (ModelState.IsValid)
            {
                db.Insurance.Add(insurance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insurance);
        }

        // GET: Insurances/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurance insurance = db.Insurance.Find(id);
            if (insurance == null)
            {
                return HttpNotFound();
            }
            return View(insurance);
        }

        // POST: Insurances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Serial_Number,UserID,InsuranceName,BuyDate,CashFlow")] Insurance insurance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insurance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insurance);
        }

        // GET: Insurances/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insurance insurance = db.Insurance.Find(id);
            if (insurance == null)
            {
                return HttpNotFound();
            }
            return View(insurance);
        }

        [HttpPost]
        public ActionResult Calcucate(IRRViewModel data)
        {
            IRRCalculater cal = new IRRCalculater();
            var result = cal.IRR(data);
            return Content(result);
        }

        // POST: Insurances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insurance insurance = db.Insurance.Find(id);
            db.Insurance.Remove(insurance);
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
