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
            // 生成的 ActionEventConfig.ActionEventType 为 int(Luban 表按 int 建), 强转回枚举(枚举→枚举亦安全)
            self.ActionEventType = (EActionEventType)self.ActionEventConfig.ActionEventType;
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
