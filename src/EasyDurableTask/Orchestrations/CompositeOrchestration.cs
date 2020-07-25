namespace EasyDurableTask.Orchestrations
{
    using DurableTask.Core;
    using EasyDurableTask.Core;
    using System.Threading.Tasks;

    public abstract class CompositeOrchestration<TResult, TInput> : TaskOrchestration<TResult, TInput>
    {
        public override async Task<TResult> RunTask(OrchestrationContext context, TInput input)
        {
            return await (GetCompositeResult(context, input).ResultFunc());
        }

        public abstract ChainOrchestrationResult<TResult> GetCompositeResult(OrchestrationContext contex, TInput input);
    }
}