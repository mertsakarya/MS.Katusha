namespace MS.Katusha.Domain.Raven.Entities
{
    public class FacetData
    {
        public int Count { get; set; }

        public string Range { get; set; }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Range, Count);
        }
    }
}