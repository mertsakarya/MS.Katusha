namespace MS.Katusha.Domain.Raven.Entities
{
    public class VisitorsSinceLastVisitResult
    {
        public long ProfileId { get; set; }
        public long VisitorProfileId { get; set; }
        public int Count { get; set; }
    }
}