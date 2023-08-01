﻿
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

using System.Text;
using ET;
using UnityEngine.UI;

public partial class UICodeSpawner
{
    static public void SpawnLoopItemCode(GameObject gameObject)
    {
        Path2WidgetCachedDict?.Clear();
        Path2WidgetCachedDict = new Dictionary<string, List<Component>>();
        FindAllWidgets(gameObject.transform, "");
        SpawnCodeForScrollLoopItemBehaviour(gameObject);
        SpawnCodeForScrollLoopItemViewSystem(gameObject);
        AssetDatabase.Refresh();
    }
    
    static void SpawnCodeForScrollLoopItemViewSystem(GameObject gameObject)
    {
        if (null == gameObject)
        {
            return;
        }
        string strDlgName = gameObject.name;

        string strFilePath = Application.dataPath + "/Scripts/Codes/HotfixView/Client/GamePlay/UIItemBehaviour";

        if ( !System.IO.Directory.Exists(strFilePath) )
        {
            System.IO.Directory.CreateDirectory(strFilePath);
        }
        strFilePath     = Application.dataPath + "/Scripts/Codes/HotfixView/Client/GamePlay/UIItemBehaviour/" + strDlgName + "ViewSystem.cs";

        if (File.Exists(strFilePath))
            return;

        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine()
            .AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("namespace ET.Client");
        strBuilder.AppendLine("{");

        
        strBuilder.AppendFormat("\t[FriendOf(typeof({0}))]\r\n", strDlgName);
        strBuilder.AppendFormat("\t[EntitySystemOf(typeof({0}))]\r\n", strDlgName);

        strBuilder.AppendFormat("\tpublic static partial class {0}\r\n", strDlgName + "System");
        strBuilder.AppendLine("\t{");
        strBuilder.AppendLine("");
        

        strBuilder.AppendFormat("\t\tpublic static void Destroy(this {0} self)\n", strDlgName)
                .AppendLine("\t\t{")
                .AppendLine("\t\t ")
                .AppendFormat("\t\t\tself.DestroyWidget();\r\n")
                .AppendLine("\t\t}")
                .AppendLine();

        
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");
        
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }
    
    
    static void SpawnCodeForScrollLoopItemBehaviour(GameObject gameObject)
    {
        if (null == gameObject)
        {
            return;
        }
        string strDlgName = gameObject.name;

        string strFilePath = Application.dataPath + "/Scripts/Codes/ModelView/Client/GamePlay/UIItemBehaviour";

        if ( !System.IO.Directory.Exists(strFilePath) )
        {
            System.IO.Directory.CreateDirectory(strFilePath);
        }
        strFilePath     = Application.dataPath + "/Scripts/Codes/ModelView/Client/GamePlay/UIItemBehaviour/" + strDlgName + ".cs";
        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine()
            .AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("namespace ET.Client");
        strBuilder.AppendLine("{");
        strBuilder.AppendLine("\t[EnableMethod]");
        strBuilder.AppendFormat("\tpublic partial class Scroll_{0} : Entity,IAwake,IDestroy,IUIScrollItem \r\n", strDlgName)
            .AppendLine("\t{");
        
        strBuilder.AppendLine("\t\tpublic long DataId {get;set;}");
        
        strBuilder.AppendLine("\t\tprivate bool isCacheNode = false;");
        strBuilder.AppendLine("\t\tpublic void SetCacheMode(bool isCache)");
        strBuilder.AppendLine("\t\t{");
        strBuilder.AppendLine("\t\t\tthis.isCacheNode = isCache;");
        strBuilder.AppendLine("\t\t}\n");


        CreateScrollLoopItemBindTrans(ref strBuilder, strDlgName);
        CreateWidgetBindCode(ref strBuilder, gameObject.transform);
        CreateDestroyWidgetCode(ref strBuilder,true);
        CreateDeclareCode(ref strBuilder);
        
        strBuilder.AppendLine("\t\tpublic Transform uiTransform = null;");
        
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");
        
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }

    static void CreateScrollLoopItemBindTrans(ref StringBuilder strBuilder, string strDlgName)
    {
        strBuilder.AppendFormat("\t\tpublic Scroll_{0} BindTrans(Transform trans)\r\n", strDlgName);
        strBuilder.AppendLine("\t\t{");
        strBuilder.AppendLine("\t\t\tif (!Object.ReferenceEquals(this.uiTransform, trans) && !Object.ReferenceEquals(this.uiTransform, null))");
        strBuilder.AppendLine("\t\t\t{");

        string pointStr = /*false ? "self" :*/ "this";
        foreach (KeyValuePair<string, List<Component>> pair in Path2WidgetCachedDict)
        {
            foreach (var info in pair.Value)
            {
                Component widget = info;
                string strClassType = widget.GetType().ToString();

                if (pair.Key.StartsWith(CommonUIPrefix))
                {
                    strBuilder.AppendFormat("\t\t\t	{0}.m_{1}?.Dispose();\r\n", pointStr, pair.Key.ToLower());
                    strBuilder.AppendFormat("\t\t\t	{0}.m_{1} = null;\r\n", pointStr, pair.Key.ToLower());
                    continue;
                }
            }

        }
        strBuilder.AppendLine("\t\t\t}\n");

        strBuilder.AppendLine("\t\t\tthis.uiTransform = trans;");
        strBuilder.AppendLine("\t\t\treturn this;");
        strBuilder.AppendLine("\t\t}\n");
    }
}
