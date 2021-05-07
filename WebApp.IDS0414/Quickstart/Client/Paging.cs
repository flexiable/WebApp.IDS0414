using System.Collections.Generic;

namespace WebApp.IDS0414.Controllers
{
    public class Paging<T> where T:class
    {
        public int PageSize { get; set; } = 20;

        public int PageNum { get; set; } = 1;

        public string SearchText { get; set; }
        public string SortName { get; set; }
        public string SortOrder { get; set; }

        public T Filter { get; set; }
    }

    public class ResponsePaging<T> where T : class
    {
        public int total { get; set; } = 0;

    

        public List<T> rows { get; set; }
    }
}