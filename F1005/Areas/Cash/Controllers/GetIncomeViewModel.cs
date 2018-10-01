namespace F1005.Areas.Cash.Controllers
{
    internal class GetIncomeViewModel
    {
        public int InCashID { get; set; }
        public string UserName { get; set; }
        public string InCashType { get; set; }
        public int? InAmount { get; set; }
        public string InDate { get; set; }
        public string InNote { get; set; }
    }
}