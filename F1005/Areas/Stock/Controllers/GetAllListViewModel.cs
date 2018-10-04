namespace F1005.Areas.Stock.Controllers
{
    internal class GetAllListViewModel
    {
        public int stockTradeID { get; set; }
        public string stockID { get; set; }
        public string stockPrice { get; set; }
        public int? stockAmount { get; set; }
        public string stockTP { get; set; }
        public string stockFee { get; set; }
        public string stockTax { get; set; }
        public string stockNetincome { get; set; }
        public string stockNote { get; set; }
        public string stockDate { get; set; }
    }
}