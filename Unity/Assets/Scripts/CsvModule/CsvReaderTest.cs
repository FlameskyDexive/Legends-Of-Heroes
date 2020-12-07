using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

using UnityEngine.UI;

public class CsvReaderTest : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	    Button btnGetCsv = GameObject.Find("BtnGetCsvData").GetComponent<Button>();
	    btnGetCsv.onClick.AddListener(GetCsvData);

	}
    
	/// <summary>
	/// Buttons the click button load res.
	/// 解析获取csv配表工具
	/// </summary>
	
    public void GetCsvData()
    {
        /*DataSetMgr.prePath = Application.streamingAssetsPath + "/Config";
        DataSetMgr.InitDataTab();*/
        /*HundredRoomData hrData = (HundredRoomData) HundredRoomMgr.instance.GetDataById(1);
        
        /*skillTabData bean = (skillTabData)skillTabMgr.instance._GetDataById(1);
        Debug.Log("data is "+ bean.SkillName + " -- " +  bean.ActiveCondition.ToString());#1#
        UserInfoData user = (UserInfoData)UserInfoMgr.instance.GetDataById(2);
        Debug.Log($"---{user.Coin}");
        Debug.Log($"---{hrData.Jackpot_rate}");*/
        //Debug.Log("skill enum---" + user.ThisSkillMode.ToString() + "---role bool---" + user.IsMainRole.ToString());
    }
    
  

}
