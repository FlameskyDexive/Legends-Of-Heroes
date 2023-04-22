namespace ET
{
    [FriendOf(typeof(Unit))]
    public static class UnitSystem
    {
        [ObjectSystem]
        public class UnitAwakeSystem : AwakeSystem<Unit, int>
        {
            protected override void Awake(Unit self, int configId)
            {
                self.ConfigId = configId;
            }
        }
        
        public static void SyncInfo(this Unit self, UnitInfo unitInfo)
        {
            self.Position = unitInfo.Position;
            self.Forward = unitInfo.Forward;
        }
    }
}