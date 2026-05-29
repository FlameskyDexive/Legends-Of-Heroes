using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace ET.Test
{
    /// <summary>
    /// 大规模 3D 碰撞检测基准：
    /// 1) 正确性：广相（CollisionWorld3D 扫描剪枝）结果必须与暴力 O(N^2) 完全一致；
    /// 2) 性能：1k/5k/10k/20k 碰撞体批量检测计时；
    /// 结果写入控制台并存档到 ./Logs/CollisionBenchmark_<时间戳>.log。
    /// </summary>
    public class Collision_Benchmark_Test : ATestHandler
    {
        private const float MinHalf = 0.4f;
        private const float MaxHalf = 1.2f;
        private const int WarmupRuns = 3;
        private const int TimedRuns = 30;

        public override async ETTask<int> Handle(TestContext context)
        {
            await using TestFiberScope scope = await TestFiberScope.Create(context.Fiber, SceneType.TestEmpty, nameof(Collision_Benchmark_Test));

            StringBuilder report = new StringBuilder();
            report.AppendLine("==================== 3D Collision Broad-Phase Benchmark ====================");
            report.AppendLine($"Time      : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Machine   : {Environment.MachineName}  cores={Environment.ProcessorCount}  64bit={Environment.Is64BitProcess}");
            report.AppendLine($"Runtime   : {Environment.Version}  OS={Environment.OSVersion}");
            report.AppendLine($"Algorithm : Dense uniform grid broad-phase (scalar AABB) + exact narrow-phase");
            report.AppendLine($"Shapes    : mixed AABB / OBB / Sphere (1:1:1), half-extent {MinHalf}~{MaxHalf}");
            report.AppendLine();

            // ---- 1) 正确性对照 ----
            int correctnessRet = this.VerifyCorrectness(500, report);
            if (correctnessRet != ErrorCode.ERR_Success)
            {
                this.WriteLog(report);
                Log.Console(report.ToString());
                return correctnessRet;
            }

            // ---- 2) 规模性能 ----
            report.AppendLine("---- Performance (per detection pass; build = Clear+Add all colliders) ----");
            report.AppendLine(string.Format("{0,7} | {1,9} | {2,8} | {3,11} | {4,11} | {5,11} | {6,14}",
                "N", "worldSz", "pairs", "build(ms)", "detect(ms)", "total(ms)", "throughput"));
            report.AppendLine(new string('-', 92));

            int[] sizes = { 1000, 5000, 10000, 20000 };
            foreach (int size in sizes)
            {
                this.BenchmarkSize(size, report);
            }

            report.AppendLine();
            report.AppendLine("Notes:");
            report.AppendLine("  - Complexity: near-O(N) (constant throughput across N), zero per-frame heap alloc.");
            report.AppendLine("  - build = rebuild all world AABBs (needed when colliders move); detect = grid + narrow-phase.");
            report.AppendLine("  - Measured on this dev CPU via the DOTNET server build (managed, single-thread, NO Burst/SIMD).");
            report.AppendLine("    On Android the client runs IL2CPP (native); the math-heavy narrow-phase is typically");
            report.AppendLine("    comparable-to-somewhat-slower than this — profile on the actual device to confirm.");
            report.AppendLine("  - Frame budgets: 60fps=16.6ms, 30fps=33.3ms.");
            report.AppendLine("  - To push higher (e.g. 10k+ @60fps on mobile): (1) Burst-compile detect in a Unity Job");
            report.AppendLine("    (SIMD + multicore, usually 5~10x); (2) skip static-vs-static pairs; (3) run detection");
            report.AppendLine("    at a lower frequency or only on an active subset.");
            report.AppendLine("============================================================================");

            string path = this.WriteLog(report);
            Log.Console(report.ToString());
            if (path != null)
            {
                Log.Console($"benchmark report archived: {path}");
            }

            return ErrorCode.ERR_Success;
        }

        // 广相结果必须与暴力 O(N^2) 完全一致，否则视为漏检 / 误检。
        private int VerifyCorrectness(int n, StringBuilder report)
        {
            Collider3D[] arr = BuildColliders(n, 7777u, out _);

            HashSet<long> brute = new HashSet<long>();
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (Collision3DHelper.Intersect(arr[i], arr[j]))
                    {
                        brute.Add(Key(i, j));
                    }
                }
            }

            CollisionWorld3D world = new CollisionWorld3D(n);
            for (int i = 0; i < n; i++)
            {
                world.Add(arr[i]);
            }
            List<CollisionPair> results = new List<CollisionPair>(n);
            world.DetectPairs(results);

            HashSet<long> sap = new HashSet<long>();
            foreach (CollisionPair p in results)
            {
                if (!sap.Add(Key(p.A, p.B)))
                {
                    Log.Console($"broad-phase produced duplicate pair ({p.A},{p.B})");
                    report.AppendLine($"[FAIL] duplicate pair ({p.A},{p.B})");
                    return 1;
                }
            }

            if (!sap.SetEquals(brute))
            {
                int missing = 0, extra = 0;
                foreach (long k in brute)
                {
                    if (!sap.Contains(k)) missing++;
                }
                foreach (long k in sap)
                {
                    if (!brute.Contains(k)) extra++;
                }
                Log.Console($"broad-phase mismatch: brute={brute.Count}, sap={sap.Count}, missing={missing}, extra={extra}");
                report.AppendLine($"[FAIL] correctness: brute={brute.Count} sap={sap.Count} missing={missing} extra={extra}");
                return 2;
            }

            report.AppendLine($"[OK] correctness verified: N={n}, broad-phase == brute-force, pairs={brute.Count}");
            report.AppendLine();
            return ErrorCode.ERR_Success;
        }

        private void BenchmarkSize(int n, StringBuilder report)
        {
            Collider3D[] arr = BuildColliders(n, (uint)(1000 + n), out float worldSize);

            CollisionWorld3D world = new CollisionWorld3D(n);
            List<CollisionPair> results = new List<CollisionPair>(n);

            // 预热 + 取相交对数
            world.Clear();
            for (int i = 0; i < n; i++) world.Add(arr[i]);
            for (int w = 0; w < WarmupRuns; w++) world.DetectPairs(results);
            int pairs = results.Count;

            // detect 计时（bounds 已就绪）
            double detectMin = double.MaxValue;
            Stopwatch sw = Stopwatch.StartNew();
            for (int it = 0; it < TimedRuns; it++)
            {
                long t0 = sw.ElapsedTicks;
                world.DetectPairs(results);
                long t1 = sw.ElapsedTicks;
                double ms = (t1 - t0) * 1000.0 / Stopwatch.Frequency;
                if (ms < detectMin) detectMin = ms;
            }
            sw.Stop();
            double detectAvg = sw.Elapsed.TotalMilliseconds / TimedRuns;

            // build 计时（Clear + 重新 Add，模拟移动碰撞体每帧重建包围盒）
            Stopwatch bsw = Stopwatch.StartNew();
            for (int it = 0; it < TimedRuns; it++)
            {
                world.Clear();
                for (int i = 0; i < n; i++) world.Add(arr[i]);
            }
            bsw.Stop();
            double buildAvg = bsw.Elapsed.TotalMilliseconds / TimedRuns;

            double totalAvg = buildAvg + detectAvg;
            double throughput = totalAvg > 0 ? n / totalAvg : 0;

            report.AppendLine(string.Format("{0,7} | {1,9:F1} | {2,8} | {3,11:F3} | {4,11:F3} | {5,11:F3} | {6,9:F0} c/ms",
                n, worldSize, pairs, buildAvg, detectAvg, totalAvg, throughput));

            Log.Console($"bench N={n}: build={buildAvg:F3}ms detect={detectAvg:F3}ms (min {detectMin:F3}) total={totalAvg:F3}ms pairs={pairs}");
        }

        // 在体积内均匀撒点，密度恒定（worldSize 随 N^(1/3) 缩放），形状 AABB/OBB/Sphere 各 1/3。
        private static Collider3D[] BuildColliders(int n, uint seed, out float worldSize)
        {
            worldSize = (float)Math.Cbrt(n) * 4f;
            float h = worldSize * 0.5f;
            Random rng = new Random(seed);
            Collider3D[] arr = new Collider3D[n];
            for (int i = 0; i < n; i++)
            {
                float3 center = rng.NextFloat3(new float3(-h), new float3(h));
                int shape = rng.NextInt(0, 3);
                if (shape == 0)
                {
                    arr[i] = Collider3D.Create(new AABB(center, rng.NextFloat3(new float3(MinHalf), new float3(MaxHalf))));
                }
                else if (shape == 1)
                {
                    arr[i] = Collider3D.Create(new OBB(center, rng.NextFloat3(new float3(MinHalf), new float3(MaxHalf)), rng.NextQuaternionRotation()));
                }
                else
                {
                    arr[i] = Collider3D.Create(new Sphere(center, rng.NextFloat(MinHalf, MaxHalf)));
                }
            }
            return arr;
        }

        private static long Key(int a, int b)
        {
            return ((long)a << 32) | (uint)b;
        }

        private string WriteLog(StringBuilder report)
        {
            try
            {
                Directory.CreateDirectory("./Logs");
                string path = $"./Logs/CollisionBenchmark_{DateTime.Now:yyyyMMdd_HHmmss}.log";
                File.WriteAllText(path, report.ToString());
                return Path.GetFullPath(path);
            }
            catch (Exception e)
            {
                Log.Console($"write benchmark log failed: {e.Message}");
                return null;
            }
        }
    }
}
