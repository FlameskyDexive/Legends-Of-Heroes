using UnityEngine;

namespace ET.Client
{
    // 行为树运行时字节加载：通过 YooAsset 按全路径加载 cn.etetet.bundles 下的 .bytes。
    // 参照 cn.etetet.map 的 RecastFileReader 范式。BTreeLoader.LoadBytesAsync 会 Invoke 此处理器。
    [Invoke]
    public class GetOneBehaviorTreeBytesHandler : AInvokeHandler<BTreeBytesLoader.GetOneBehaviorTreeBytes, ETTask<byte[]>>
    {
        public override async ETTask<byte[]> Handle(BTreeBytesLoader.GetOneBehaviorTreeBytes args)
        {
            TextAsset textAsset = await ResourcesComponent.Instance.LoadAssetAsync<TextAsset>(
                $"Packages/cn.etetet.bundles/Bundles/AI/Bytes/{args.TreeName}.bytes");
            return textAsset?.bytes;
        }
    }
}
