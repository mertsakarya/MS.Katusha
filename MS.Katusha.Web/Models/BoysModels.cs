using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Models
{
    public class BoysIndexModel : PagedListModel<BoyModel>
    {
    }

    public class BaseBoysModel
    {
        public BoyModel Boy { get; set; }
    }

    public class BoysEditModel : BaseBoysModel
    {
    }

    public class BoysDetailsModel : BaseBoysModel
    {
    }

    public class BoysCreateModel : BaseBoysModel
    {
    }

}