
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

using System.Text;
using ET;

public partial class UICodeSpawner
{
    static public void SpawnLoopItemCode(GameObject go)
    {
        Path2WidgetCachedDict?.Clear();
        Path2WidgetCachedDict = new Dictionary<string, Component>();
        FindAllWidgets(go.transform, "");
        SpawnCodeForScrollLoopItemBehaviour(go);
        SpawnCodeForScrollLoopItemViewSystem(go);
        AssetDatabase.Refresh();
    }
    
    static void SpawnCodeForScrollLoopItemViewSystem(GameObject gameoBject)
    {
        if (null == gameoBject)
        {
            return;
        }
        string strDlgName = gameoBject.name;

        string strFilePath = Application.dataPath + "/../Codes/HotfixView/GameLogic/UIItemBehaviour";

        if ( !System.IO.Directory.Exists(strFilePath) )
        {
            System.IO.Directory.CreateDirectory(strFilePath);
        }
        strFilePath     = Application.dataPath + "/../Codes/HotfixView/GameLogic/UIItemBehaviour/" + strDlgName + "ViewSystem.cs";
        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine()
            .AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("namespace ET");
        strBuilder.AppendLine("{");
        
        
        strBuilder.AppendLine("\t[ObjectSystem]");
        strBuilder.AppendFormat("\tpublic class Scroll_{0}DestroySystem : DestroySystem<Scroll_{1}> \r\n", strDlgName, strDlgName);
        strBuilder.AppendLine("\t{");
        strBuilder.AppendFormat("\t\tpublic override void Destroy( Scroll_{0} self )",strDlgName);
        strBuilder.AppendLine("\n\t\t{");
        
        CreateDlgWidgetDisposeCode(ref strBuilder,true);
        strBuilder.AppendFormat("\t\t\tself.uiTransform = null;\r\n");

        strBuilder.AppendLine("\t\t}");
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");
        
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }
    
    
    static void SpawnCodeForScrollLoopItemBehaviour(GameObject gameoBject)
    {
        if (null == gameoBject)
        {
            return;
        }
        string strDlgName = gameoBject.name;

        string strFilePath = Application.dataPath + "/../Codes/ModelView/GameLogic/UIItemBehaviour";

        if ( !System.IO.Directory.Exists(strFilePath) )
        {
            System.IO.Directory.CreateDirectory(strFilePath);
        }
        strFilePath     = Application.dataPath + "/../Codes/ModelView/GameLogic/UIItemBehaviour/" + strDlgName + ".cs";
        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine()
            .AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("namespace ET");
        strBuilder.AppendLine("{");
        strBuilder.AppendFormat("\tpublic  class Scroll_{0} : Entity \r\n", strDlgName)
            .AppendLine("\t{");
        
        
        strBuilder.AppendLine("\t\tprivate bool isCacheNode = true;");
        strBuilder.AppendLine("\t\tpublic void SetCacheMode(bool isCache)");
        strBuilder.AppendLine("\t\t{");
        strBuilder.AppendLine("\t\t\tthis.isCacheNode = isCache;");
        strBuilder.AppendLine("\t\t}\n");
        strBuilder.AppendFormat("\t\tpublic Scroll_{0} BindTrans(Transform trans)\r\n",strDlgName);
        strBuilder.AppendLine("\t\t{");
        strBuilder.AppendLine("\t\t\tthis.uiTransform = trans;");
        strBuilder.AppendLine("\t\t\treturn this;");
        strBuilder.AppendLine("\t\t}\n");
        
        CreateWidgetBindCode(ref strBuilder, gameoBject.transform);
        
        CreateDeclareCode(ref strBuilder);
        
        strBuilder.AppendLine("\t\tpublic Transform uiTransform = null;");
        
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");
        
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }

}
