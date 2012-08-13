using MS.Katusha.Enumerations;

namespace MS.Katusha.Infrastructure.Exceptions.Services
{
    public class KatushaProductNameNotFoundException : KatushaException
    {
        public KatushaProductNameNotFoundException(string productName)
            : base("ProductNameNotFound", null) { ProductName = productName; }

        public string ProductName { get; private set; }
    }
}
