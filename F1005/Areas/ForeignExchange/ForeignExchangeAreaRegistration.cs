using System.Web.Mvc;

namespace F1005.Areas.ForeignExchange
{
    public class ForeignExchangeAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ForeignExchange";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ForeignExchange_default",
                "ForeignExchange/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}