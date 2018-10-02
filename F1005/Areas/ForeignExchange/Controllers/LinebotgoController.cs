using F1005.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace F1005.Areas.ForeignExchange.Controllers
{
    public class LinebotgoController : Controller
    {
        MyInvestEntities dc = new MyInvestEntities();
        // GET: TestHome 

        //public HttpResponseMessage Index()
        //{
        //    return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Service is LIVE!!");
        //}
        //public async Task<HttpStatusCode> LinePost()
        public void LinePost1()
        {
            //var rawData = Request;
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string rawData = new StreamReader(req).ReadToEnd();
            var result = JsonConvert.DeserializeObject<Linebotclass>(rawData);
            string userid = result.events[0].source.userId; //使用者 ID
            string message = result.events[0].message.text; //文字訊息
            var replyToken = result.events[0].replyToken; //回傳的 token

            //var rate = LineFXgetrates(message);
            var rate = LinegetStockname(message);
            var arr = rate.ToArray();
            var count = rate.Count();
            var replytext = "";
            //if (count == 1)  //外匯顯示
            //{
            //    replytext = "您搜尋的 " + message + " (" + arr[0].Name + ")" + arr[0].Date.Value.ToShortDateString() + "的匯率為 " + arr[0].OnlineSell;
            //}
            //else
            //{
            //    replytext = "不好意思，查無此外幣種類，請重新輸入!";
            //}
            if (count == 1)  //股票顯示
            {
                replytext = "您搜尋的 " + message + "前一日收盤價為 "+ arr[0].收盤價 + "當日漲幅為 " + arr[0].漲跌價差;
            }
            else
            {
                replytext = "不好意思，查無此外幣種類，請重新輸入!";
            }

            var replyMessage = new
            {
                replyToken = replyToken,
                messages = new object[] 
                { new
                    {
                        type = "text",
                        text = replytext
                    }
                }
            };
            string s = JsonConvert.SerializeObject(replyMessage);
            byte[] bytes = Encoding.UTF8.GetBytes(s);

            WebClient webClient = new WebClient();
            webClient.Headers.Clear();
            webClient.Headers.Add("Content-Type", "application/json");
            webClient.Headers.Add("Authorization", "Bearer " + "wi5XRkwWHAXrh7vLyGbo2qKctW1Rv5P4O6i0wsnCS5+MYJDWmQdoCTlLwVk0xDWVFeEnlVNMvrTIMsQjdurzto03j+10zBrJO1LHaDf/T6zF4YENyFdSGGHN8CMGh2Qwb3NCWXz9PR7stpETWzZvMwdB04t89/1O/w1cDnyilFU=");
            webClient.UploadData("https://api.line.me/v2/bot/message/reply", bytes);
            //var botHelper = BotHelper
            //Trace.TraceInformation($"rawData:{rawData}");
            //try
            //{
            //    //var result = await 
            //    return System.Net.HttpStatusCode.OK;

            //}
            //catch (Exception exp)
            //{
            //    Trace.TraceError(exp.Message);
            //    return HttpStatusCode.NotFound;
            //}
        }
        public IQueryable<CurrencyRate> LineFXgetrates(string cur)
        {
            var rate = dc.CurrencyRate.Where(c => c.CurrencyClass == cur);           
            return rate;
        }
        public IQueryable<Stock_data> LinegetStockname(string cur)
        {
            var rate = dc.Stock_data.Where(c => c.證券名稱 == cur);
            return rate;
        }

        public void LinePost()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string rawData = new StreamReader(req).ReadToEnd();
            var result = JsonConvert.DeserializeObject<Linebotclass>(rawData);
            string userid = result.events[0].source.userId; //使用者 ID
            string message = result.events[0].message.text; //文字訊息
            var replyToken = result.events[0].replyToken; //回傳的 token

            var rate = LineFXgetrates(message);
            var arr = rate.ToArray();
            var count = rate.Count();
            var replytext = "";
            if (count == 1)  //外匯顯示
            {
                replytext = "您搜尋的 " + message + " (" + arr[0].Name + ")" + arr[0].Date.Value.ToShortDateString() + "的匯率為 " + arr[0].OnlineSell;
            }
            else
            {
                var rate1 = LinegetStockname(message);
                var arr1 = rate1.ToArray();
                var count1 = rate1.Count();
                if (count1 == 1)  //股票顯示
                {
                    replytext = "您搜尋的 " + message + "前一日收盤價為 " + arr1[0].收盤價 + "當日漲幅為 " + arr1[0].漲跌價差;
                }
                else
                {
                    replytext = "不好意思，查無此外幣或股票種類，請重新輸入!";
                }
            }

            var replyMessage = new
            {
                replyToken = replyToken,
                messages = new object[]
                { new
                    {
                        type = "text",
                        text = replytext
                    }
                }
            };
            string s = JsonConvert.SerializeObject(replyMessage);
            byte[] bytes = Encoding.UTF8.GetBytes(s);

            WebClient webClient = new WebClient();
            webClient.Headers.Clear();
            webClient.Headers.Add("Content-Type", "application/json");
            webClient.Headers.Add("Authorization", "Bearer " + "wi5XRkwWHAXrh7vLyGbo2qKctW1Rv5P4O6i0wsnCS5+MYJDWmQdoCTlLwVk0xDWVFeEnlVNMvrTIMsQjdurzto03j+10zBrJO1LHaDf/T6zF4YENyFdSGGHN8CMGh2Qwb3NCWXz9PR7stpETWzZvMwdB04t89/1O/w1cDnyilFU=");
            webClient.UploadData("https://api.line.me/v2/bot/message/reply", bytes);
        }


    }
}