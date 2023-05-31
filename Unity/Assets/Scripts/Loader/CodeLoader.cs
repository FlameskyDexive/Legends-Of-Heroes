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
        private Assembly hotfixAssembly;

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
                bool modelDllExist = false;
                bool hotfixDllExist = false;
                Assembly[] asss = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly ass in asss)
                {
                    string name = ass.GetName().Name;
                    if (name == $"Model_{GlobalConfig.Instance.ModelVersion}")
                    {
                        modelDllExist = true;
                        this.model = ass;
                    }
                    if (name == $"Hotfix_{GlobalConfig.Instance.HotFixVersion}")
                    {
                        hotfixDllExist = true;
                        this.hotfixAssembly = ass;
                    }
                }

                if (modelDllExist && hotfixDllExist)
                {
                    Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(typeof(Game).Assembly, typeof(Init).Assembly, this.model, hotfixAssembly);
                    EventSystem.Instance.Add(types);
                }
                else
                {

                    if (!Define.IsEditor)
                    {
                        if (Define.EnableIL2CPP)
                        {
                            HybridCLRHelper.Load();
                        }
                    }

                    if (!modelDllExist)
                    {
                        byte[] assBytes = MonoResComponent.Instance.LoadRawFile($"Model_{GlobalConfig.Instance.ModelVersion}.dll");
                        byte[] pdbBytes = MonoResComponent.Instance.LoadRawFile($"Model_{GlobalConfig.Instance.ModelVersion}.pdb");
                        this.model = Assembly.Load(assBytes, pdbBytes);
                    }

                    if (!hotfixDllExist)
                    {
                        this.LoadHotfix();
                    }
                    Dictionary<string, Type> types2 = AssemblyHelper.GetAssemblyTypes(typeof(Game).Assembly, typeof(Init).Assembly, this.model, hotfixAssembly);

                    EventSystem.Instance.Add(types2);
                }

            }

            IStaticMethod start = new StaticMethod(this.model, "ET.Entry", "Start");
            start.Run();
        }

        // 热重载调用该方法
        public void LoadHotfix()
        {
            byte[] assBytes = MonoResComponent.Instance.LoadRawFile($"Hotfix_{GlobalConfig.Instance.HotFixVersion}.dll");
            byte[] pdbBytes = MonoResComponent.Instance.LoadRawFile($"Hotfix_{GlobalConfig.Instance.HotFixVersion}.pdb");

            hotfixAssembly = Assembly.Load(assBytes, pdbBytes);

        }
    }
}