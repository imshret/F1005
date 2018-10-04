using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace F1005.Areas.InsurancesandFund.Models
{
    public class InsurancesViewModel
    {
        public int SerialNumber { get; set; }
        public Nullable<int> STID { get; set; }
        public string UserID { get; set; }
        public string InsuranceName { get; set; }
        public string PurchaseDate { get; set; }
        public string WithdrawDate { get; set; }
        public string PaymentPerYear { get; set; }
        public Nullable<int> PayYear { get; set; }
        public Nullable<bool> PurchaseOrWithdraw { get; set; }
        public string Withdrawal { get; set; }
        public Nullable<bool> Withdrawed { get; set; }
        public Nullable<int> CashFlow { get; set; }


    }
   
    
}