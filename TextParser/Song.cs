using System;
using System.Collections.Generic;
using System.Text;

namespace TextParser
{
    public class Song
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public long Duration { get; set; }
        public bool IsPopular { get; set; }
        public decimal Rating { get; set; } // 1 - 5
        public List<string> KeyWords { get; set; }
    }
}
