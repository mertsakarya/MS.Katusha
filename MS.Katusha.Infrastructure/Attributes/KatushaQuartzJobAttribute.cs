using System;

namespace MS.Katusha.Infrastructure.Attributes
{
    public class KatushaQuartzJobAttribute : Attribute
    {
        private string _resourceString;

        public KatushaQuartzJobAttribute() { Enabled = true; }
        public string ResourceString
        {
            get { return _resourceString; }
            set
            {
                _resourceString = value;
                var rm = new ResourceManager();
                TriggerName = value + "Trigger";
                JobName = value;
                StartTimeUtc = DateTimeOffset.UtcNow;
                EndTimeUtc = null;
                var val = rm._C(value);
                CronExpression = val;
                IntervalSeconds = 0;
                val = rm._C(value + ".Enabled");
                Enabled = val == null || String.IsNullOrWhiteSpace(val) || val.ToLowerInvariant() == "true";
            }
        }

        public string TriggerName { get; set; }
        public string TriggerGroup { get; set; }
        public string JobName { get; set; }
        public string JobGroup { get; set; }
        public DateTimeOffset StartTimeUtc { get; set; }
        public DateTimeOffset? EndTimeUtc { get; set; }
        public string CronExpression { get; set; }
        public int IntervalSeconds { get; set; }
        public bool Enabled { get; set; }
    }
}
