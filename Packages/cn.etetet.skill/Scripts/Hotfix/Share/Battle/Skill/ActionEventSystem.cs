namespace ET
{
    [EntitySystemOf(typeof(ActionEvent))]
    [FriendOf(typeof(ActionEvent))]
    public static partial class ActionEventSystem
    {
        [EntitySystem]
        public static void Awake(this ActionEvent self, int configId, int triggerTime, EActionEventSourceType sourceType)
        {
            self.EventTriggerTime = triggerTime + self.GetSingleton<TimeInfo>().ServerNow();
            self.ConfigId = configId;
            // ActionEventConfig.ActionEventType 现为 Luban 多态表生成的 EActionEventType 枚举, 直接取用做派发 key
            self.ActionEventType = self.ActionEventConfig.ActionEventType;
            self.SourceType = sourceType;
        }

        public static void Transfer(this ActionEvent self)
        {
        }

        [EntitySystem]
        public static void Destroy(this ActionEvent self)
        {
        }
    }
}
