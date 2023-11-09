using System;

namespace api_access.models
{
    public class Editor
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public DateTime born { get; set; }
    }
}