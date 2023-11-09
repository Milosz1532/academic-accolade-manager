using System;
using System.Collections.Generic;

namespace api_access.models
{
    public class Author
    {
        public int id { get; set; }
        public string first_Name { get; set; }
        public string last_Name { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public DateTime born { get; set; }
        public int department_id { get; set; }
        public string orcid { get; set; }

        public Department Department { get; set; }

        public ICollection<Author__Article> Author__Article { get; set; }
        public ICollection<Author__Book> Author__Book { get; set; }
        public ICollection<Author__Conference> Author__Conference { get; set; }

        public ICollection<Author__Research> Author__Research { get; set; }
        public ICollection<Author__Title> Author__Title { get; set; }
        public ICollection<Award> Awards { get; set; }
        public ICollection<Others> Others { get; set; }
        public ICollection<Research> Researches { get; set; }
    }
}