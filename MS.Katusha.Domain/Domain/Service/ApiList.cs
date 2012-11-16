using System.Collections.Generic;

namespace MS.Katusha.Domain.Service
{
    public class ApiList<T>
    {
        public int Total { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public IList<T> Items { get; set; }
    }
}