using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ET
{
    internal class ThreadPoolScheduler: IScheduler
    {
        private readonly List<Thread> threads;

        private readonly ConcurrentQueue<int> idQueue = new();
        
        private readonly FiberManager fiberManager;
        private DateTime dt1970;
        private long lastTimeTicks = 0;
        private long totalTicksSinceStart = 0;

        public ThreadPoolScheduler(FiberManager fiberManager)
        {
            this.fiberManager = fiberManager;
            int threadCount = Environment.ProcessorCount;
            this.threads = new List<Thread>(threadCount);
            for (int i = 0; i < threadCount; ++i)
            {
                Thread thread = new(this.Loop);
                this.threads.Add(thread);
                thread.Start();
            }
            this.dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this.lastTimeTicks = this.dt1970.Ticks;
            this.totalTicksSinceStart = 0;
        }

        private void Loop()
        {
            int count = 0;
            while (true)
            {
                if (count <= 0)
                {
                    Thread.Sleep(1);
                    
                    // count最小为1
                    count = this.fiberManager.Count() / this.threads.Count + 1;
                }

                --count;
                
                if (this.fiberManager.IsDisposed())
                {
                    return;
                }
                
                if (!this.idQueue.TryDequeue(out int id))
                {
                    Thread.Sleep(1);
                    continue;
                }

                Fiber fiber = this.fiberManager.Get(id);
                if (fiber == null)
                {
                    continue;
                }

                if (fiber.IsDisposed)
                {
                    continue;
                }

                Fiber.Instance = fiber;
                SynchronizationContext.SetSynchronizationContext(fiber.ThreadSynchronizationContext);
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
                SynchronizationContext.SetSynchronizationContext(null);
                Fiber.Instance = null;

                this.idQueue.Enqueue(id);
            }
        }

        public void Dispose()
        {
            foreach (Thread thread in this.threads)
            {
                thread.Join();
            }
        }

        public void Add(int fiberId)
        {
            this.idQueue.Enqueue(fiberId);
        }
    }
}