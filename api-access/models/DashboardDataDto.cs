using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace api_access.models
{
    public class DashboardDataDto
    {
        public int ConferencesCount { get; set; }
        public int ArticleCount { get; set; }
        public int EmployeesCount { get; set; }
        public int BooksCount { get; set; }
        public int ResearchCount { get; set; }
        public Dictionary<string, int> AuthorsCountByCity { get; set; }
        public List<Research> RecentResearches { get; set; }
        public List<Article> RecentArticles { get; set; }
        public List<Conference> RecentConferences { get; set; }
        public List<Book> RecentBooks { get; set; }
    }
}
