using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ET
{
    public static class AwaiterCoroutineExtension
    {
        public static AwaiterCoroutine<IEnumerator> GetAwaiter(this IEnumerator coroutine)
        {
            return new AwaiterCoroutine<IEnumerator>(coroutine);
        }

        public static AwaiterCoroutine<WaitForNextFrame> GetAwaiter(this WaitForNextFrame waitForNextFrame)
        {
            return new AwaiterCoroutine<WaitForNextFrame>(waitForNextFrame);
        }

        public static AwaiterCoroutine<WaitForSeconds> GetAwaiter(this WaitForSeconds waitForSeconds)
        {
            return new AwaiterCoroutine<WaitForSeconds>(waitForSeconds);
        }

        public static AwaiterCoroutine<WaitForSecondsRealtime> GetAwaiter(this WaitForSecondsRealtime waitForSecondsRealtime)
        {
            return new AwaiterCoroutine<WaitForSecondsRealtime>(waitForSecondsRealtime);
        }

        public static AwaiterCoroutine<WaitForEndOfFrame> GetAwaiter(this WaitForEndOfFrame waitForEndOfFrame)
        {
            return new AwaiterCoroutine<WaitForEndOfFrame>(waitForEndOfFrame);
        }

        public static AwaiterCoroutine<WaitForFixedUpdate> GetAwaiter(this WaitForFixedUpdate waitForFixedUpdate)
        {
            return new AwaiterCoroutine<WaitForFixedUpdate>(waitForFixedUpdate);
        }

        public static AwaiterCoroutine<WaitUntil> GetAwaiter(this WaitUntil waitUntil)
        {
            return new AwaiterCoroutine<WaitUntil>(waitUntil);
        }

        public static AwaiterCoroutine<WaitWhile> GetAwaiter(this WaitWhile waitWhile)
        {
            return new AwaiterCoroutine<WaitWhile>(waitWhile);
        }

        public static AwaiterCoroutine<WWW> GetAwaiter(this WWW www)
        {
            return new AwaiterCoroutine<WWW>(www);
        }

        /*public static AwaiterCoroutine<AsyncOperation> GetAwaiter(this AsyncOperation asyncOperation)
        {
            return new AwaiterCoroutine<AsyncOperation>(asyncOperation);
        }*/

        public static AwaiterCoroutine<CustomYieldInstruction> GetAwaiter(this CustomYieldInstruction customYieldInstruction)
        {
            return new AwaiterCoroutine<CustomYieldInstruction>(customYieldInstruction);
        }

        public static AwaiterCoroutineWaitForMainThread GetAwaiter(this WaitForMainThread waitForMainThread)
        {
            return new AwaiterCoroutineWaitForMainThread();
        }
    }

    public struct WaitForNextFrame { }
    public struct WaitForMainThread { }
}