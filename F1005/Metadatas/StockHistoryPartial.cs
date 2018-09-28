using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace F1005.Models
{
    [MetadataType(typeof(StockHistoryMetadata))]
    public partial class StockHistory
    {
    }

    public class StockHistoryMetadata
    {

        [Key]
        public int stockTradeID { get; set; }
        [Display(Name = "股票代碼")]
        public string stockID { get; set; }

        [Display(Name = "價格")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public Nullable<decimal> stockPrice { get; set; }
        [Display(Name = "數量")]
        public Nullable<int> stockAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Display(Name = "手續費")]
        public Nullable<int> stockFee { get; set; }
        [Display(Name = "證所稅")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public Nullable<int> stockTax { get; set; }
        [Display(Name = "現金變動")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public Nullable<int> stockNetincome { get; set; }
        [Display(Name = "目標價")]
        //[DisplayFormat(DataFormatString = "{2:C2}")]
        public Nullable<decimal> stockTP { get; set; }
        [Display(Name = "由現金帳戶扣款")]
        public bool CashAccount { get; set; }
        [Display(Name = "筆記")]
        public string stockNote { get; set; }

    }
}