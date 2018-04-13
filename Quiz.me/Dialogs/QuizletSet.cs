using System;
using System.Collections.Generic;

namespace Quiz.me.Dialogs
{
    [Serializable]
    public class QuizletSet
    {
        public int id { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string created_by { get; set; }
        public int term_count { get; set; }
        public int created_date { get; set; }
        public int modified_date { get; set; }
        public int published_date { get; set; }
        public bool has_images { get; set; }
        public List<object> subjects { get; set; }
        public string visibility { get; set; }
        public string editable { get; set; }
        public bool has_access { get; set; }
        public bool can_edit { get; set; }
        public string description { get; set; }
        public string lang_terms { get; set; }
        public string lang_definitions { get; set; }
        public int password_use { get; set; }
        public int password_edit { get; set; }
        public int access_type { get; set; }
        public int creator_id { get; set; }
        public Creator creator { get; set; }
        public List<object> class_ids { get; set; }
        public List<QuizletCard> terms { get; set; }
        public string display_timestamp { get; set; }
    }
}