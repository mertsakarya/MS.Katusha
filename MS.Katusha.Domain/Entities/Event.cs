using System;
using MS.Katusha.Domain.Entities.BaseEntities;
using Newtonsoft.Json;

namespace MS.Katusha.Domain.Entities
{
    public class Event : BaseGuidModel
    {
        public long ProfileId { get; set; }

        public long SubjectId { get; set; }

        public Enumerations.Action Action { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | ProfileId: {0} | SubjectId: {1} | Action: {2} | Description: [\r\n{3}\r\n]", ProfileId, SubjectId, Action, Description);
        }
    }
}