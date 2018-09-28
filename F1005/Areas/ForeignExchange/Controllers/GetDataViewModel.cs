namespace F1005.Areas.ForeignExchange.Controllers
{
    internal class GetDataViewModel
    {
        public int Id { get; set; }
        public int SummaryId { get; set; }
        public string TradeClass { get; set; }
        public string CurrencyClass { get; set; }
        public double? NTD { get; set; }
        public double? USD { get; set; }
        public double? ExchargeRate { get; set; }
        public string Tradetime { get; set; }
        public string UserName { get; set; }
        public string note { get; set; }
    }
}