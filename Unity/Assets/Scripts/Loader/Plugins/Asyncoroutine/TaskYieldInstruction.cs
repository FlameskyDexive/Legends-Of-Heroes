using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// It refers to this link : https://github.com/Microsoft/xbox-live-unity-plugin/blob/master/Assets/Xbox%20Live/Scripts/UnityTaskExtensions.cs

namespace ET
{
    public class TaskYieldInstruction : CustomYieldInstruction
    {
        public Task Task { get; private set; }

        public override bool keepWaiting
        {
            get
            {
                if (Task.Exception != null)
                    throw Task.Exception;
                
                return !Task.IsCompleted;
            }
        }

        public TaskYieldInstruction(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            Task = task;
        }
    }

    public class TaskYieldInstruction<T> : TaskYieldInstruction
    {
        public new Task<T> Task { get; private set; }

        public T Result
        {
            get { return Task.Result; }
        }

        public TaskYieldInstruction(Task<T> task)
            : base(task)
        {
            Task = task;
        }
    }

}