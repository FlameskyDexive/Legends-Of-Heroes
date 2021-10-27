using System;
using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    public abstract class LoopScrollDataSource
    {
        public Action<Transform,int> scrollMoveEvent;
        public abstract void ProvideData(Transform transform, int idx);
    }

	public class LoopScrollSendIndexSource : LoopScrollDataSource
    {
        //public static readonly LoopScrollSendIndexSource Instance = new LoopScrollSendIndexSource();
      
        public LoopScrollSendIndexSource(){}

        public override void ProvideData(Transform transform, int idx)
        {
            scrollMoveEvent?.Invoke(transform, idx);
        }
    }

	public class LoopScrollArraySource<T> : LoopScrollDataSource
    {
        T[] objectsToFill;

		public LoopScrollArraySource(T[] objectsToFill)
        {
            this.objectsToFill = objectsToFill;
        }

        public override void ProvideData(Transform transform, int idx)
        {
            scrollMoveEvent?.Invoke(transform, idx);
        }
    }
}