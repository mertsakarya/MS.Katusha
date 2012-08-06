using MS.Katusha.Enumerations;

namespace MS.Katusha.Infrastructure.Exceptions.Services
{
    public class KatushaNeedsPaymentButUserEmptyException : KatushaException
    {
        public KatushaNeedsPaymentButUserEmptyException(ProductNames product)
            : base("NeedsPayment", "Katusha needs payment, sorry", null)
        {
            Product = product;
        }

        public ProductNames Product { get; private set; }
    }
}