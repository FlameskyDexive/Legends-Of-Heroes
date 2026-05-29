namespace ET
{
    /// <summary>
    /// 一对相交的碰撞体在 <see cref="CollisionWorld3D"/> 中的下标。约定 A &lt; B。
    /// </summary>
    public readonly struct CollisionPair
    {
        public readonly int A;
        public readonly int B;

        public CollisionPair(int a, int b)
        {
            this.A = a;
            this.B = b;
        }
    }
}
