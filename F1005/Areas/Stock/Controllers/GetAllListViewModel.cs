namespace F1005.Areas.Stock.Controllers
{
    internal class GetAllListViewModel
    {
        public string stockID { get; set; }
        public decimal? stockPrice { get; set; }
        public int? stockAmount { get; set; }
        public decimal? stockTP { get; set; }
        public int? stockFee { get; set; }
        public int? stockTax { get; set; }
        public int? stockNetincome { get; set; }
        public string stockNote { get; set; }
        public string stockDate { get; set; }
    }
}