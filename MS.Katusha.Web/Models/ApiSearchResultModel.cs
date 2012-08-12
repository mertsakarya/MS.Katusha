using System;
using System.Collections.Generic;

namespace MS.Katusha.Web.Models.Entities
{
    public class ApiSearchResultModel
    {
        public List<Guid> Profiles { get; set; }
        public int Total { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}