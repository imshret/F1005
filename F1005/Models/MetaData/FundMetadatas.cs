using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace F1005.Models
{
    public class FundMetadata
    {
        [Display(Name = "交易序號")]
        public int SerialNumber { get; set; }
        [Display(Name = "用戶ID")]
        public string UserID { get; set; }
        [Display(Name = "基金名稱")]
        public string FundName { get; set; }
        [Display(Name = "申購or贖回")]
        public Nullable<bool> BuyOrSell { get; set; }
        [Display(Name = "手續費")]
        public Nullable<double> Fee { get; set; }
        [Display(Name = "單位數")]
        public double Units { get; set; }
        [Display(Name = "交易日期")]
        public System.DateTime Date { get; set; }
        [Display(Name = "淨值")]
        public double NAV { get; set; }
        [Display(Name = "現金流")]
        public double CashFlow { get; set; }
    }
    [MetadataType(typeof(FundMetadata))]
    public partial class Fund
    {

    }



}