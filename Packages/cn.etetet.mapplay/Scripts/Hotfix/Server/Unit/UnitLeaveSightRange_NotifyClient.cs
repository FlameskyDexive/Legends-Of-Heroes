namespace ET.Server
{
    // 离开视野
    [Event(SceneType.Map)]
    public class UnitLeaveSightRange_NotifyClient: AEvent<Scene, UnitLeaveSightRange>
    {
        protected override async ETTask Run(Scene scene, UnitLeaveSightRange args)
        {
            Unit a = args.A;
            if (a == null || a.UnitType != UnitType.Player)
            {
                return;
            }

            M2C_RemoveUnits removeUnits = M2C_RemoveUnits.Create();
            removeUnits.Units.Add(args.BId);
            MapMessageHelper.NoticeClient(a, removeUnits, NoticeType.Self);
            
            await ETTask.CompletedTask;
        }
    }
}