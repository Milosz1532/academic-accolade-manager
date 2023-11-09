using System;
using System.Collections.Generic;

namespace api_access.models
{
    public class Research
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime date_start { get; set; }
        public DateTime date_end { get; set; }
        public string function_in_a_team { get; set; }
        public string source_of_funding { get; set; }
        public ICollection<Author__Research> Author__Research { get; set; }
    }
}