using System;

namespace Quiz.me.Dialogs
{
    [Serializable]
    public class Creator
    {
        public string username { get; set; }
        public string account_type { get; set; }
        public string profile_image { get; set; }
        public int id { get; set; }
    }
}