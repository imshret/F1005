using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace F1005.Areas.ForeignExchange.Controllers
{
    public class GetCurrencyViewModel
    {
        public int ID { get; set; }
        public string CurrencyClass { get; set; }
        public string CashBuy { get; set; }
        public string CashSell { get; set; }
        public string OnlineBuy { get; set; }
        public string OnlineSell { get; set; }
        public string Date { get; set; }
        public string CurrencyClassName { get; set; }
    }
}