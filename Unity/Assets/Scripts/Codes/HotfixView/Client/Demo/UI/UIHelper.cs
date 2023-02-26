namespace ET.Client
{
    public static class UIHelper
    {
        

        public static string GetIconQualityBG_1_Name(int rare)
        {
            var rareName = "Common_003";
            switch (rare)
            {
                case 2: { rareName = "Common_004"; } break;
                case 3: { rareName = "Common_005"; } break;
            }
            return rareName;
        }

        public static string GetIconQualityBG_2_Name(int rare)
        {
            var rareName = "Common_014";
            switch (rare)
            {
                case 2: { rareName = "Common_013"; } break;
                case 3: { rareName = "Common_012"; } break;
            }
            return rareName;
        }
    }
}