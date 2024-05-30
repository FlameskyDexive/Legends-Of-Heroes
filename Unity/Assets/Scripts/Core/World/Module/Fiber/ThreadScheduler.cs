using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ET
{
    // 一个Fiber一个固定的线程
    internal class ThreadScheduler : IScheduler
    {
        private readonly ConcurrentDictionary<int, Thread> dictionary = new();

        private readonly FiberManager fiberManager;
        private DateTime dt1970;
        private long lastTimeTicks = 0;
        private long totalTicksSinceStart = 0;

        public ThreadScheduler(FiberManager fiberManager)
        {
            this.fiberManager = fiberManager;
            this.dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this.lastTimeTicks = this.dt1970.Ticks;
            this.totalTicksSinceStart = 0;
        }

        private void Loop(int fiberId)
        {
            Fiber fiber = fiberManager.Get(fiberId);
            Fiber.Instance = fiber;
            SynchronizationContext.SetSynchronizationContext(fiber.ThreadSynchronizationContext);

            while (true)
            {
                if (this.fiberManager.IsDisposed())
                {
                    return;
                }

                fiber = fiberManager.Get(fiberId);
                if (fiber == null)
                {
                    this.dictionary.Remove(fiberId, out _);
                    return;
                }

                if (fiber.IsDisposed)
                {
                    this.dictionary.Remove(fiberId, out _);
                    return;
                }

                fiber.Update();
                fiber.LateUpdate();
                long timeNow = DateTime.UtcNow.Ticks - this.dt1970.Ticks;
                long deltaTime = timeNow - this.lastTimeTicks;
                if (this.lastTimeTicks == this.dt1970.Ticks)
                {
                    deltaTime = 0;
                }
                this.totalTicksSinceStart += deltaTime;
                this.lastTimeTicks = timeNow;
                while (this.totalTicksSinceStart >= DefineCore.FixedDeltaTicks)
                {
                    this.totalTicksSinceStart -= DefineCore.FixedDeltaTicks;
                    fiber.FixedUpdate();
                }

                Thread.Sleep(1);
            }
        }

        public void Dispose()
        {
            foreach (var kv in this.dictionary.ToArray())
            {
                kv.Value.Join();
            }
        }

        public void Add(int fiberId)
        {
            Thread thread = new(() => this.Loop(fiberId));
            this.dictionary.TryAdd(fiberId, thread);
            thread.Start();
        }
    }
}