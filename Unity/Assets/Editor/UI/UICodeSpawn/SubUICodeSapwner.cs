﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


public partial class UICodeSpawner
{
    static public void SpawnSubUICode(GameObject go)
    {
        IsCreateItemCode  = false;
        IsCreateSubUICode = true;
        Path2WidgetCachedDict?.Clear();
        Path2WidgetCachedDict = new Dictionary<string, Component>();
        FindAllWidgets(go.transform, "");
        SpawnCodeForSubUI(go);
        SpawnCodeForSubUIBehaviour(go);
        AssetDatabase.Refresh();
    }
    
    static void SpawnCodeForSubUI(GameObject objPanel)
    {
        if (null == objPanel)
        {
            return;
        }
        string strDlgName = objPanel.name;

        string strFilePath = Application.dataPath + "/HotfixView/GameLogic/UIBehaviour/CommonUI" +
                             "";

        if ( !System.IO.Directory.Exists(strFilePath) )
        {
            System.IO.Directory.CreateDirectory(strFilePath);
        }
        strFilePath     = Application.dataPath + "/HotfixView/GameLogic/UIBehaviour/CommonUI/" + strDlgName + "ViewSystem.cs";
	    
        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine()
            .AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("namespace ET");
        strBuilder.AppendLine("{");
        strBuilder.AppendLine("\t[ObjectSystem]");
        strBuilder.AppendFormat("\tpublic class {0}AwakeSystem : AwakeSystem<{1},Transform> \r\n", strDlgName, strDlgName);
        strBuilder.AppendLine("\t{");
        strBuilder.AppendFormat("\t\tpublic override void Awake({0} self,Transform transform)\n",strDlgName);
        strBuilder.AppendLine("\t\t{");
        strBuilder.AppendLine("\t\t\tself.uiTransform = transform;");
        strBuilder.AppendLine("\t\t}");
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("\n");
        
       
        strBuilder.AppendLine("\t[ObjectSystem]");
        strBuilder.AppendFormat("\tpublic class {0}DestroySystem : DestroySystem<{1}> \r\n", strDlgName, strDlgName);
        strBuilder.AppendLine("\t{");
        strBuilder.AppendFormat("\t\tpublic override void Destroy({0} self)",strDlgName);
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
    
    static void SpawnCodeForSubUIBehaviour(GameObject objPanel)
    {
        if (null == objPanel)
        {
            return;
        }
        string strDlgName = objPanel.name;

        string strFilePath = Application.dataPath + "/ModelView/GameLogic/UIBehaviour/CommonUI";

        if ( !System.IO.Directory.Exists(strFilePath) )
        {
            System.IO.Directory.CreateDirectory(strFilePath);
        }
        strFilePath = Application.dataPath + "/ModelView/GameLogic/UIBehaviour/CommonUI/" + strDlgName + ".cs";
	    
        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine()
            .AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("namespace ET");
        strBuilder.AppendLine("{");
        strBuilder.AppendFormat("\tpublic  class {0} : Entity \r\n", strDlgName)
            .AppendLine("\t{");
        
       
        CreateWidgetBindCode(ref strBuilder, objPanel.transform);
        CreateDeclareCode(ref strBuilder);
        strBuilder.AppendLine("\t\tpublic Transform uiTransform = null;");
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");
        
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }
}