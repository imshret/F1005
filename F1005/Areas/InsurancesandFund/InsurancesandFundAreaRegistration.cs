using System.Web.Mvc;

namespace F1005.Areas.InsurancesandFund
{
    public class InsurancesandFundAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "InsurancesandFund";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "InsurancesandFund_default",
                "InsurancesandFund/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}