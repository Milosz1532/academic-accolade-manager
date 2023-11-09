namespace api_access.models
{
    public class Author__Book
    {
        public int id { get; set; }
        public int author_id { get; set; }
        public int book_id { get; set; }
        public int participation { get; set; }

        public Author Author { get; set; }

        public Book Book { get; set; }
    }
}