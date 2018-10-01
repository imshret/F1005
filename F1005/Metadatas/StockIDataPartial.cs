using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace F1005.Models
{
    [MetadataType(typeof(StockDataMetadata))]
    public partial class Stock_data
    {
    }

    public  class StockDataMetadata
    {
        [Key]
        [Display(Name = "股票代碼")]
        public string StockID { get; set; }
        public string 證券名稱 { get; set; }
        public string 成交股數 { get; set; }
        public string 成交金額 { get; set; }
        public string 開盤價 { get; set; }
        public string 最高價 { get; set; }
        public string 最低價 { get; set; }
        public string 收盤價 { get; set; }
        public string 漲跌價差 { get; set; }
        public string 成交筆數 { get; set; }
    }
}