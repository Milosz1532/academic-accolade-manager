using System.Collections.Generic;

namespace api_access.models
{
    public class Article
    {
        public int id { get; set; }
        public string title { get; set; }
        public int year { get; set; }
        public string volume { get; set; }
        public int number { get; set; }
        public int page_start { get; set; }
        public int page_end { get; set; }
        public int month { get; set; }
        public string issn { get; set; }
        public string eissn { get; set; }
        public string note { get; set; }
        public string name { get; set; }

        public ICollection<Author__Article> Author__Article { get; set; }
    }
}