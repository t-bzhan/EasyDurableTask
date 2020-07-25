namespace EasyDurableTask.Orchestrations
{
    using DurableTask.Core;
    using EasyDurableTask.Core;
    using System;
    using System.Threading.Tasks;

    public abstract class MonitoringOrchestration<TActivities, TResult, TInput> : TaskOrchestration<TResult, TInput> where TActivities : class, IActivity
    {
        private static TimeSpan MaximumClockSkew = TimeSpan.FromSeconds(30);
        private static TimeSpan DefaulfReshInterval = TimeSpan.FromSeconds(30);

        public override async Task<TResult> RunTask(OrchestrationContext context, TInput input)
        {
            var expiration = GetExpirationTime(input);
            while (!IsCompleted(input))
            {
                if (expiration + MaximumClockSkew <= context.CurrentUtcDateTime)
                {
                    return GetTimeoutResult(input);
                }

                TimeSpan refreshInterval = GetRefreshInterval(input);
                if (refreshInterval == TimeSpan.Zero)
                {
                    refreshInterval = DefaulfReshInterval;
                }

                await context.CreateTimer<object>(context.CurrentUtcDateTime.Add(refreshInterval), null);
                var activities = context.CreateClient<TActivities>();
                input = await this.RefreshInput(activities, input);
            }

            return GetResult(input);
        }

        public abstract DateTimeOffset GetExpirationTime(TInput input);

        public abstract TResult GetTimeoutResult(TInput input);

        public abstract TResult GetResult(TInput input);

        public abstract bool IsCompleted(TInput input);

        public abstract TimeSpan GetRefreshInterval(TInput input);

        public abstract Task<TInput> RefreshInput(TActivities activities, TInput input);
    }
}
