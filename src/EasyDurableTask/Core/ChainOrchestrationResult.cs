namespace EasyDurableTask.Core
{
    using DurableTask.Core;
    using System;
    using System.Threading.Tasks;

    public class ChainOrchestrationResult<TResult>
    {
        public Func<Task<TResult>> ResultFunc;

        private OrchestrationContext context;

        public ChainOrchestrationResult(OrchestrationContext context)
        {
            this.context = context;
        }

        public ChainOrchestrationResult<TOut> OnNext<TOut, TOrchestration>() where TOrchestration : TaskOrchestration<TOut, TResult>
        {
            return new ChainOrchestrationResult<TOut>(context)
            {
                ResultFunc = async () =>
                {
                    var input = await ResultFunc();
                    return await context.CreateSubOrchestrationInstance<TOut>(typeof(TOrchestration), input);
                }
            };
        }

        public ChainOrchestrationResult<TResult> Delay(TimeSpan interval)
        {
            return new ChainOrchestrationResult<TResult>(context)
            {
                ResultFunc = async () =>
                {
                    var input = await ResultFunc();
                    return await context.CreateTimer(context.CurrentUtcDateTime.Add(interval), input);
                }
            };
        }
    }
}
