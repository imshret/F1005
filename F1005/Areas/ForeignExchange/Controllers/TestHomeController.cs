using F1005.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace F1005.Areas.ForeignExchange.Controllers
{
    public class TestHomeController : Controller
    {
        MyInvestEntities dc = new MyInvestEntities();
        // GET: TestHome
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetDataTable()
        {
            var x = Session["User"].ToString();
            var result = dc.FXtradeTable.ToList().Where(c => c.SummaryTable.UserName == x).Select(c=> new GetDataViewModel
            {
                Id = c.Id,
                SummaryId = c.SummaryId,
                TradeClass = c.TradeClass,
                CurrencyClass = c.CurrencyClass,
                NTD = c.NTD,
                USD = c.USD,
                ExchargeRate = c.ExchargeRate,
                Tradetime = c.SummaryTable.TradeDate.ToLongDateString(),
                UserName = c.SummaryTable.UserName,
                note=c.note       
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void Post([FromBody]FXtradeTable tt)
        {
            dc.FXtradeTable.Add(tt);
            dc.SaveChanges();
        }

        public void Delete(int id)
        {
            FXtradeTable tt = dc.FXtradeTable.Find(id);
            if (tt == null)
                return;
            var name = Session["User"].ToString();
            CashIncome cashincome = new CashIncome();
            if (tt.TradeClass == "賣出")
            {
                CashExpense cashexpense = new CashExpense();
                cashexpense.UserName = Session["User"].ToString();

                var summary = dc.SummaryTable.Where(c => c.UserName == name).ToList();

                cashexpense.OID = summary[0].STId;
                cashexpense.ExCashType = summary[0].TradeType;
                cashexpense.ExDate = summary[0].TradeDate;
                cashexpense.ExNote = "資料表刪除";
                cashexpense.ExAmount = Convert.ToInt32(Math.Abs(tt.NTD.Value));
                dc.CashExpense.Add(cashexpense);

                dc.FXtradeTable.Remove(tt);
                dc.SaveChanges();

                UsersData usersData = new UsersData();
                TestHomeController th = new TestHomeController();
                var ud = th.GetUsersData(name);
                usersData.FXValue = th.SaveUserdata(name);
                usersData.UserName = ud[0].UserName;
                var cv = Math.Abs(tt.NTD.Value);
                usersData.CashValue = ud[0].CashValue - cv;
                usersData.InsuranceValue = ud[0].InsuranceValue;
                usersData.StockValue = ud[0].StockValue;
                usersData.FundValue = ud[0].FundValue;
                usersData.Email = ud[0].Email;
                usersData.Password = ud[0].Password;
                dc.Entry(usersData).State = EntityState.Modified;
                dc.SaveChanges();
            }
            else
            {
                var summary = dc.SummaryTable.Where(c => c.UserName == name).ToList();

                cashincome.UserName = name;
                cashincome.OID = summary[0].STId;
                cashincome.InCashType = summary[0].TradeType;
                cashincome.InDate = summary[0].TradeDate;
                cashincome.InNote = "資料表刪除";
                var cv = Math.Abs(tt.NTD.Value);
                if (tt.TradeClass == "買入(不要連動新臺幣帳戶)")
                {
                    cashincome.InAmount = 0;
                }
                else
                {
                    cashincome.InAmount = Convert.ToInt32(cv);
                }
                dc.CashIncome.Add(cashincome);

                dc.FXtradeTable.Remove(tt);
                dc.SaveChanges();

                UsersData usersData = new UsersData();
                TestHomeController th = new TestHomeController();

                var ud = th.GetUsersData(name);

                usersData.FXValue = th.SaveUserdata(name);
                usersData.UserName = ud[0].UserName;
                if (tt.TradeClass == "買入(不要連動新臺幣帳戶)")
                {
                    usersData.CashValue = ud[0].CashValue;
                }
                else
                {
                    usersData.CashValue = ud[0].CashValue + cv;
                }
                usersData.InsuranceValue = ud[0].InsuranceValue;
                usersData.StockValue = ud[0].StockValue;
                usersData.FundValue = ud[0].FundValue;
                usersData.Email = ud[0].Email;
                usersData.Password = ud[0].Password;
                dc.Entry(usersData).State = EntityState.Modified;
                dc.SaveChanges();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dc.Dispose();
            }
            base.Dispose(disposing);
        }
        //Jqgrid
        // GET: Admin/Ranger/QueryRanger
        public JsonResult Jqgridtable()
        {
            var x = Session["User"].ToString();
            var result = dc.FXtradeTable.ToList().Where(c => c.SummaryTable.UserName == x).Select(c => new GetDataViewModel
            {
                Id = c.Id,
                SummaryId = c.SummaryId,
                TradeClass = c.TradeClass,
                CurrencyClass = c.CurrencyClass,
                NTD = c.NTD,
                USD = c.USD,
                ExchargeRate = c.ExchargeRate,
                Tradetime = c.SummaryTable.TradeDate.ToLongDateString(),
                UserName = c.SummaryTable.UserName,
                note=c.note
            });

            //組成jqGrid要讀取的資料
            var jsonData = new
            {
                rows = result.ToList()    //jqGrid呈現在表格中的資料
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        //chart新臺幣累積圖
        //public JsonResult chartpie()
        //{
        //    var x = Session["User"].ToString();
        //    var result = dc.FXtradeTable.Where(c => c.SummaryTable.UserName == x).GroupBy(c => c.CurrencyClass, c => c.NTD, (CurrencyClass, NTD) => new
        //    {
        //        Currency = CurrencyClass,
        //        NTD = NTD.Sum()
        //    });
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //chart外幣淨值圖
        public JsonResult chartpie1()
        {
            var x = Session["User"].ToString();
            var result = dc.FXtradeTable.Where(c => c.SummaryTable.UserName == x).GroupBy(c => c.CurrencyClass, c => c.USD, (CurrencyClass, USD) => new
            {
                Currency = CurrencyClass,
                USD = USD.Sum()
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //交易變動存回個人資產表
        public double SaveUserdata(string s)
        {
            double fxtotal = new double();
            var gf = GetFxvalue(s);
            for (int i = 0; i < gf.Count; i++)
            {
                var gfcur = gf[i].Currency;
                var gn = GetFxrate(gfcur);
                var result = Convert.ToDouble(gn[0]) * gf[i].USD;
                fxtotal += result.Value;
            }
            return fxtotal;
        }
        public List<FxsaveUserdata> GetFxvalue(string s)
        {
            //var x = Session["User"].ToString();
            //var x = "msit119_one";
            var result = dc.FXtradeTable.Where(c => c.SummaryTable.UserName == s).GroupBy(c => c.CurrencyClass, c => c.USD, (CurrencyClass, USD) => new FxsaveUserdata
            {
                Currency = CurrencyClass,
                USD = USD.Sum()
            }).ToList();
            return result;
        }
        public List<string> GetFxrate(string s)
        {
            var ret = dc.CurrencyRate.Where(c => c.CurrencyClass == s).Select(c => c.OnlineSell).ToList();
            return ret;
        }
        public List<UsersData> GetUsersData(string s)
        {
            var result = dc.UsersData.Where(c => c.UserName == s).ToList();
            return result;
        }
    }
}