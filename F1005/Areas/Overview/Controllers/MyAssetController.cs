using F1005.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace F1005.Areas.Overview.Controllers
{
    public class MyAssetController : Controller
    {
        private MyInvestEntities db = new MyInvestEntities();

        // GET: Overview/MyAsset
        public ActionResult Index()
        {
            return View();
        }

        // GET: Overview/MyAsset/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Overview/MyAsset/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Overview/MyAsset/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Overview/MyAsset/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Overview/MyAsset/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Overview/MyAsset/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Overview/MyAsset/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //====================================

        //Get Asset Percentage
        public ActionResult GetDoughnut()
        {
            var Assquery = from d in db.SummaryTable
                           group d by new { d.TransType } into g
                           select new
                           {
                               AssType = g.Key.TransType,
                               Percentage1 = (Double)g.Sum(m => m.Amount) / (Double)(from d in db.FinanceAsset
                                                                                     select new
                                                                                     {
                                                                                         d.Amount
                                                                                     }).Sum(c => c.Amount)
                           };
            var query = Assquery.ToList().Select(c => new
            {
                MyType = c.AssType,
                Percentage = (c.Percentage1 * 100).ToString("f2")
            });

            return Json(query, JsonRequestBehavior.AllowGet);
        }
    }
}
