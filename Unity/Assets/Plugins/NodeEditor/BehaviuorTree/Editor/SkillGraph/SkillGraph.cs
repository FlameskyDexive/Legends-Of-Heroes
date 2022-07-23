using System;
using System.IO;
using ET;
using Sirenix.OdinInspector;
// using Sirenix.Serialization;
// using UnityEditor;
using UnityEngine;
using OdinSerializer;
// using SerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace Plugins.NodeEditor
{
    public class SkillGraph : NPBehaveGraph
    {
        [BoxGroup("此技能树数据载体(客户端)")] [DisableInEditorMode]
        public NP_DataSupportor SkillDataSupportor_Client = new NP_DataSupportor();

        [BoxGroup("技能树反序列化测试(客户端)")] [DisableInEditorMode]
        public NP_DataSupportor SkillDataSupportor_Client_Des = new NP_DataSupportor();
        

        [Button("自动配置所有技能结点数据", 25), GUIColor(0.4f, 0.8f, 1)]
        public void AutoSetCanvasDatas()
        {
            this.OnGraphEnable();
            base.AutoSetCanvasDatas();
            SkillDataSupportor_Client.NpDataSupportorBase = this.NpDataSupportor_Client;
            this.AutoSetSkillData_NodeData(SkillDataSupportor_Client);
        }

        [Button("保存技能树信息为二进制文件", 25), GUIColor(0.4f, 0.8f, 1)]
        public void Save()
        {
            if (string.IsNullOrEmpty(SavePathClient) ||
                string.IsNullOrEmpty(Name))
            {
                Log.Error($"保存路径或文件名不能为空，请检查配置");
                return;
            }
            
            using (FileStream file = File.Create($"{SavePathClient}/{this.Name}.bytes"))
            {
                byte[] bytes = SerializationUtility.SerializeValue(SkillDataSupportor_Client, DataFormat.Binary);
                file.Write(bytes, 0, bytes.Length);
            }
            using (FileStream file = File.Create($"{SavePathClient}/{this.Name}.json"))
            {
                byte[] bytes = SerializationUtility.SerializeValue(SkillDataSupportor_Client, DataFormat.JSON);
                file.Write(bytes, 0, bytes.Length);
            }
            Debug.Log($"保存 {SavePathClient}/{this.Name}.bytes 成功");
        }

        [Button("测试技能树反序列化", 25), GUIColor(0.4f, 0.8f, 1)]
        public void TestDe()
        {
            try
            {
                byte[] mClientfile = File.ReadAllBytes($"{SavePathClient}/{this.Name}.bytes");
                byte[] mClientfile2 = File.ReadAllBytes($"{SavePathClient}/{this.Name}.json");
                if (mClientfile.Length == 0) Debug.Log("没有读取到文件");
                string str = "";
                foreach (byte b in mClientfile)
                {
                    str += $"{b},";
                }
                SkillDataSupportor_Client_Des = SerializationUtility.DeserializeValue<NP_DataSupportor>(mClientfile, DataFormat.Binary);
                NP_DataSupportor nd2 = SerializationUtility.DeserializeValue<NP_DataSupportor>(mClientfile2, DataFormat.JSON);
                Debug.Log($"{str}, {nd2 == null}");
                Debug.Log($"反序列化 {SavePathClient}/{this.Name}.bytes 成功");
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                throw;
            }
        }

        private void AutoSetSkillData_NodeData(NP_DataSupportor npDataSupportor)
        {
            if (npDataSupportor.BuffNodeDataDic == null) return;
            npDataSupportor.BuffNodeDataDic.Clear();

            foreach (var node in this.nodes)
            {
                if (node is BuffNodeBase mNode)
                {
                    mNode.AutoAddLinkedBuffs();
                    BuffNodeDataBase buffNodeDataBase = mNode.GetBuffNodeData();
                    if (buffNodeDataBase is NormalBuffNodeData normalBuffNodeData)
                    {
                        normalBuffNodeData.BuffData.BelongToBuffDataSupportorId =
                            npDataSupportor.NpDataSupportorBase.NPBehaveTreeDataId;
                    }

                    npDataSupportor.BuffNodeDataDic.Add(buffNodeDataBase.NodeId.Value, buffNodeDataBase);
                }
            }
        }
    }
}
