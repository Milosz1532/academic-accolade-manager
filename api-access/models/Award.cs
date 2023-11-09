namespace api_access.models
{
    public class Award
    {
        public int id { get; set; }
        public string name { get; set; }
        public int author_id { get; set; }
        public int year { get; set; }
        public Author Author { get; set; }
    }
}