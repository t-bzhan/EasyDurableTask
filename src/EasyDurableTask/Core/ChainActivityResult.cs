namespace EasyDurableTask.Core
{
    using DurableTask.Core;
    using System;
    using System.Threading.Tasks;

    public class ChainActivityResult<TResult>
    {
        public Func<Task<TResult>> ResultFunc;

        private OrchestrationContext context;

        public ChainActivityResult(OrchestrationContext context)
        {
            this.context = context;
        }

        public ChainActivityResult<TOut> OnNext<TOut, TActivity>(Func<TResult, TActivity, Task<TOut>> activityFunc, TActivity activity) where TActivity : IActivity
        {
            return new ChainActivityResult<TOut>(context)
            {
                ResultFunc = async () =>
                {
                    var input = await ResultFunc();
                    return await activityFunc(input, activity);
                }
            };
        }

        public ChainActivityResult<TOut> OnNext<TOut, TActivity>() where TActivity : AsyncTaskActivity<TResult, TOut>
        {
            return new ChainActivityResult<TOut>(context)
            {
                ResultFunc = async () =>
                {
                    var input = await ResultFunc();
                    return await context.ScheduleTask<TOut>(typeof(TActivity), input);
                }
            };
        }

        public ChainActivityResult<TResult> Delay(TimeSpan interval)
        {
            return new ChainActivityResult<TResult>(context)
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
