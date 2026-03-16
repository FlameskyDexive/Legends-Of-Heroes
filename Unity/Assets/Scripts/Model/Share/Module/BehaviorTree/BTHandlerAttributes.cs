namespace ET
{
    public abstract class BTHandlerAttribute : BaseAttribute
    {
        public readonly string Name;

        protected BTHandlerAttribute(string name)
        {
            this.Name = name;
        }
    }

    public sealed class BTActionHandlerAttribute : BTHandlerAttribute
    {
        public BTActionHandlerAttribute(string name = "") : base(name)
        {
        }
    }

    public sealed class BTConditionHandlerAttribute : BTHandlerAttribute
    {
        public BTConditionHandlerAttribute(string name = "") : base(name)
        {
        }
    }

    public sealed class BTServiceHandlerAttribute : BTHandlerAttribute
    {
        public BTServiceHandlerAttribute(string name = "") : base(name)
        {
        }
    }

    public sealed class BTNodeHandlerAttribute : BaseAttribute
    {
    }
}
