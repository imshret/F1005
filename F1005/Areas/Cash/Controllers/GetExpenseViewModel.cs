namespace F1005.Areas.Cash.Controllers
{
    internal class GetExpenseViewModel
    {
        public int ExCashID { get; set; }
        public string UserID { get; set; }
        public ExpenseType ExCashType { get; set; }
        public int? ExAmount { get; set; }
        public string ExDate { get; set; }
        public string ExNote { get; set; }
    }

    public enum ExpenseType
    {
        現金 = 1,
        股票 = 2,
        外匯 = 3,
        基金 = 4,
        儲蓄險 = 5
    }
}