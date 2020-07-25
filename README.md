# EasyDurableTask
EasyDurableTask is created to ease usage of durabletask (https://github.com/Azure/durabletask)

This is only a prototype work and be caution to use it in your producation.

We want to reduce users' mind burden of writing something that is not expected by the framework.
It has extracted common patterns as orchestrations:

    1. Activity Chain

    2. Fan out/Fan in

    3. Monitoring pattern

    4. Compensate

And orchestrations could be composed together to form complex workflows.

## Chained orchestration
```
    public class DummyChainingOrchestration : ChainingOrchestration<OperationState, string>
    {
        public override ChainResult<OperationState> GetChainedResult(OrchestrationContext context, string input)
        {
            return ChainingOrchestrationBuilder.StartWith(context, input)
                                               .Delay(TimeSpan.FromSeconds(30))
                                               .OnNext<OperationState, DummyActivity>();
        }
    }
```

## Composite orchestration:
```
    public class DummyCompositeOrchestration : CompositeOrchestration<int, string>
    {
        public override ChainOrchestrationResult<int> GetCompositeResult(OrchestrationContext context, string input)
        {
            return CompositeOrchestrationBuilder.StartWith(context, input)
                                              .Delay(TimeSpan.FromSeconds(30))
                                              .OnNext<int, DummyChainingOrchestration1>()
                                              .OnNext<int, DummyFanOutFanInOrchestration>();
        }
    }
```
