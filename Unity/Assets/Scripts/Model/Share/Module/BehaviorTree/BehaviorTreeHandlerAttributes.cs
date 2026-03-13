namespace ET
{
    public abstract class BehaviorTreeHandlerAttribute : BaseAttribute
    {
        public readonly string Name;

        protected BehaviorTreeHandlerAttribute(string name)
        {
            this.Name = name;
        }
    }

    public sealed class BehaviorTreeActionHandlerAttribute : BehaviorTreeHandlerAttribute
    {
        public BehaviorTreeActionHandlerAttribute(string name = "") : base(name)
        {
        }
    }

    public sealed class BehaviorTreeConditionHandlerAttribute : BehaviorTreeHandlerAttribute
    {
        public BehaviorTreeConditionHandlerAttribute(string name = "") : base(name)
        {
        }
    }

    public sealed class BehaviorTreeServiceHandlerAttribute : BehaviorTreeHandlerAttribute
    {
        public BehaviorTreeServiceHandlerAttribute(string name = "") : base(name)
        {
        }
    }
}
