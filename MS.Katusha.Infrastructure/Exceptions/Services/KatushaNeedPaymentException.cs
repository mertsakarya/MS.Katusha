using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Infrastructure.Exceptions.Services
{
    public class KatushaNeedsPaymentException : KatushaException
    {
        public KatushaNeedsPaymentException(User user, ProductNames product)
            : base("NeedsPayment", "Katusha needs payment, sorry", null)
        {
            User = user;
            Product = product;
        }

        public User User { get; private set; }
        public ProductNames Product { get; private set; }
    }
}
