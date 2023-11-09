namespace api_access.models
{
    public class Author__Research
    {
        public int id { get; set; }
        public int author_id { get; set; }
        public int research_id { get; set; }

        public Author Author { get; set; }
        public Research Research { get; set; }
    }
}
