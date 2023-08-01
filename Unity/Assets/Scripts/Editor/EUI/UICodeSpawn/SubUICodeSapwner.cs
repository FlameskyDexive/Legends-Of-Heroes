using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


public partial class UICodeSpawner
{
    static public void SpawnSubUICode(GameObject gameObject)
    {
      
        Path2WidgetCachedDict?.Clear();
        Path2WidgetCachedDict = new Dictionary<string, List<Component>>();
        FindAllWidgets(gameObject.transform, "");
        SpawnCodeForSubUI(gameObject);
        SpawnCodeForSubUIBehaviour(gameObject);
        AssetDatabase.Refresh();
    }
    
    static void SpawnCodeForSubUI(GameObject objPanel)
    {
        if (null == objPanel)
        {
            return;
        }
        string strDlgName = objPanel.name;

        string strFilePath = Application.dataPath + "/Scripts/HotfixView/Client/Demo/UIBehaviour/CommonUI" +
                             "";

        if ( !System.IO.Directory.Exists(strFilePath) )
        {
            System.IO.Directory.CreateDirectory(strFilePath);
        }
        strFilePath     = Application.dataPath + "/Scripts/HotfixView/Client/Demo/UIBehaviour/CommonUI/" + strDlgName + "ViewSystem.cs";
	    
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


        strBuilder.AppendFormat("\t\tpublic static void Awake(this {0} self)\n", strDlgName)
                .AppendLine("\t\t{")
                .AppendLine("\t\t ")
                .AppendLine("\t\t\tself.uiTransform = transform;")
                .AppendLine("\t\t}")
                .AppendLine();

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
    
    static void SpawnCodeForSubUIBehaviour(GameObject objPanel)
    {
        if (null == objPanel)
        {
            return;
        }
        string strDlgName = objPanel.name;

        string strFilePath = Application.dataPath + "/Scripts/ModelView/Client/Demo/UIBehaviour/CommonUI";

        if ( !System.IO.Directory.Exists(strFilePath) )
        {
            System.IO.Directory.CreateDirectory(strFilePath);
        }
        strFilePath = Application.dataPath + "/Scripts/ModelView/Client/Demo/UIBehaviour/CommonUI/" + strDlgName + ".cs";
	    
        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine()
            .AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("namespace ET.Client");
        strBuilder.AppendLine("{");
        strBuilder.AppendLine("\t[EnableMethod]");
        strBuilder.AppendFormat("\tpublic partial class {0} : Entity,ET.IAwake<UnityEngine.Transform>,IDestroy \r\n", strDlgName)
            .AppendLine("\t{");
        
       
        CreateWidgetBindCode(ref strBuilder, objPanel.transform);
        CreateDestroyWidgetCode(ref strBuilder);
        CreateDeclareCode(ref strBuilder);
        strBuilder.AppendLine("\t\tpublic Transform uiTransform = null;");
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");
        
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }
}
