using System.Collections.Generic;

namespace XPlay.Runtime
{
    public class XPlayCutSceneData: XPlayDirectableData
    {
        public int id;
        public List<XPlayGroupData> GroupDatas = new List<XPlayGroupData>();
    }
}