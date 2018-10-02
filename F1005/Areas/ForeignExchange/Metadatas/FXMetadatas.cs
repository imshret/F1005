using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace F1005.Models
{
    [MetadataType(typeof(FXMetadatas))]
    public partial class FXtradeTable
    {
    }

    public class FXMetadatas
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "流水號")]
        public string SummaryId { get; set; }
        [Display(Name = "外幣種類")]
        public string CurrencyClass { get; set; }
        [Display(Name = "新臺幣金額")]
        public Nullable<double> NTD { get; set; }
        [Display(Name = "外幣金額")]
        public Nullable<double> USD { get; set; }
        [Display(Name = "匯率")]
        public Nullable<double> ExchargeRate { get; set; }
        [Display(Name = "優惠匯率")]
        public Nullable<double> tax { get; set; }
        [Display(Name = "交易類別")]
        public string TradeClass { get; set; }
        [Display(Name = "備註")]
        public string note { get; set; }
    }
}