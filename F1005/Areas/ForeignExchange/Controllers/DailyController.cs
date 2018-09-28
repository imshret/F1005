using F1005.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace F1005.Areas.ForeignExchange.Controllers
{
    public class DailyController : ApiController
    {
        [HttpGet]
        [Route("api/FX/Getlist")]
        public object Getlist()
        {
            var db = new F1005.Models.MyInvestEntities();
            var ret = from c in db.CurrencyRate
                      select c;
            dynamic retObject = new { data = ret.ToList() };
            return retObject;
            //return Ok(retObject); 如果用IHttpActionResult 就需return Ok();
        }

        [HttpGet]
        [Route("api/FX/Getmoney")]
        //public IHttpActionResult Getlist()
        public object Getmoney()
        {
            var db = new MyInvestEntities();
            var ret = from c in db.CurrencyRate
                      select c;
            dynamic retObject = new { data = ret.ToList() };
            return retObject;
        }

        [HttpPost]
        [Route("api/FX/Getnowfc")]
        public IHttpActionResult Getnowfc()
        {
            //取得前端傳來的body內容
            var body = Request.Content.ReadAsStringAsync().Result;
            //將字串的JSON轉為rec物件
            var rec = Newtonsoft.Json.JsonConvert.DeserializeObject<String>(body);
            //新增資料庫資料
            var db = new MyInvestEntities();
            var ret = from c in db.CurrencyRate
                      where c.CurrencyClass.Contains(rec)
                      select c.OnlineSell;
            return Ok(ret);
        }

        [HttpPost]
        [Route("api/FX/tradedata")]
        public IHttpActionResult tradedata()
        {
            //取得前端傳來的body內容
            var body = Request.Content.ReadAsStringAsync().Result;
            //將字串的JSON轉為rec物件
            var rec = Newtonsoft.Json.JsonConvert.DeserializeObject<String>(body);
            //新增資料庫資料
            return Ok(rec);
        }
    }
}
