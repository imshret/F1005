﻿using F1005.Models;
using System;

namespace F1005.Areas.BStage.Models
{
    public class BSViewModel
    {     
        public string UserName { get; set; }

        public string ExCashType { get; set; }
        public Nullable<int> ExAmount { get; set; }
        //public System.DateTime ExDate { get; set; }
        public string ExDate { get; set; }
        public string ExNote { get; set; }
                
        public string InCashType { get; set; }
        public Nullable<int> InAmount { get; set; }
        public System.DateTime InDate { get; set; }
        public string InNote { get; set; }


        public string stockID { get; set; }
        public Nullable<decimal> stockPrice { get; set; }
        public Nullable<int> stockAmount { get; set; }
        public string stockNote { get; set; }
        

        public string CurrencyClass { get; set; }
        //public string Tradetime { get; set; }
        public Nullable<System.DateTime> Tradetime { get; set; }
        public string TradeClass { get; set; }
        public string note { get; set; }

        public string InsuranceName { get; set; }
        public System.DateTime PurchaseDate { get; set; }
        public System.DateTime WithdrawDate { get; set; }
        public Nullable<int> PaymentPerYear { get; set; }
        public Nullable<int> PayYear { get; set; }   
        public Nullable<int> Withdrawal { get; set; }
        
        public string FundName { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<double> NAV { get; set; }
        public Nullable<double> Units { get; set; }






        //public virtual SummaryTable SummaryTable { get; set; }
    }

    //internal class IQueryable<AllModel>
    //{
      
    //    public string TypeName { get; set; }
    //    public string Nums { get; set; }

    //}
}