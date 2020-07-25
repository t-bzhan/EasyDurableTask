namespace EasyDurableTask.Orchestrations
{
    using DurableTask.Core;
    using EasyDurableTask.Core;
    using System.Threading.Tasks;

    public abstract class ChainingOrchestration<TResult, TInput> : TaskOrchestration<TResult, TInput>
    {
        public override async Task<TResult> RunTask(OrchestrationContext context, TInput input)
        {
            return await (GetChainedResult(context, input).ResultFunc());
        }

        public abstract ChainActivityResult<TResult> GetChainedResult(OrchestrationContext contex, TInput input);
    }
}
