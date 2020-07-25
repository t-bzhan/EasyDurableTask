namespace EasyDurableTask.Orchestrations
{
    using DurableTask.Core;
    using EasyDurableTask.Core;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class FanOutFanInOrchestration<TActivity, TResult, TInput, TIntermediateIn, TIntermediateOut> : TaskOrchestration<TResult, TInput> where TActivity: class, IActivity
    {
        public override async Task<TResult> RunTask(OrchestrationContext context, TInput input)
        {
            var activities = context.CreateClient<TActivity>();
            var intermediates = await FanOut(input, activities);

            List<Task<TIntermediateOut>> intermediateTasks = new List<Task<TIntermediateOut>>();
            foreach (var intermediate in intermediates)
            {
                intermediateTasks.Add(ExecuteSubTasks(intermediate, activities));
            }
            
            var intermediateResults = await Task.WhenAll(intermediateTasks);

            return Aggregate(intermediateResults);
        }

        public abstract Task<IEnumerable<TIntermediateIn>> FanOut(TInput input, TActivity activity);

        public abstract TResult Aggregate(IEnumerable<TIntermediateOut> intermidates);

        public abstract Task<TIntermediateOut> ExecuteSubTasks(TIntermediateIn intermidate, TActivity activities);
    }
}