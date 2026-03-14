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
                    BTPackage package = BTLoader.Instance.LoadPackage(treeName, false);
                    if (package == null)
                    {
                        Log.Debug($"load behavior tree failed: {treeName}");
                        break;
                    }

                    BTDefinition entryTree = package.GetEntryTree();
                    Log.Debug($"behavior tree loaded: {treeName}, package={package.PackageName}, treeCount={package.Trees.Count}, entry={entryTree?.TreeName}, nodeCount={entryTree?.Nodes.Count ?? 0}");
                    break;
                }
                case "RunTree":
                {
                    string fileName = ss.Length > 1 ? ss[1] : "AITest";
                    string treeName = ss.Length > 2 ? ss[2] : "AITest";
                    byte[] bytes = await BTLoader.Instance.LoadBytesAsync(fileName, false);
                    if (bytes == null || bytes.Length == 0)
                    {
                        Log.Debug($"run behavior tree failed, bytes empty: {fileName}");
                        break;
                    }

                    BTRunner runner = BTRuntime.Create(fiber.Root, bytes, treeName);
                    if (runner == null)
                    {
                        Log.Debug($"run behavior tree failed: {fileName}/{treeName}");
                        break;
                    }

                    runner.Start();
                    await fiber.Root.GetComponent<TimerComponent>().WaitAsync(2200);
                    runner.Dispose();
                    Log.Debug($"behavior tree run finish: {fileName}/{treeName}");
                    break;
                }
            }
            await ETTask.CompletedTask;
        }
    }
}
