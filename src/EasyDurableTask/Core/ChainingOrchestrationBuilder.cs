namespace EasyDurableTask.Core
{
    using DurableTask.Core;
    using System.Threading.Tasks;

    public static class ChainingOrchestrationBuilder
    {
        public static ChainActivityResult<TInput> StartWith<TInput>(OrchestrationContext context, TInput input)
        {
            return new ChainActivityResult<TInput>(context)
            {
                ResultFunc = () =>
                {
                    return Task.FromResult(input);
                }
            };
        }
    }
}