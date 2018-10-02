namespace F1005.Areas.Cash.Controllers
{
    internal class GetExpenseViewModel
    {
        public int ExCashID { get; set; }
        public string UserID { get; set; }
        public string ExCashType { get; set; }
        public string ExAmount { get; set; }
        public string ExDate { get; set; }
        public string ExNote { get; set; }
    }
}