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
        strBuilder.AppendFormat("\t[EntitySystemOf(typeof({0}))]\n",strDlgName);
        strBuilder.AppendFormat("\t[FriendOfAttribute(typeof({0}))]\n",strDlgName);
        strBuilder.AppendFormat("\tpublic static partial class {0}System \r\n", strDlgName, strDlgName);
        strBuilder.AppendLine("\t{");
        strBuilder.AppendLine("\t\t[EntitySystem]");
        strBuilder.AppendFormat("\t\tprivate static void Awake(this {0} self,Transform transform)\n",strDlgName);
        strBuilder.AppendLine("\t\t{");
        strBuilder.AppendLine("\t\t\tself.uiTransform = transform;");
        strBuilder.AppendLine("\t\t}\n");
        
        
        strBuilder.AppendLine("\t\t[EntitySystem]");
        strBuilder.AppendFormat("\t\tprivate static void Destroy(this {0} self)\n",strDlgName);
        strBuilder.AppendLine("\t\t{");
        strBuilder.AppendLine("\t\t\tself.DestroyWidget();");
        strBuilder.AppendLine("\t\t}");
        
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("\n");
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
	strBuilder.AppendLine("\t[ChildOf]");
        strBuilder.AppendLine("\t[EnableMethod]");
        strBuilder.AppendFormat("\tpublic  class {0} : Entity,ET.IAwake<UnityEngine.Transform>,IDestroy \r\n", strDlgName)
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
