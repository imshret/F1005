using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace F1005.Models
{
    public class InsurancesMetadata
    {
        [Display(Name ="交易序號")]
        public int SerialNumber { get; set; }
        [Display(Name = "用戶ID")]
        public string UserID { get; set; }
        [Display(Name = "保單名稱")]
        public string InsuranceName { get; set; }
        [Display(Name = "購買日期")]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode =false)]
        public System.DateTime PurchaseDate { get; set; }
        [Display(Name = "解約日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = false)]
        public System.DateTime WithdrawDate { get; set; }
        [Display(Name = "年繳保費")]
        [DisplayFormat(DataFormatString = "{0:C}",ApplyFormatInEditMode =false)]
        public int PaymentPerYear { get; set; }
        [Display(Name = "繳費年期")]
        public int PayYear { get; set; }
        [Display(Name = "購買or解約")]
        public Nullable<bool> PurchaseOrWithdraw { get; set; }
        [Display(Name = "現金流")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public Nullable<int> CashFlow { get; set; }
        [Display(Name = "解約金")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public int Withdrawal { get; set; }
        [Display(Name = "現金扣款")]
        public bool RelateCash { get; set; }
    }
    [MetadataType(typeof(InsurancesMetadata))]
    public partial class Insurances: IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PurchaseDate > WithdrawDate)
            {
                yield return new ValidationResult("解約日期必須大於購買日期", new string[] { "WithdrawDate", "PurchaseDate" });
            }
            if (WithdrawDate.Year - PurchaseDate.Year < PayYear)
            {
                yield return new ValidationResult("保單年度小於繳費年度", new string[] { "PayYear" });
            }
        }
    }
}