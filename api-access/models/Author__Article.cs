namespace api_access.models
{
    public class Author__Article
    {
        public int id { get; set; }
        public int author_id { get; set; }
        public int article_id { get; set; }
        public int participation { get; set; }

        public Author Author { get; set; }

        public Article Article { get; set; }
    }
}