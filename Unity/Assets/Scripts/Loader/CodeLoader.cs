using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using YooAsset;

namespace ET
{
    public class CodeLoader : Singleton<CodeLoader>
    {
        private Assembly model;

        public void Start()
        {
            if (Define.EnableCodes)
            {
                if (GlobalConfig.Instance.CodeMode != CodeMode.ClientServer)
                {
                    throw new Exception("ENABLE_CODES mode must use ClientServer code mode!");
                }

                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(assemblies);
                EventSystem.Instance.Add(types);
                foreach (Assembly ass in assemblies)
                {
                    string name = ass.GetName().Name;
                    if (name == "Unity.Model.Codes")
                    {
                        this.model = ass;
                    }
                }
            }
            else
            {
                byte[] assBytes = MonoResComponent.Instance.LoadRawFile($"Model_{GlobalConfig.Instance.ModelVersion}.dll");
                byte[] pdbBytes = MonoResComponent.Instance.LoadRawFile($"Model_{GlobalConfig.Instance.ModelVersion}.pdb");

                if (!Define.IsEditor)
                {
                    if (Define.EnableIL2CPP)
                    {
                        HybridCLRHelper.Load();
                    }
                }

                this.model = Assembly.Load(assBytes, pdbBytes);
                this.LoadHotfix();
            }

            IStaticMethod start = new StaticMethod(this.model, "ET.Entry", "Start");
            start.Run();
        }

        // 热重载调用该方法
        public void LoadHotfix()
        {
            byte[] assBytes = MonoResComponent.Instance.LoadRawFile($"Hotfix_{GlobalConfig.Instance.HotFixVersion}.dll");
            byte[] pdbBytes = MonoResComponent.Instance.LoadRawFile($"Hotfix_{GlobalConfig.Instance.HotFixVersion}.pdb");

            Assembly hotfixAssembly = Assembly.Load(assBytes, pdbBytes);

            Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(typeof(Game).Assembly, typeof(Init).Assembly, this.model, hotfixAssembly);

            EventSystem.Instance.Add(types);
        }
    }
}