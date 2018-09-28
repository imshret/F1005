using System.Web;
using System.Web.Optimization;

namespace F1005
{
    public class BundleConfig
    {
        // 如需統合的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好可進行生產時，請使用 https://modernizr.com 的建置工具，只挑選您需要的測試。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/themes/base/jquery-ui.min.css",
                      "~/Content/jquery.jqGrid/ui.jqgrid.css"));

            bundles.Add(new ScriptBundle("~/bundles/jsgrid").Include(
                      "~/Content/jsgrid/jsgrid.min.js",
                      "~/Content/jsgrid/i18n/jsgrid-zh-tw.js",
                      "~/Scripts/jquery.jqGrid.min.js",
                      "~/Scripts/i18n/grid.locale-tw.js"                     
          
           ));
        }
    }
}
