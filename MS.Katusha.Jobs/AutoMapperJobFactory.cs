using System;
using Autofac;
using Quartz;
using Quartz.Spi;

namespace MS.Katusha.Jobs
{
    public class AutoMapperJobFactory : IJobFactory
    {
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try {
                using (var lifetimeScope = DependencyHelper.Container.BeginLifetimeScope()) {
                    var job = (IJob)lifetimeScope.Resolve(bundle.JobDetail.JobType);
                    return job;
                }
            } catch (Exception e) {
                var se = new SchedulerException("Problem instantiating class", e);
                throw se;
            }
        }
    }
}