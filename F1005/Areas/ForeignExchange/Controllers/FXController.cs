using F1005.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace F1005.Areas.ForeignExchange.Controllers
{
    public class FXController : Controller
    {
        // GET: FX
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult History()
        {
            return View();
        }

        public ActionResult ExchangeRates()
        {
            return View();
        }
    }
}