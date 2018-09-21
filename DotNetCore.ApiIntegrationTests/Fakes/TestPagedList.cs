using System.Collections.Generic;

namespace DotNetCore.ApiIntegrationTests.Fakes
{
    public class TestPagedList<T> where T : class
    {
        public IEnumerable<T> Items
        {
            get;
            set;
        }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;
    }
}