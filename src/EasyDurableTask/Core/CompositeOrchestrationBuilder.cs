namespace EasyDurableTask.Core
{
    using DurableTask.Core;
    using System.Threading.Tasks;

    public static class CompositeOrchestrationBuilder
    {
        public static ChainOrchestrationResult<TInput> StartWith<TInput>(OrchestrationContext context, TInput input)
        {
            return new ChainOrchestrationResult<TInput>(context)
            {
                ResultFunc = () =>
                {
                    return Task.FromResult(input);
                }
            };
        }
    }
}