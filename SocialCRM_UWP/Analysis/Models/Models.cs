using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialCRM_UWP.Analysis.Models
{
    
    public class InstagramLightMediaModel
    {
        public string id { get; set; }
        public string edge_media_to_caption_edges_node_text { get; set; }
        public string shortcode { get; set; }
        public long edge_media_to_comment_count { get; set; }
        public string taken_at_timestamp { get; set; }
        public string display_url { get; set; }
        public long edge_liked_by_count { get; set; }
        public long edge_media_preview_like_count { get; set; }
        public string owner_id { get; set; }
        public string thumbnail_src { get; set; }
        public bool is_video { get; set; }
        public long video_view_count { get; set; }

        public string profile_pic_url { get; set; }
        public bool is_private { get; set; }
        public long following_count { get; set; }
        public long follower_count { get; set; }
        public string full_name { get; set; }
        public string username { get; set; }
        public string usertags_count { get; set; }
        public string media_count { get; set; }
    }

    public class AparatVideoModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string username { get; set; }
        public string userid { get; set; }
        public string visit_cnt { get; set; }
        public string uid { get; set; }
        public string sender_name { get; set; }
        public string big_poster { get; set; }
        public string small_poster { get; set; }
        public string profilePhoto { get; set; }
        public string duration { get; set; }
        public string sdate { get; set; }
        public string create_date { get; set; }
        public string sdate_timediff { get; set; }
        public string frame { get; set; }
        public string official { get; set; }

        public string pic_m { get; set; }
        public string video_cnt { get; set; }
        public string url { get; set; }
        public string follower_cnt { get; set; }
        public string followed_cnt { get; set; }

        public string like_cnt { get; set; }
    }


}
