using System.Collections.Generic;

namespace api_access.models
{
    public class Book
    {
        public int id { get; set; }
        public string title { get; set; }
        public int? publisher_id { get; set; }
        public string year { get; set; }
        public string volume { get; set; }
        public int? number { get; set; }
        public string series { get; set; }
        public string address { get; set; }
        public string edition { get; set; }
        public int? month { get; set; }
        public string isbn_hardcover { get; set; }
        public string isbn_softcover { get; set; }
        public string isbn_ebook { get; set; }
        public string note { get; set; }

        public ICollection<Author__Book> Author__Book { get; set; }

        public ICollection<Book__Editor> Book__Editor { get; set; }

        public Publisher Publisher { get; set; }
    }
}