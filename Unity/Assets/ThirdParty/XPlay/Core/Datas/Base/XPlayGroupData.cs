using System.Collections.Generic;

namespace XPlay.Runtime
{
    public class XPlayGroupData: XPlayDirectableData
    {
        public int id;
        public string groupName = "";
        public List<XPlayTrackData> TrackDatas = new List<XPlayTrackData>();
    }
}