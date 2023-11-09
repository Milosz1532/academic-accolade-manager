using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_access.models
{
    public class Author__Title
    {
        public int id { get; set; }
        public int author_id { get; set; }
        public int title_id { get; set; }

        public DateTime recieved_date { get; set; }


        public TitleAndDegree TitleAndDegree { get; set; }
    }
}
