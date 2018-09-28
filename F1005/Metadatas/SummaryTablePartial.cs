using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace F1005.Models
{
    [MetadataType(typeof(SummaryTableMetadata))]
    public partial class SummaryTable
    {
    }

    public class SummaryTableMetadata
    {
        [Key]
        [ForeignKey("StockHisroty")]
        public int STId { get; set; }
        [Display(Name = "交易類別")]
        public string TradeType { get; set; }
        [Display(Name = "交易時間")]
        public System.DateTime TradeDate { get; set; }
        [Display(Name = "使用者名稱")]
        public string UserName { get; set; }
    }
}