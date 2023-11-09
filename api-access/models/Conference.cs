using System;
using System.Collections.Generic;

namespace api_access.models
{
    public class Conference
    {
        public int id { get; set; }
        public string title { get; set; }
        public string address { get; set; }
        public DateTime date { get; set; }
        public string organization { get; set; }
        public string note { get; set; }
        public byte active_participation { get; set; }

        public ICollection<Author__Conference> Author__Conference { get; set; }
    }
}