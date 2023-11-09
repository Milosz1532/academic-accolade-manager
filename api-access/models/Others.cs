namespace api_access.models
{
    public class Others
    {
        public int id { get; set; }
        public string note { get; set; }
        public int author_id { get; set; }
        public Author Author { get; set; }
    }
}