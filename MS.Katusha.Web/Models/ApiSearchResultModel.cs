using System.Collections.Generic;

namespace MS.Katusha.Web.Models
{
    public class ApiSearchResultModel
    {
        public IList<ApiProfileInfo> Profiles { get; set; }
        public int Total { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}