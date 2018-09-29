using F1005.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace F1005.Areas.BackStage.Controllers
{
    public class BSController : Controller
    {
        // GET: BackStage/BS
        public ActionResult IndexStock()
        {
            MyInvestEntities db = new MyInvestEntities();
                       
            return View(db.StockHistory.ToList());
        }

        public ActionResult IndexFX()
        {
            MyInvestEntities db = new MyInvestEntities();

            return View(db.FXtradeTable.ToList());
        }

    }
}