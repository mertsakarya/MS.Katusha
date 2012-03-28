using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Infrastructure.Cache
{
    public class CacheObject : BaseModel
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}
