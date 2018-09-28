namespace F1005.Areas.Cash.Controllers
{
    internal class GetIncomeViewModel
    {
        public int InCashID { get; set; }
        public string UserName { get; set; }
        public IncomeType InCashType { get; set; }
        public int? InAmount { get; set; }
        public string InDate { get; set; }
        public string InNote { get; set; }
    }

    public enum IncomeType
    {
        現金 = 1,
        股票 = 2,
        外匯 = 3,
        基金 = 4,
        儲蓄險 = 5
    }
}