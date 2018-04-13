﻿using System;

namespace Quiz.me.Dialogs
{
    [Serializable]
    public class QuizletCard
    {
        public string id { get; set; }
        public string term { get; set; }
        public string definition { get; set; }
        public string image { get; set; }
        public string rank { get; set; }
    }
}