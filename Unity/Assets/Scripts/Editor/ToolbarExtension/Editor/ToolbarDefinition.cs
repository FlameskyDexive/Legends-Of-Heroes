using System;

namespace ToolbarExtension
{
    public enum OnGUISide : byte
    {
        Left,
        Right,
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class ToolbarAttribute : Attribute
    {
        public OnGUISide Side { get; }
        public int Priority { get; }

        public ToolbarAttribute(OnGUISide side, int priority)
        {
            Side = side;
            Priority = priority;
        }
    }
}