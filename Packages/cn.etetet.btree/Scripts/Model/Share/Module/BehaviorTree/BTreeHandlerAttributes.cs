namespace ET
{
    public abstract class BTreeHandlerAttribute : BaseAttribute
    {
        public readonly string Name;

        protected BTreeHandlerAttribute(string name)
        {
            this.Name = name;
        }
    }

    public sealed class BTreeActionHandlerAttribute : BTreeHandlerAttribute
    {
        public BTreeActionHandlerAttribute(string name = "") : base(name)
        {
        }
    }

    public sealed class BTreeConditionHandlerAttribute : BTreeHandlerAttribute
    {
        public BTreeConditionHandlerAttribute(string name = "") : base(name)
        {
        }
    }

    public sealed class BTreeServiceHandlerAttribute : BTreeHandlerAttribute
    {
        public BTreeServiceHandlerAttribute(string name = "") : base(name)
        {
        }
    }

    public sealed class BTreeNodeHandlerAttribute : BaseAttribute
    {
    }
}
