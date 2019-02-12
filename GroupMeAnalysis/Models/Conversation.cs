using System.Collections.Generic;

namespace GroupMeAnalysis.Models
{
    public class Conversation {
        public int id { get; set; }
        public int group_id { get; set; }
        public int creator_user_id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string image_url { get; set; }
        public int created_at { get; set; }
        public List<User> members { get; set; }
    }
}