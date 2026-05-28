using System.Collections.Generic;

namespace ET
{
    // 占位配置类：让战斗系统先编译通过。正式应通过本项目 Luban 导表生成
    // （把 Luban/Config/Datas/ActionEventConfig.xlsx 接入 luban.conf 后由 et-luban 生成同名类）。届时删除本文件。
    [EnableClass]
    public sealed class ActionEventConfig
    {
        public int Id;
        public string Name = string.Empty;
        public string Desc = string.Empty;
        public EActionEventType ActionEventType;
        public bool IsClientOnly;
        public List<int> Params = new();
    }

    [AllowInstance]
    public class ActionEventConfigCategory : Singleton<ActionEventConfigCategory>, ISingletonAwake
    {
        private readonly Dictionary<int, ActionEventConfig> dataMap = new();

        public void Awake()
        {
        }

        public ActionEventConfig Get(int id) => this.dataMap.TryGetValue(id, out ActionEventConfig v) ? v : null;

        public IReadOnlyDictionary<int, ActionEventConfig> DataMap => this.dataMap;
    }
}
