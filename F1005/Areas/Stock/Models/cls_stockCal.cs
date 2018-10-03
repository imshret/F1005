using F1005.Areas.Stock.Controllers;
using F1005.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace F1005.Areas.Stock.Models
{
    public class cls_stockCal
    {
    }
    public class AVGCalculator
    {
        public decimal GetAvg(string username,string searchid,int invchange,int cashflow)
        {
            MyInvestEntities db = new MyInvestEntities();
            //var username = stockHistory.SummaryTable.UserName;
            //var searchid = stockHistory.stockID;
            //var invchange = (int)stockHistory.stockAmount;
            decimal avg = 0;
            //計算總剩餘庫存量
            //假設invsum+invchange !=0 (賣出數量一定不會超過庫存量)
            var Invsum = db.StockHistory.Where(m => m.stockID == searchid & m.SummaryTable.UserName == username).Sum(s => s.stockAmount);
            var _sum = Invsum + invchange;
            Invsum= (_sum != null) ? (_sum) : ( invchange);   //(結果:0、大於0)
            if (Invsum > 0)
            {
                if (Invsum > invchange)   //過去有該檔股票，且就算賣出也不會賣到光
                {
                    //買進記錄按日期排序
                    var BuyList = db.StockHistory.Where(m => m.stockID == searchid & m.stockAmount > 0).OrderByDescending(t => t.SummaryTable.TradeDate).ToArray();
                    Decimal TotalCost = 0;
                    TotalCost = (invchange >= 0) ? (decimal)(cashflow * (-1)) : 0;  //如果買進則初始值為此筆成本，如果為賣出則初始值為0
                    int _tempinv = 0;
                    _tempinv = (invchange >= 0) ? (int)(Invsum - invchange) : (int)Invsum;

                    foreach (var item in BuyList)
                    {
                        int iteminv = (int)item.stockAmount;
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
                    avg =(decimal)( TotalCost / Invsum);
                    return avg;
                }
                else //新增持股
                {
                    avg = (decimal)(cashflow*(-1) / invchange);
                    return avg;
                }
            }
            else    //賣出持股後庫存為0
            {
                avg = 0;
                return avg;
            }
        }


        public decimal GetAvg2(string username,string searchid)
        {
            MyInvestEntities db = new MyInvestEntities();
            var Invsum = db.StockHistory.Where(m => m.stockID == searchid & m.SummaryTable.UserName == username).Sum(s => s.stockAmount);
            Invsum = (Invsum != null) ? Invsum : 0;

            //買進記錄按日期排序
            var BuyList = db.StockHistory.Where(m => m.stockID == searchid & m.stockAmount > 0).OrderByDescending(t => t.SummaryTable.TradeDate).ToArray();

            Decimal TotalCost = 0;
            Decimal _tempinv = (Decimal)Invsum;

            if (Invsum > 0)
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
                Decimal Avgcost = (Decimal)(TotalCost / (Invsum));

                return Avgcost;
            }
            else
            {

                Decimal Avgcost = 0;

                //回傳無庫存

                return Avgcost;
            }

        }


        public decimal  getpv(int? amount,string lastprice)
        {
            decimal pv = 0;
            pv = (Convert.ToDecimal(amount) * Convert.ToDecimal(lastprice));
            return pv;
        }

        public decimal getProfitPercent(decimal avgcost, string lastprice)
        {
            decimal avg = avgcost; 
            decimal percent = 0;
            percent = ((Convert.ToDecimal(lastprice))- avg) / avg;


            return percent;

        }

        public decimal getPVsum(string username) {
            MyInvestEntities db = new MyInvestEntities();
            var InvList = db.StockHistory.Where(c => c.SummaryTable.UserName == username).GroupBy(c => c.stockID, c => new InvViewModel
            {
                amount = c.stockAmount,
                pv = c.Stock_data.收盤價

            }, (id, invVM) => new            
            {
                stockamount = invVM.Select(c => c.amount).Sum(),
                stockpv = invVM.Select(c => c.pv).FirstOrDefault(),
            });
            decimal totalpv = 0;
            foreach (var item in InvList)
            {
                totalpv = (Convert.ToDecimal(item.stockamount)) * (Convert.ToDecimal(item.stockpv));
            }


            return totalpv;
        }

        public List<UsersData> GetUserdata(string username) {
            MyInvestEntities db = new MyInvestEntities();
            var userdata = db.UsersData.Where(c => c.UserName == username).ToList();
                return userdata;
        }

    }
}
