using F1005.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace F1005.Areas.Stock.Controllers
{
    public class StockController : Controller
    {
        MyInvestEntities db = new MyInvestEntities();
        // GET: Stock
        public ActionResult Index()
        {
            return View();
        }
        //買進股票
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStockBuy([Bind(Include = "TradeType,TradeDate,UserName")]SummaryTable summaryTable, [Bind(Include = "stockID,stockPrice,stockAmount,stockFee,stockTax,stockNetincome,stockNote,CashAccount")]StockHistory stockHistory)
        {

            if (ModelState.IsValid)
            {
                //存入總表獲得交易序號
                db.SummaryTable.Add(summaryTable);
                db.SaveChanges();
                var id = db.SummaryTable.Select(c => c.STId).ToList().LastOrDefault();
                if (stockHistory.CashAccount)
                {
                    CashExpense expense = new CashExpense();
                    expense.OID = id;
                    expense.UserName = summaryTable.UserName;
                    expense.ExCashType = summaryTable.TradeType;
                    expense.ExAmount = (stockHistory.stockNetincome) * (-1);
                    expense.ExDate = summaryTable.TradeDate;
                    expense.ExNote = "買進股票";
                    db.CashExpense.Add(expense);
                    db.SaveChanges();
                }
                //存入股票交易紀錄表
                stockHistory.STId = id;
                stockHistory.stockAmount = stockHistory.stockAmount * 1000;
                db.StockHistory.Add(stockHistory);
                db.SaveChanges();
                ViewBag.Alert = "alert('新增成功')";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        //賣出股票
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStockSell([Bind(Include = "TradeType,TradeDate,UserName")]SummaryTable summaryTable, [Bind(Include = "stockID,stockPrice,stockAmount,stockFee,stockTax,stockNetincome,stockNote")]StockHistory stockHistory)
        {
            //([Bind(Include = "TradeType,date,userID")]
            //[Bind(Include = "stockID,stockPrice,stockAmount,stockFee,stockTax,stockNetincome,stockNote")]
            if (ModelState.IsValid)
            {
                //存入總表取得交易序號
                db.SummaryTable.Add(summaryTable);
                db.SaveChanges();
                var id = db.SummaryTable.Select(c => c.STId).ToList().LastOrDefault();

                //存入現金帳戶
                CashIncome cashincome = new CashIncome();
                cashincome.OID = id;
                cashincome.UserName = summaryTable.UserName;
                cashincome.InCashType = summaryTable.TradeType;
                cashincome.InAmount = (stockHistory.stockNetincome);
                cashincome.InDate = summaryTable.TradeDate;
                cashincome.InNote = "賣出股票";
                db.CashIncome.Add(cashincome);
                db.SaveChanges();


                //存入股票交易記錄表

                stockHistory.stockAmount = stockHistory.stockAmount * (-1000);
                stockHistory.STId = id;
                db.StockHistory.Add(stockHistory);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        //查詢庫存數量與成本
        [HttpGet]
        public JsonResult Cost(string searchid)
        {
            //計算總剩餘庫存量
            var username = Session["User"].ToString();

            var BuyInv = db.StockHistory.Where(m => m.stockID == searchid & m.stockNetincome < 0 & m.SummaryTable.UserName == username).Sum(s => s.stockAmount);
            var SellInv = db.StockHistory.Where(m => m.stockID == searchid & m.stockNetincome > 0 & m.SummaryTable.UserName == username).Sum(s => s.stockAmount);
            BuyInv = (BuyInv != null) ? BuyInv : 0;
            SellInv = (SellInv != null) ? SellInv : 0;
            var TotalInv = BuyInv + SellInv;
            //買進記錄按日期排序
            var BuyList = db.StockHistory.Where(m => m.stockID == searchid & m.stockNetincome < 0).OrderByDescending(t => t.SummaryTable.TradeDate).ToArray();

            Decimal TotalCost = 0;
            Decimal _tempinv = (Decimal)TotalInv;

            if (TotalInv > 0)
            {
                //從近一筆開始累加               
                foreach (var item in BuyList)
                {
                    Decimal iteminv = (Decimal)item.stockAmount;
                    if (iteminv <= _tempinv)
                    {
                        TotalCost = (Decimal)(TotalCost + item.stockNetincome * (-1));
                        _tempinv = _tempinv - iteminv;
                    }
                    else
                    {
                        TotalCost = TotalCost + (Decimal)(_tempinv * (item.stockNetincome / item.stockAmount) * (-1));
                        break;
                    }
                }
                Decimal Avgcost = (Decimal)(TotalCost / (TotalInv));


                List<InvData> InvDatas = new List<InvData>
                {
                    new InvData{
                        Inv=(Decimal)(TotalInv/1000),
                        Avgcost =Avgcost,
                    }
                };
                return Json(InvDatas, JsonRequestBehavior.AllowGet);
                //return Content(InvDatas);
            }
            else
            {
                //回傳無庫存
                ViewBag.InvState = "無庫存";
                return Json("無庫存", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult EXDCost(string searchid)
        {
            //計算總剩餘庫存量
            var username = Session["User"].ToString();
            //summaryTable.UserName = username;
            var BuyInv = db.StockHistory.Where(m => m.stockID == searchid & m.stockNetincome < 0 & m.SummaryTable.UserName == username).Sum(s => s.stockAmount);
            var SellInv = db.StockHistory.Where(m => m.stockID == searchid & m.stockNetincome > 0 & m.SummaryTable.UserName == username).Sum(s => s.stockAmount);
            BuyInv = (BuyInv != null) ? BuyInv : 0;
            SellInv = (SellInv != null) ? SellInv : 0;
            var TotalInv = BuyInv +SellInv
                ;
            //買進記錄按日期排序
            var BuyList = db.StockHistory.Where(m => m.stockID == searchid & m.stockNetincome < 0).OrderByDescending(t => t.SummaryTable.TradeDate).ToArray();

            Decimal TotalCost = 0;
            Decimal _tempinv = (Decimal)TotalInv;

            if (TotalInv > 0)
            {
                //從近一筆開始累加               
                foreach (var item in BuyList)
                {
                    Decimal iteminv = (Decimal)item.stockAmount;
                    if (iteminv <= _tempinv)
                    {
                        TotalCost = (Decimal)(TotalCost + item.stockNetincome * (-1));
                        _tempinv = _tempinv - iteminv;
                    }
                    else
                    {
                        TotalCost = TotalCost + (Decimal)(_tempinv * (item.stockNetincome / item.stockAmount) * (-1));
                        break;
                    }
                }
                Decimal Avgcost = (Decimal)(TotalCost / (TotalInv));
                List<InvData> InvDatas = new List<InvData>
                {
                    new InvData{
                        Inv=(Decimal)(TotalInv/1000),
                        Avgcost =Avgcost,
                    }
                };

                return Json(InvDatas, JsonRequestBehavior.AllowGet);
                //return Content(InvDatas);
            }
            else
            {
                //回傳無庫存
                ViewBag.InvState = "無庫存";
                return Json("無庫存", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult EXSCost(string searchid)
        {
            //計算總剩餘庫存量
            var username = Session["User"].ToString();
            //summaryTable.UserName = username;
            var BuyInv = db.StockHistory.Where(m => m.stockID == searchid & m.stockNetincome < 0 & m.SummaryTable.UserName == username).Sum(s => s.stockAmount);
            var SellInv = db.StockHistory.Where(m => m.stockID == searchid & m.stockNetincome > 0 & m.SummaryTable.UserName == username).Sum(s => s.stockAmount);
            BuyInv = (BuyInv != null) ? BuyInv : 0;
            SellInv = (SellInv != null) ? SellInv : 0;
            var TotalInv = BuyInv + SellInv;
            //買進記錄按日期排序
            var BuyList = db.StockHistory.Where(m => m.stockID == searchid & m.stockNetincome < 0).OrderByDescending(t => t.SummaryTable.TradeDate).ToArray();

            Decimal TotalCost = 0;
            Decimal _tempinv = (Decimal)TotalInv;

            if (TotalInv > 0)
            {
                //從近一筆開始累加               
                foreach (var item in BuyList)
                {
                    Decimal iteminv = (Decimal)item.stockAmount;
                    if (iteminv <= _tempinv)
                    {
                        TotalCost = (Decimal)(TotalCost + item.stockNetincome * (-1));
                        _tempinv = _tempinv - iteminv;
                    }
                    else
                    {
                        TotalCost = TotalCost + (Decimal)(_tempinv * (item.stockNetincome / item.stockAmount) * (-1));
                        break;
                    }
                }
                Decimal Avgcost = (Decimal)(TotalCost / (TotalInv));

                List<InvData> InvDatas = new List<InvData>
                {
                    new InvData{
                        Inv=(Decimal)(TotalInv/1000),
                        Avgcost =Avgcost,
                    }
                };
                return Json(InvDatas, JsonRequestBehavior.AllowGet);
                //return Content(InvDatas);
            }
            else
            {
                //回傳無庫存
                ViewBag.InvState = "無庫存";
                return Json("無庫存", JsonRequestBehavior.AllowGet);
            }
        }

        //匯入除息資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEXD([Bind(Include = "TradeType,TradeDate,UserName")]SummaryTable summaryTable, [Bind(Include = "stockID,stockPrice,stockAmount,stockFee,stockTax,stockNetincome,stockNote")]StockHistory stockHistory)
        {
            //([Bind(Include = "TradeType,date,userID")]
            //[Bind(Include = "stockID,stockPrice,stockAmount,stockFee,stockTax,stockNetincome,stockNote"])
            if (ModelState.IsValid)
            {

                db.SummaryTable.Add(summaryTable);
                db.SaveChanges();

                var id = db.SummaryTable.Select(c => c.STId).ToList().LastOrDefault();
                //存入現金帳戶
                CashIncome cashincome = new CashIncome();
                cashincome.OID = id;
                cashincome.UserName = summaryTable.UserName;
                cashincome.InCashType = summaryTable.TradeType;
                cashincome.InAmount = (stockHistory.stockNetincome);
                cashincome.InDate = summaryTable.TradeDate;
                cashincome.InNote = "除息收入";
                db.CashIncome.Add(cashincome);
                db.SaveChanges();

                //存入股票交易紀錄表
                stockHistory.STId = id;
                db.StockHistory.Add(stockHistory);
                db.SaveChanges();

                return RedirectToAction("Index");

            }


            return RedirectToAction("Index");
        }

        //匯入除權資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEXS([Bind(Include = "TradeType,TradeDate,UserName")]SummaryTable summaryTable, [Bind(Include = "stockID,stockPrice,stockAmount,stockFee,stockTax,stockNetincome,stockNote")]StockHistory stockHistory)
        {
            //([Bind(Include = "TradeType,date,userID")]
            //[Bind(Include = "stockID,stockPrice,stockAmount,stockFee,stockTax,stockNetincome,stockNote")
            if (ModelState.IsValid)
            {

                db.SummaryTable.Add(summaryTable);
                db.SaveChanges();

                var id = db.SummaryTable.Select(c => c.STId).ToList().LastOrDefault();
                stockHistory.STId = id;
                db.StockHistory.Add(stockHistory);
                db.SaveChanges();

                return RedirectToAction("Index");

            }


            return RedirectToAction("Index");
        }


        [HttpGet]
        public JsonResult Cash()
        {
            var username = Session["User"].ToString();
            //var username = "C001";
            //計算現金餘額
            var CashIn = db.CashIncome.Where(m => m.UserName == username).Sum(m => m.InAmount);
            var CashOut = db.CashExpense.Where(m => m.UserName == username).Sum(m => m.ExAmount);
            CashIn = (CashIn != null) ? CashIn : 0;
            CashOut = (CashOut != null) ? CashOut : 0;

            var cashaccount = CashIn - CashOut;

            //回傳現金帳戶狀態
            if (cashaccount >= 0)
            {
                return Json(cashaccount, JsonRequestBehavior.AllowGet);
                //return Content(InvDatas);
            }
            else
            {
                return Json("無現金資料", JsonRequestBehavior.AllowGet);
            }
        }

        //顯示特定User的股票買賣紀錄
        public ActionResult GetAllList()
        {
            var username = Session["User"].ToString();

            var query = db.StockHistory.ToList().Where(c => c.SummaryTable.UserName == username).Select(c => new GetAllListViewModel
            {
                stockID = c.stockID,
                stockPrice = c.stockPrice,
                stockAmount = c.stockAmount,
                stockTP = c.stockTP,
                stockFee = c.stockFee,
                stockTax = c.stockTax,
                stockNetincome = c.stockNetincome,
                stockNote = c.stockNote,
                stockDate = c.SummaryTable.TradeDate.ToShortDateString()
            });
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //顯示user股票庫存
        public ActionResult GetAllInv()
        {
            var username = Session["User"].ToString();
            var InvList = db.StockHistory.Where(c => c.SummaryTable.UserName == username).GroupBy(c => c.stockID, c => c.stockAmount, (id, amount) => new
            {
                stockid = id,
                stockamount = amount.Sum()
            });
            

           
            return Json(InvList, JsonRequestBehavior.AllowGet);
        }

    }
    public class InvData
    {
        public Decimal Inv { get; set; }
        public Decimal Avgcost { get; set; }
    }
}
