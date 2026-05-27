using System.IO;

namespace ET.Client
{
    [FriendOf(typeof(DlgLSLobby))]
    public static class DlgLSLobbySystem
    {
        public static void RegisterUIEvent(this DlgLSLobby self)
        {
            EntityRef<DlgLSLobby> selfRef = self;
            self.View.EMatchButton.AddListenerAsync(self.Root(), () => OnEnterMap(selfRef));
            self.View.EReplayButton.AddListener(self.Root(), () => OnReplay(selfRef));
        }

        public static void ShowWindow(this DlgLSLobby self, Entity contextData = null)
        {
        }

        private static async ETTask OnEnterMap(EntityRef<DlgLSLobby> selfRef)
        {
            DlgLSLobby self = selfRef;
            if (self == null) { return; }
            await EnterMapHelper.Match(self.Fiber());
        }

        private static void OnReplay(EntityRef<DlgLSLobby> selfRef)
        {
            DlgLSLobby self = selfRef;
            if (self == null) { return; }
            string path = self.View.EReplayPathInputField.text;
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                Log.Error($"replay path is invalid: {path}");
                return;
            }
            byte[] bytes = File.ReadAllBytes(path);
            Replay replay = MemoryPackHelper.Deserialize(typeof(Replay), bytes, 0, bytes.Length) as Replay;
            if (replay == null)
            {
                Log.Error("deserialize replay failed");
                return;
            }
            Log.Debug($"start replay: {replay.Snapshots.Count} {replay.FrameInputs.Count} {replay.UnitInfos.Count}");
            LSSceneChangeHelper.SceneChangeToReplay(self.Root(), replay).Coroutine();
        }
    }
}
