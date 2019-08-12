using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM_UWP.Rss.Models
{
    public class RSSViewModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Link { get; set; }
        public string Date { get; set; }
    }

    public class News
    {
        public string Link { get; set; }
        public string Date { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public List<string> Labels { get; set; }
    }
}
