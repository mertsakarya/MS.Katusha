using PagedList;

namespace MS.Katusha.Web.Models
{
    public class PagedListModel<T>
    {
        public IPagedList<T> List { get; set; }
    }
}