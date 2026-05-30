namespace ET.Client
{
    // 进图完成 -> 打开战斗 HUD(DlgBattle:摇杆移动 + 吐球/分裂技能按钮)。
    // 原 YIUI 版 HUD 创建(SceneChangeFinish_CreateMainUI)已停用、无人接替,导致进图后 DlgBattle 从未被创建;
    // 这是 EUI 版替代。EnterMapFinish 在 Wait_SceneChangeFinish 之后发布,此时 CurrentScene 与本地 MyUnit 均已就绪。
    // 注:当前对所有进图地图都开 DlgBattle(球球大作战与普通战斗地图通用的技能条/摇杆 HUD);
    //     若普通 RPG 地图需要不同 HUD,可在此用 CurrentScenesComponent 的场景名做分流。
    [Event(SceneType.Client)]
    public class EnterMapFinish_CreateBattleHUD : AEvent<Scene, EnterMapFinish>
    {
        protected override async ETTask Run(Scene root, EnterMapFinish args)
        {
            // 🔑 确保 root 场景上有 OperaComponent —— DlgBattle 的摇杆(OnMove)/技能按钮(OnClickSkill1/2)
            // 通过 self.Scene().GetComponent<OperaComponent>()?.OnXxx() 上报操作,而 DlgBattle 的 Scene()==root(UI 都挂 root)。
            // OperaComponent 此前**从未被任何地方添加**,导致 GetComponent 恒为 null、?. 短路 → 摇杆/技能全无响应(本次根因)。
            // OperaComponent 用 self.Root().ClientSenderComponent 发 C2M_Operation,服务端 C2M_OperationHandler 据此移动(StraightMoveToAsync)/放技能(OperateSkillCast→SpitBall/SplitBall)。
            if (root.GetComponent<OperaComponent>() == null)
            {
                root.AddComponent<OperaComponent>();
            }

            UIComponent ui = root.GetComponent<UIComponent>();
            if (ui.IsWindowVisible(WindowID.WindowID_Battle))
            {
                return;
            }
            await ui.ShowWindowAsync(WindowID.WindowID_Battle);
        }
    }
}
