using F1005.Models;
using System;

namespace F1005.Areas.BStage.Models
{
    public class BSViewModel
    {     
        public string UserName { get; set; }
        public string Email { get; set; }
        public string TradeDate { get; set; }

        public string ExCashType { get; set; }
        public Nullable<int> ExAmount { get; set; }
        //public System.DateTime ExDate { get; set; }
        public string ExDate { get; set; }
        public string ExNote { get; set; }
                
        public string InCashType { get; set; }
        public Nullable<int> InAmount { get; set; }
        public string InDate { get; set; }
        public string InNote { get; set; }


        public string stockID { get; set; }
        public Nullable<decimal> stockPrice { get; set; }
        public Nullable<int> stockAmount { get; set; }
        public string stockNote { get; set; }
        public Nullable<int> stockNetincome { get; set; }

        public string CurrencyClass { get; set; }    
        public Nullable<double> NTD { get; set; }
        public string note { get; set; }

        public string InsuranceName { get; set; }
        public string PurchaseDate { get; set; }
        public string WithdrawDate { get; set; }
        public Nullable<int> PaymentPerYear { get; set; }
        public Nullable<int> CashFlow { get; set; }
        public Nullable<int> PayYear { get; set; }   
        public Nullable<int> Withdrawal { get; set; }
        
        public string FundName { get; set; }
        public string Date { get; set; }
        public Nullable<double> NAV { get; set; }
        public Nullable<double> Units { get; set; }
        public Nullable<double> CashFlowX { get; set; }

        
        public Nullable<double> NetBenefit { get; set; }

        public virtual SummaryTable SummaryTable { get; set; }
    }
}