namespace MS.Katusha.Domain.Entities
{
    public class ProductExecutionData
    {
        public int Value { get; set; }
        public byte TimeFrame { get; set; }
        public bool Recurring { get; set; }
        public byte Membership { get; set; }
    }
}