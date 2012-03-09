using MS.Katusha.Web.Models.Entities;
using PagedList;

namespace MS.Katusha.Web.Models
{
    public abstract class PagedListModel<T>
    {
        public IPagedList<T> List { get; set; }
    }

    public class GirlsIndexModel : PagedListModel<GirlModel>
    {

    }
}