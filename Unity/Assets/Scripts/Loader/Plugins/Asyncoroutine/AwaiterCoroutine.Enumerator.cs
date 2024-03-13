using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public partial class AwaiterCoroutine<TInstruction>
    {
        public class Enumerator : IEnumerator
        {
            private AwaiterCoroutine<TInstruction> _parent;
            private IEnumerator _nestedCoroutine;
            public object Current { get; private set; }

            public Enumerator(AwaiterCoroutine<TInstruction> parent)
            {
                _parent = parent;
                _nestedCoroutine = parent.Instruction as IEnumerator;
            }

            bool IEnumerator.MoveNext()
            {
                if (_nestedCoroutine != null)
                {
                    bool result = _nestedCoroutine.MoveNext();
                    Current = _nestedCoroutine.Current;
                    _parent.IsCompleted = !result;

                    return result;
                }
                
                if (Current == null)
                {
                    Current = _parent.Instruction;
                    return true;
                }

                _parent.IsCompleted = true;
                return false;
            }

            void IEnumerator.Reset()
            {
                Current = null;
                _parent.IsCompleted = false;
            }
        }
    }
}