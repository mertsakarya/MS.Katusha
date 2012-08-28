using System.Collections.Generic;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Domain.Entities
{
    public class CheckoutDetailResult
    {
        public IList<string> Errors { get; set; }
        public CheckoutStatus CheckoutStatus { get; set; }
        public ProductNames ProductName { get; set; }
        public string PayerId { get; set; }
        public string Token { get; set; }
        public User User { get; set; }
        public string Referrer { get; set; }
    }
}