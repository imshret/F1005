using System.Web.Mvc;

namespace F1005.Areas.BackStage
{
    public class BackStageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BackStage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "BackStage_default",
                "BackStage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}