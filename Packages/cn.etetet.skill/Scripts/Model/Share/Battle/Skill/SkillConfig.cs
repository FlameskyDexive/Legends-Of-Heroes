using System.Collections.Generic;

namespace ET
{
    // 占位配置类：让战斗系统先编译通过。正式应通过本项目 Luban 导表生成
    // （把 Luban/Config/Datas/SkillConfig.xlsx 接入 luban.conf 后由 et-luban 生成同名 SkillConfig/SkillConfigCategory）。
    // 届时删除本文件，改用生成码。
    [EnableClass]
    public sealed class SkillConfig
    {
        public int Id;
        public int Level;
        public int AbstractType;
        public string Name = string.Empty;
        public string Desc = string.Empty;
        public int Life;
        public int CD;
        public List<int> ActionEventIds = new();
        public List<int> ActionEventTriggerPercent = new();
    }

    [AllowInstance]
    public class SkillConfigCategory : Singleton<SkillConfigCategory>, ISingletonAwake
    {
        private readonly Dictionary<(int, int), SkillConfig> dataMap = new();

        public void Awake()
        {
        }

        public SkillConfig Get(int id, int level) => this.dataMap.TryGetValue((id, level), out SkillConfig v) ? v : null;

        public IReadOnlyDictionary<(int, int), SkillConfig> DataMap => this.dataMap;
    }
}
