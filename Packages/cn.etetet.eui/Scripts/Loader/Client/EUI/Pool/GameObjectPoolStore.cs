using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    /// <summary>
    /// EUI 全局 GameObject 池的静态状态。
    /// 放在 Loader 程序集里以规避 Hotfix 程序集禁止非 Const 静态字段（ET0004）的约束。
    /// 业务调用统一走 <see cref="GameObjectPoolHelper"/>。
    /// </summary>
    public static class GameObjectPoolStore
    {
        [StaticField]
        public static readonly Dictionary<string, GameObjectPool> PoolDict = new Dictionary<string, GameObjectPool>();

        [StaticField]
        public static GameObject PoolRoot;

        /// <summary>
        /// Init 场景已固化的池根路径。
        /// </summary>
        public const string POOL_ROOT_PATH = "/Global/PoolRoot";
    }
}
