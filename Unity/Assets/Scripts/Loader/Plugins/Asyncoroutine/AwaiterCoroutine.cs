using System;
using System.Collections;
using System.Threading;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ET
{
    public partial class AwaiterCoroutine<TInstruction> : INotifyCompletion
    {
        private Action _continuation;

        public TInstruction Instruction { get; protected set; }
        public Enumerator Coroutine { get; private set; }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get
            {
                return _isCompleted;
            }
            protected set
            {
                _isCompleted = value;

                if (value && _continuation != null)
                {
                    _continuation();
                    _continuation = null;
                }
            }
        }

        public AwaiterCoroutine()
        {

        }

        public AwaiterCoroutine(TInstruction instruction)
        {
            ProcessCoroutine(instruction);
        }

        private void ProcessCoroutine(TInstruction instruction)
        {
            Instruction = instruction;
            Coroutine = new Enumerator(this);

            AwaiterCoroutineer.Instance.StartAwaiterCoroutine(this);
        }

        public TInstruction GetResult()
        {
            return Instruction;
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            OnCompleted(continuation);
        }

        protected virtual void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }
    }

    public class AwaiterCoroutineWaitForMainThread : AwaiterCoroutine<WaitForMainThread>
    {
        public AwaiterCoroutineWaitForMainThread()
        {
            Instruction = default(WaitForMainThread);
        }

        protected override void OnCompleted(Action continuation)
        {
            base.OnCompleted(continuation);

            if (SynchronizationContext.Current != null)
            {
                IsCompleted = true;
            }
            else
            {
                AwaiterCoroutineer.Instance.SynchronizationContext.Post(state =>
                {
                    IsCompleted = true;
                }, null);
            }
        }
    }
}