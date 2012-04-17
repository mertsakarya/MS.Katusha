using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using MS.Katusha.Infrastructure.Attributes;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Raven.Client;

namespace MS.Katusha.Jobs
{
    public static class QuartzHelper
    {

        public static void RegisterQuartz(IDocumentStore ravenStore)
        {
            DependencyHelper.RegisterDependencies(ravenStore);

            var properties = new NameValueCollection {
                                                         {"quartz.scheduler.jobFactory.type", "MS.Katusha.Jobs.AutoMapperJobFactory, MS.Katusha.Jobs"},
                                                         {"quartz.scheduler.instanceName", "RemoteServer"},
                                                         {"quartz.threadPool.type", "Quartz.Simpl.SimpleThreadPool, Quartz"},
                                                         {"quartz.threadPool.threadCount", "5"},
                                                         {"quartz.threadPool.threadPriority", "Normal"}
                                                     };

            var sf = new StdSchedulerFactory(properties);
            var sched = sf.GetScheduler();

            var jobs = from type in Assembly.GetExecutingAssembly().GetTypes()
                       where typeof(IJob).IsAssignableFrom(type)
                       select type;
            foreach (var type in jobs) {
                var attributes = Attribute.GetCustomAttributes(type);
                foreach (var attribute in attributes) {
                    if (!(attribute is KatushaQuartzJobAttribute)) continue;
                    var quartzJobAttribute = attribute as KatushaQuartzJobAttribute;
                    if (!quartzJobAttribute.Enabled) continue;
                    var trigger = GetTrigger(quartzJobAttribute);
                    if (trigger == null) continue;
                    IJobDetail jobDetail = new JobDetailImpl(quartzJobAttribute.JobName, quartzJobAttribute.JobGroup, type);
                    sched.ScheduleJob(jobDetail, trigger);
                }
            }
            sched.Start();
        }

        private static ITrigger GetTrigger(KatushaQuartzJobAttribute quartzJobAttribute)
        {
            ITrigger trigger;
            try {
                if (quartzJobAttribute.IntervalSeconds != 0) {
                    trigger = new SimpleTriggerImpl(quartzJobAttribute.TriggerName, quartzJobAttribute.TriggerGroup, DateTime.UtcNow, null, int.MaxValue, new TimeSpan(0, 0, 0, quartzJobAttribute.IntervalSeconds));
                } else {
                    trigger = new CronTriggerImpl(
                        quartzJobAttribute.TriggerName,
                        quartzJobAttribute.TriggerGroup,
                        quartzJobAttribute.JobName,
                        quartzJobAttribute.JobGroup,
                        quartzJobAttribute.StartTimeUtc,
                        quartzJobAttribute.EndTimeUtc,
                        quartzJobAttribute.CronExpression
                        );
                }
            } catch {
                trigger = null;
            }
            return trigger;
        }
    }
}