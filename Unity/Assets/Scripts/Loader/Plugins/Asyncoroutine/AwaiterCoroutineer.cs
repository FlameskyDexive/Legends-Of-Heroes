using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace ET
{
    public class AwaiterCoroutineer : MonoBehaviour
    {
        private static AwaiterCoroutineer _instance;
        public static AwaiterCoroutineer Instance
        {
            get
            {
                Install();
                return _instance;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Install()
        {
            if (_instance == null)
                _instance = new GameObject("AwaiterCoroutineer").AddComponent<AwaiterCoroutineer>();
        }

        public SynchronizationContext SynchronizationContext { get; private set; }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            DontDestroyOnLoad(_instance);
            SynchronizationContext = SynchronizationContext.Current;
        }

        public void StartAwaiterCoroutine<T>(AwaiterCoroutine<T> awaiterCoroutine)
        {
            StartCoroutine(awaiterCoroutine.Coroutine);
        }

        public void StopAwaiterCoroutine<T>(AwaiterCoroutine<T> awaiterCoroutine)
        {
            StopCoroutine(awaiterCoroutine.Coroutine);
        }
    }
}