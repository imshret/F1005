using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace F1005.Models
{
    [MetadataType(typeof(IDListMetadata))]
    public partial class StockIDList
    {
    }

    public class IDListMetadata
    {
        [Key]
        [ForeignKey("StockHisroty")]
        [Display(Name = "股票代碼")]
        public string stockID { get; set; }
        [Display(Name = "股票名稱")]
        public string stockName { get; set; }
    }
}