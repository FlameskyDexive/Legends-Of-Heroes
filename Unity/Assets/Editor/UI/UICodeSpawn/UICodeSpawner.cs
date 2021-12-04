using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

using ET;

public partial class UICodeSpawner
{
	static public void SpawnEUICode(GameObject go)
	{
		if (null == go)
		{
			Debug.LogError("UICode Select GameObject is null!");
			return;
		}
            
		string uiName = go.name;
		if (uiName.StartsWith("Dlg"))
		{
			Debug.LogWarning($"开始生成Dlg: {uiName}");
			SpawnDlgCode(go);
			return;
		}
		else if(uiName.StartsWith("ES"))
		{
			Debug.LogWarning($"开始生成子UI: {uiName}");
			SpawnSubUICode(go);
			return;
		}
		else if (uiName.StartsWith("Item_"))
		{
			Debug.LogWarning($"开始生成滚动列表项: {uiName}");
			SpawnLoopItemCode(go);
			return;
		}
		
		Debug.LogError($"选择的预设物不属于 Dlg, 子UI，滚动列表项，请检查 {uiName}！！！！！！");
	}
	
	
    static public void SpawnDlgCode(GameObject go)
    {
	    Path2WidgetCachedDict?.Clear();
        Path2WidgetCachedDict = new Dictionary<string, Component>();
        
		FindAllWidgets(go.transform, "");
		
        SpawnCodeForDlg(go);
        SpawnCodeForDlgEventHandle(go);
        SpawnCodeForDlgModel(go);
        
        SpawnCodeForDlgBehaviour(go);
        SpawnCodeForDlgComponentBehaviour(go);
        
        AssetDatabase.Refresh();
    }
    
    static void SpawnCodeForDlg(GameObject gameObject)
    {
        string strDlgName  = gameObject.name;
        string strFilePath = Application.dataPath + "/../Codes/HotfixView/GameLogic/UI/" + strDlgName ;
        
        
        if ( !System.IO.Directory.Exists(strFilePath) )
        {
	        System.IO.Directory.CreateDirectory(strFilePath);
        }
        
	    strFilePath = Application.dataPath + "/../Codes/HotfixView/GameLogic/UI/" + strDlgName + "/" + strDlgName + "System.cs";
        if(System.IO.File.Exists(strFilePath))
        {
            Debug.LogError("已存在 " + strDlgName + "System.cs,将不会再次生成。");
            return;
        }

        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine("using System.Collections;")
                  .AppendLine("using System.Collections.Generic;")
                  .AppendLine("using System;")
                  .AppendLine("using UnityEngine;")
                  .AppendLine("using UnityEngine.UI;\r\n");

        strBuilder.AppendLine("namespace ET");
        strBuilder.AppendLine("{");
        
        strBuilder.AppendFormat("\tpublic static  class {0}\r\n", strDlgName + "System");
          strBuilder.AppendLine("\t{");
          strBuilder.AppendLine("");


        strBuilder.AppendFormat("\t\tpublic static void RegisterUIEvent(this {0} self)\n",strDlgName)
               .AppendLine("\t\t{")
               .AppendLine("\t\t ")
               .AppendLine("\t\t}")
               .AppendLine();


        strBuilder.AppendFormat("\t\tpublic static void ShowWindow(this {0} self, Entity contextData = null)\n", strDlgName);
        strBuilder.AppendLine("\t\t{");
          
        strBuilder.AppendLine("\t\t}")
	        .AppendLine();
        
        strBuilder.AppendLine("\t\t \r\n");
        
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");

        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }
    
    
	static void SpawnCodeForDlgEventHandle(GameObject gameObject)
    {
        string strDlgName = gameObject.name;
        string strFilePath = Application.dataPath + "/../Codes/HotfixView/GameLogic/UI/" + strDlgName + "/Event" ;
        
        
        if ( !System.IO.Directory.Exists(strFilePath) )
        {
	        System.IO.Directory.CreateDirectory(strFilePath);
        }
        
	    strFilePath = Application.dataPath + "/../Codes/HotfixView/GameLogic/UI/" + strDlgName + "/Event/" + strDlgName + "EventHandler.cs";
        if(System.IO.File.Exists(strFilePath))
        {
	        Debug.LogError("已存在 " + strDlgName + ".cs,将不会再次生成。");
            return;
        }

        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
        StringBuilder strBuilder = new StringBuilder();
        
        strBuilder.AppendLine("namespace ET");
        strBuilder.AppendLine("{");

        strBuilder.AppendFormat("\t[AUIEvent(WindowID.WindowID_{0})]\n",strDlgName.Substring(3));
        strBuilder.AppendFormat("\tpublic  class {0}EventHandler : IAUIEventHandler\r\n", strDlgName);
          strBuilder.AppendLine("\t{");
          strBuilder.AppendLine("");
          
          
          strBuilder.AppendLine("\t\tpublic void OnInitWindowCoreData(UIBaseWindow uiBaseWindow)")
	          .AppendLine("\t\t{");

          strBuilder.AppendFormat("\t\t  uiBaseWindow.WindowData.windowType = UIWindowType.Normal; \r\n");
          
          strBuilder.AppendLine("\t\t}")
	          .AppendLine();
          
          strBuilder.AppendLine("\t\tpublic void OnInitComponent(UIBaseWindow uiBaseWindow)")
            		.AppendLine("\t\t{");

          strBuilder.AppendFormat("\t\t  uiBaseWindow.AddComponent<{0}ViewComponent>(); \r\n",strDlgName);
          strBuilder.AppendFormat("\t\t  uiBaseWindow.AddComponent<{0}>(); \r\n",strDlgName);
          
          strBuilder.AppendLine("\t\t}")
            .AppendLine();
          
          strBuilder.AppendLine("\t\tpublic void OnRegisterUIEvent(UIBaseWindow uiBaseWindow)")
	          .AppendLine("\t\t{");

          strBuilder.AppendFormat("\t\t  uiBaseWindow.GetComponent<{0}>().RegisterUIEvent(); \r\n",strDlgName);
          
          strBuilder.AppendLine("\t\t}")
	          .AppendLine();
          
          
          strBuilder.AppendLine("\t\tpublic void OnShowWindow(UIBaseWindow uiBaseWindow, Entity contextData = null)")
	          .AppendLine("\t\t{");
          strBuilder.AppendFormat("\t\t  uiBaseWindow.GetComponent<{0}>().ShowWindow(contextData); \r\n",strDlgName);
          strBuilder.AppendLine("\t\t}")
	          .AppendLine();

            
          strBuilder.AppendLine("\t\tpublic void OnHideWindow(UIBaseWindow uiBaseWindow)")
	          .AppendLine("\t\t{");
          
          strBuilder.AppendLine("\t\t}")
	          .AppendLine();
          
          
          strBuilder.AppendLine("\t\tpublic void BeforeUnload(UIBaseWindow uiBaseWindow)")
	          .AppendLine("\t\t{");
          
          strBuilder.AppendLine("\t\t}")
	          .AppendLine();
          
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");

        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }
    
	
	static void SpawnCodeForDlgModel(GameObject gameObject)
    {
        string strDlgName = gameObject.name;
        string strFilePath = Application.dataPath + "/../Codes/ModelView/GameLogic/UI/" + strDlgName  ;
        
        
        if ( !System.IO.Directory.Exists(strFilePath) )
        {
	        System.IO.Directory.CreateDirectory(strFilePath);
        }
        
	    strFilePath = Application.dataPath + "/../Codes/ModelView/GameLogic/UI/" + strDlgName  + "/" + strDlgName  + ".cs";
        if(System.IO.File.Exists(strFilePath))
        {
	        Debug.LogError("已存在 " + strDlgName + ".cs,将不会再次生成。");
            return;
        }

        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
        StringBuilder strBuilder = new StringBuilder();
        
        strBuilder.AppendLine("namespace ET");
        strBuilder.AppendLine("{");

      
        strBuilder.AppendFormat("\tpublic  class {0} :Entity\r\n", strDlgName);
          strBuilder.AppendLine("\t{");
          strBuilder.AppendLine("");
          
	    strBuilder.AppendLine("\t\tpublic "+strDlgName+"ViewComponent View { get => this.Parent.GetComponent<"+ strDlgName +"ViewComponent>();} \r\n");
	    
        strBuilder.AppendLine("\t\t \r\n");
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");

        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }
    

    static void SpawnCodeForDlgBehaviour(GameObject gameObject)
    {
        if (null == gameObject)
        {
            return;
        }
        string strDlgName = gameObject.name ;
        string strDlgComponentName =  gameObject.name + "ViewComponent";

        string strFilePath = Application.dataPath + "/../Codes/HotfixView/GameLogic/UIBehaviour/" + strDlgName;

        if ( !System.IO.Directory.Exists(strFilePath) )
        {
	        System.IO.Directory.CreateDirectory(strFilePath);
        }
	    strFilePath = Application.dataPath + "/../Codes/HotfixView/GameLogic/UIBehaviour/" + strDlgName + "/" + strDlgComponentName + "System.cs";
	    
        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

        
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine()
	        .AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("namespace ET");
        strBuilder.AppendLine("{");
        strBuilder.AppendLine("\t[ObjectSystem]");
        strBuilder.AppendFormat("\tpublic class {0}AwakeSystem : AwakeSystem<{1}> \r\n", strDlgComponentName, strDlgComponentName);
        strBuilder.AppendLine("\t{");
        strBuilder.AppendFormat("\t\tpublic override void Awake({0} self)\n",strDlgComponentName);
        strBuilder.AppendLine("\t\t{");
        strBuilder.AppendLine("\t\t\tself.uiTransform = self.GetParent<UIBaseWindow>().uiTransform;");
        strBuilder.AppendLine("\t\t}");
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("\n");
        
       
        strBuilder.AppendLine("\t[ObjectSystem]");
        strBuilder.AppendFormat("\tpublic class {0}DestroySystem : DestroySystem<{1}> \r\n", strDlgComponentName, strDlgComponentName);
        strBuilder.AppendLine("\t{");
        strBuilder.AppendFormat("\t\tpublic override void Destroy({0} self)",strDlgComponentName);
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

    static void SpawnCodeForDlgComponentBehaviour(GameObject gameObject)
    {
	    if (null == gameObject)
	    {
		    return;
	    }
	    string strDlgName = gameObject.name ;
	    string strDlgComponentName =  gameObject.name + "ViewComponent";


	    string strFilePath = Application.dataPath + "/../Codes/ModelView/GameLogic/UIBehaviour/" + strDlgName;
	    if ( !System.IO.Directory.Exists(strFilePath) )
	    {
		    System.IO.Directory.CreateDirectory(strFilePath);
	    }
	    strFilePath = Application.dataPath + "/../Codes/ModelView/GameLogic/UIBehaviour/" + strDlgName + "/" + strDlgComponentName + ".cs";
	    StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
	    StringBuilder strBuilder = new StringBuilder();
	    strBuilder.AppendLine()
		    .AppendLine("using UnityEngine;");
	    strBuilder.AppendLine("using UnityEngine.UI;");
	    strBuilder.AppendLine("namespace ET");
	    strBuilder.AppendLine("{");
	    strBuilder.AppendFormat("\tpublic  class {0} : Entity \r\n", strDlgComponentName)
		    .AppendLine("\t{");
     
	    CreateWidgetBindCode(ref strBuilder, gameObject.transform);
	    CreateDeclareCode(ref strBuilder);
	    strBuilder.AppendFormat("\t\tpublic Transform uiTransform = null;\r\n");
	    strBuilder.AppendLine("\t}");
	    strBuilder.AppendLine("}");
        
	    sw.Write(strBuilder);
	    sw.Flush();
	    sw.Close();
    }
    
    

    public static void CreateDlgWidgetDisposeCode(ref StringBuilder strBuilder,bool isSelf = false)
    {
	    string pointStr = isSelf ? "self" : "this";
	    foreach (KeyValuePair<string, Component> pair in Path2WidgetCachedDict)
	    {
		    Component widget = pair.Value;
		    string strClassType = widget.GetType().ToString();
		   
		    if (pair.Key.StartsWith("ES"))
		    {
			    strBuilder.AppendFormat("\t\t	{0}.m_{1}?.Dispose();\r\n", pointStr,pair.Key.ToLower());
			    strBuilder.AppendFormat("\t\t	{0}.m_{1} = null;\r\n", pointStr,pair.Key.ToLower());
			    continue;
		    }

		    if ( strClassType== "UnityEngine.UI.Button")
		    {
			    strBuilder.AppendFormat("\t\t	{0}.m_{1} = null;\r\n", pointStr,widget.name+"Image");
		    }

		    strBuilder.AppendFormat("\t\t	{0}.m_{1} = null;\r\n", pointStr,widget.name);
	    }

	 
    }

    public static void CreateWidgetBindCode(ref StringBuilder strBuilder, Transform transRoot)
    {
        foreach (KeyValuePair<string, Component> pair in Path2WidgetCachedDict)
        {
	        Component widget = pair.Value;
			string strPath = GetWidgetPath(widget.transform, transRoot);
			string strClassType = widget.GetType().ToString();
			string strInterfaceType = strClassType;
			
			if (pair.Key.StartsWith("ES"))
			{
				GetSubUIBaseWindowCode(ref strBuilder, pair.Key,strPath);
				continue;
			}

			
			strBuilder.AppendFormat("		public {0} {1}\r\n", strInterfaceType, widget.name);
			strBuilder.AppendLine("     	{");
			strBuilder.AppendLine("     		get");
			strBuilder.AppendLine("     		{");
			
			strBuilder.AppendLine("     			if (this.uiTransform == null)");
			strBuilder.AppendLine("     			{");
			strBuilder.AppendLine("     				Log.Error(\"uiTransform is null.\");");
			strBuilder.AppendLine("     				return null;");
			strBuilder.AppendLine("     			}");

			if (transRoot.gameObject.name.StartsWith("Item"))
			{
				strBuilder.AppendLine("     			if (this.isCacheNode)");
				strBuilder.AppendLine("     			{");
				strBuilder.AppendFormat("     				if( this.m_{0} == null )\n" , widget.name);
				strBuilder.AppendLine("     				{");
				strBuilder.AppendFormat("		    			this.m_{0} = UIFindHelper.FindDeepChild<{2}>(this.uiTransform.gameObject,\"{1}\");\r\n", widget.name, strPath, strInterfaceType);
				strBuilder.AppendLine("     				}");
				strBuilder.AppendFormat("     				return this.m_{0};\n" , widget.name);
				strBuilder.AppendLine("     			}");
				strBuilder.AppendLine("     			else");
				strBuilder.AppendLine("     			{");
				strBuilder.AppendFormat("		    		return UIFindHelper.FindDeepChild<{2}>(this.uiTransform.gameObject,\"{1}\");\r\n", widget.name, strPath, strInterfaceType);
				strBuilder.AppendLine("     			}");
			}
			else
			{
				strBuilder.AppendFormat("     			if( this.m_{0} == null )\n" , widget.name);
				strBuilder.AppendLine("     			{");
				strBuilder.AppendFormat("		    		this.m_{0} = UIFindHelper.FindDeepChild<{2}>(this.uiTransform.gameObject,\"{1}\");\r\n", widget.name, strPath, strInterfaceType);
				strBuilder.AppendLine("     			}");
				strBuilder.AppendFormat("     			return this.m_{0};\n" , widget.name);
			}
			
            strBuilder.AppendLine("     		}");
            strBuilder.AppendLine("     	}\n");

          if (strInterfaceType  == "UnityEngine.UI.Button")
          {
             string newName = widget.name + "Image";
             
             strBuilder.AppendFormat("		public {0} {1}\r\n", "UnityEngine.UI.Image", newName);
             strBuilder.AppendLine("     	{");
             strBuilder.AppendLine("     		get");
             strBuilder.AppendLine("     		{");
             strBuilder.AppendLine("     			if (this.uiTransform == null)");
             strBuilder.AppendLine("     			{");
             strBuilder.AppendLine("     				Log.Error(\"uiTransform is null.\");");
             strBuilder.AppendLine("     				return null;");
             strBuilder.AppendLine("     			}");


             if (transRoot.gameObject.name.StartsWith("Item"))
             {
	             strBuilder.AppendLine("     			if (this.isCacheNode)");
	             strBuilder.AppendLine("     			{");
	             strBuilder.AppendFormat("     			    if( this.m_{0} == null )\n" , newName);
	             strBuilder.AppendLine("     			   {");
	             strBuilder.AppendFormat("		    		this.m_{0} = UIFindHelper.FindDeepChild<{2}>(this.uiTransform.gameObject,\"{1}\");\r\n", newName, strPath, "UnityEngine.UI.Image");
	             strBuilder.AppendLine("     			   }");
	             strBuilder.AppendFormat("     			   return this.m_{0};\n" , newName);
	             strBuilder.AppendLine("     			}");
	             strBuilder.AppendLine("     			else");
	             strBuilder.AppendLine("     			{");
	             strBuilder.AppendFormat("		    		return UIFindHelper.FindDeepChild<{2}>(this.uiTransform.gameObject,\"{1}\");\r\n", newName, strPath, "UnityEngine.UI.Image");
	             strBuilder.AppendLine("     			}");
	             strBuilder.AppendLine("     	    }\n");
             }
             else
             {
	             strBuilder.AppendFormat("     			if( this.m_{0} == null )\n" , newName);
	             strBuilder.AppendLine("     			{");
	             strBuilder.AppendFormat("		    		this.m_{0} = UIFindHelper.FindDeepChild<{2}>(this.uiTransform.gameObject,\"{1}\");\r\n", newName, strPath, "UnityEngine.UI.Image");
	             strBuilder.AppendLine("     			}");
	             strBuilder.AppendFormat("     			return this.m_{0};\n" , newName);
	             strBuilder.AppendLine("     		}");
             }
             strBuilder.AppendLine("     	}\n");
          }

        }
    }
    
    public static void CreateDeclareCode(ref StringBuilder strBuilder)
    {
	    foreach (KeyValuePair<string, Component> pair in Path2WidgetCachedDict)
	    {
		    Component widget = pair.Value;
		    string strClassType = widget.GetType().ToString();

		    if ( pair.Key.StartsWith("ES"))
		    {
			    string subUIClassType = Regex.Replace(pair.Key, @"\d", "");  
			    strBuilder.AppendFormat("\t\tpublic {0} m_{1} = null;\r\n", subUIClassType, pair.Key.ToLower());
			    continue;
		    }

		    if ( strClassType== "UnityEngine.UI.Button")
		    {
			    strBuilder.AppendFormat("\t\tpublic {0} m_{1} = null;\r\n", "UnityEngine.UI.Image",
				    widget.name + "Image");
		    }

		    strBuilder.AppendFormat("\t\tpublic {0} m_{1} = null;\r\n", strClassType, widget.name);
	    }
    }

    public static void FindAllWidgets(Transform trans, string strPath)
	{
		if (null == trans)
		{
			return;
		}
		for (int nIndex= 0; nIndex < trans.childCount; ++nIndex)
		{
			Transform child = trans.GetChild(nIndex);
			string strTemp = strPath+"/"+child.name;
			
		
			bool isSubUI = child.name.StartsWith("ES");
			if (isSubUI || child.name.StartsWith("EG"))
			{
				Path2WidgetCachedDict.Add(child.name,child.GetComponent<RectTransform>());
			}
			else if (child.name.StartsWith("E"))
			{
				foreach (var uiComponent in WidgetInterfaceList)
				{
					Component component = child.GetComponent(uiComponent);
					if (null == component)
					{
						continue;
					}
					if ( Path2WidgetCachedDict.ContainsKey(child.name) )
					{
						Debug.LogWarning("预设物可能存在重复的物体名： " + strTemp );
						continue;
					}
					Path2WidgetCachedDict.Add(child.name, component);
				}
			}
		
			if (isSubUI)
			{
				Debug.Log($"遇到子UI：{child.name},不生成子UI项代码");
				continue;
			}
			FindAllWidgets(child, strTemp);
		}
	}

    static string GetWidgetPath(Transform obj, Transform root)
    {
        string path = obj.name;

        while (obj.parent != null && obj.parent != root)
        {
            obj = obj.transform.parent;
            path = obj.name + "/" + path;
        }
        return path;
    }


    static void GetSubUIBaseWindowCode(ref StringBuilder strBuilder,string widget,string strPath)
    {
	    
	    string subUIClassType = Regex.Replace(widget, @"\d", "");
	    
	    strBuilder.AppendFormat("		public {0} {1}\r\n", subUIClassType, widget );
	    strBuilder.AppendLine("     	{");
	    strBuilder.AppendLine("     		get");
	    strBuilder.AppendLine("     		{");
			
	    strBuilder.AppendLine("     			if (this.uiTransform == null)");
	    strBuilder.AppendLine("     			{");
	    strBuilder.AppendLine("     				Log.Error(\"uiTransform is null.\");");
	    strBuilder.AppendLine("     				return null;");
	    strBuilder.AppendLine("     			}");
	    
	    strBuilder.AppendFormat("     			if( this.m_{0} == null )\n" , widget.ToLower());
	    strBuilder.AppendLine("     			{");
	    strBuilder.AppendFormat("		    	   Transform subTrans = UIFindHelper.FindDeepChild<Transform>(this.uiTransform.gameObject,\"{0}\");\r\n",  strPath);
	    strBuilder.AppendFormat("		    	   this.m_{0} = this.AddChild<{1},Transform>(subTrans);\r\n", widget.ToLower(),subUIClassType);
	    strBuilder.AppendLine("     			}");
	    strBuilder.AppendFormat("     			return this.m_{0};\n" , widget.ToLower());
	    strBuilder.AppendLine("     		}");
	    
	    
	    
	    strBuilder.AppendLine("     	}\n");
    }
    

    static UICodeSpawner()
    {
        WidgetInterfaceList = new List<string>();        
        WidgetInterfaceList.Add("Button");
        WidgetInterfaceList.Add( "Text");
        WidgetInterfaceList.Add("Input");
        WidgetInterfaceList.Add("InputField");
        WidgetInterfaceList.Add( "Scrollbar");
        WidgetInterfaceList.Add("ToggleGroup");
        WidgetInterfaceList.Add("Toggle");
        WidgetInterfaceList.Add("Dropdown");
        WidgetInterfaceList.Add("Slider");
        WidgetInterfaceList.Add("ScrollRect");
        WidgetInterfaceList.Add( "Image");
        WidgetInterfaceList.Add("RawImage");
        WidgetInterfaceList.Add("Canvas");
        WidgetInterfaceList.Add("UIWarpContent");
        WidgetInterfaceList.Add("LoopVerticalScrollRect");
        WidgetInterfaceList.Add("LoopHorizontalScrollRect");
    }

    private static Dictionary<string, Component> Path2WidgetCachedDict =null;
    private static List<string> WidgetInterfaceList = null;
}

