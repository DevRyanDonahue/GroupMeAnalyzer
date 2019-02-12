using System.Collections.Generic;

namespace GroupMeAnalysis.Models
{
    public class Message {
        public int created_at { get; set; }
        public List<string> favorited_by { get; set; }
        public int group_id { get; set; }
        public long id { get; set; }
        public string sender_id { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        public string user_id { get; set; }
    }
}