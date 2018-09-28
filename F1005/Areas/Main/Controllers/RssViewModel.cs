using System.ComponentModel.DataAnnotations;

namespace F1005.Areas.Main.Controllers
{
    public class RssViewModel
    {
        [Key]
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string PubDate { get; set; }
    }
}