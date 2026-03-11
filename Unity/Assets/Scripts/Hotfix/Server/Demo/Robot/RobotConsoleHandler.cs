using System;
using System.Reflection;

namespace ET.Server
{
    [ConsoleHandler(ConsoleMode.Robot)]
    public class RobotConsoleHandler: IConsoleHandler
    {
        public async ETTask Run(Fiber fiber, ModeContex contex, string content)
        {
            string[] ss = content.Split(" ");
            switch (ss[0])
            {
                case ConsoleMode.Robot:
                    break;

                case "Run":
                {
                    int caseType = int.Parse(ss[1]);

                    try
                    {
                        Log.Debug($"run case start: {caseType}");
                        await EventSystem.Instance.Invoke<RobotInvokeArgs, ETTask>(caseType, new RobotInvokeArgs() { Fiber = fiber, Content = content });
                        Log.Debug($"run case finish: {caseType}");
                    }
                    catch (Exception e)
                    {
                        Log.Debug($"run case error: {caseType}\n{e}");
                    }
                    break;
                }
                case "RunAll":
                {
                    FieldInfo[] fieldInfos = typeof (RobotCaseType).GetFields();
                    foreach (FieldInfo fieldInfo in fieldInfos)
                    {
                        int caseType = (int)fieldInfo.GetValue(null);
                        if (caseType > RobotCaseType.MaxCaseType)
                        {
                            Log.Debug($"case > {RobotCaseType.MaxCaseType}: {caseType}");
                            break;
                        }
                        try
                        {
                            Log.Debug($"run case start: {caseType}");
                            await EventSystem.Instance.Invoke<RobotInvokeArgs, ETTask>(caseType, new RobotInvokeArgs() { Fiber = fiber, Content = content});
                            Log.Debug($"---------run case finish: {caseType}");
                        }
                        catch (Exception e)
                        {
                            Log.Debug($"run case error: {caseType}\n{e}");
                            break;
                        }
                    }
                    break;
                }
                case "LoadTree":
                {
                    string treeName = ss.Length > 1 ? ss[1] : "AITest";
                    BehaviorTreePackage package = BehaviorTreeLoader.Instance.LoadPackage(treeName, false);
                    if (package == null)
                    {
                        Log.Debug($"load behavior tree failed: {treeName}");
                        break;
                    }

                    BehaviorTreeDefinition entryTree = package.GetEntryTree();
                    Log.Debug($"behavior tree loaded: {treeName}, package={package.PackageName}, treeCount={package.Trees.Count}, entry={entryTree?.TreeName}, nodeCount={entryTree?.Nodes.Count ?? 0}");
                    break;
                }
            }
            await ETTask.CompletedTask;
        }
    }
}
