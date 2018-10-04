using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace F1005.Models
{
    [MetadataType(typeof(CashExpenseMetadata))]
    public partial class CashExpense : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            MyInvestEntities db = new MyInvestEntities();
            var Esum = db.CashExpense.Sum(c => c.ExAmount);
            var Isum = db.CashIncome.Sum(c => c.InAmount);
            var net = Isum - Esum;
            if (net < 0)
            {
                yield return new ValidationResult("收入不足，無法新增支出項目", new string [] { "ExAmount" });
            }
        }
    }

    public class CashExpenseMetadata
    {
        public int ExCashID { get; set; }
        public Nullable<int> OID { get; set; }
        public string UserName { get; set; }
        public string ExCashType { get; set; }
        public Nullable<int> ExAmount { get; set; }
        public System.DateTime ExDate { get; set; }
        public string ExNote { get; set; }

        public virtual SummaryTable SummaryTable { get; set; }
    }
}