using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace F1005.Areas.InsurancesandFund.Models
{
    public class FundViewModel
    {
        public int SerialNumber { get; set; }
        public Nullable<int> STID { get; set; }
        public string UserID { get; set; }
        public string FundName { get; set; }
        public Nullable<bool> BuyOrSell { get; set; }
        public Nullable<double> Fee { get; set; }
        public Nullable<double> Units { get; set; }
        public string Date { get; set; }
        public Nullable<double> NAV { get; set; }
        public Nullable<double> CashFlow { get; set; }
        public bool RelateCash { get; set; }
        public Nullable<double> SellNAV { get; set; }
        public Nullable<double> CurrentNAV { get; set; }

    }
    
}