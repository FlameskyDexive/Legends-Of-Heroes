using UnityEngine.UI;

namespace ET.Client
{
    [EntitySystemOf(typeof(DlgLSRoom))]
    [FriendOf(typeof(DlgLSRoom))]
    public static partial class DlgLSRoomSystem
    {
        [EntitySystem]
        private static void Awake(this DlgLSRoom self)
        {
        }

        [EntitySystem]
        private static void Update(this DlgLSRoom self)
        {
            Room room = self.Room();
            if (room == null)
            {
                return;
            }

            if (self.frame != room.AuthorityFrame)
            {
                self.frame = room.AuthorityFrame;
                self.View.EprogressText.text = room.AuthorityFrame.ToString();
            }

            if (!room.IsReplay)
            {
                if (self.predictFrame != room.PredictionFrame)
                {
                    self.predictFrame = room.PredictionFrame;
                    self.View.EpredictText.text = room.PredictionFrame.ToString();
                }
            }
        }

        public static void RegisterUIEvent(this DlgLSRoom self)
        {
            Room room = self.Room();
            if (room == null)
            {
                return;
            }

            EntityRef<DlgLSRoom> selfRef = self;
            if (room.IsReplay)
            {
                self.View.EGReplayRectTransform.gameObject.SetActive(true);
                self.View.EGPlayRectTransform.gameObject.SetActive(false);

                self.View.EjumpButton.AddListener(self.Root(), () => OnJumpReplay(selfRef));
                self.View.EspeedButton.AddListener(self.Root(), () => OnReplaySpeed(selfRef));
                self.View.EframecountText.text = room.Replay.FrameInputs.Count.ToString();
            }
            else
            {
                self.View.EGReplayRectTransform.gameObject.SetActive(false);
                self.View.EGPlayRectTransform.gameObject.SetActive(true);
                self.View.E_SaveReplayButton.AddListener(self.Root(), () => OnSaveReplay(selfRef));
            }
        }

        public static void ShowWindow(this DlgLSRoom self, Entity contextData = null)
        {
        }

        private static void OnSaveReplay(EntityRef<DlgLSRoom> selfRef)
        {
            DlgLSRoom self = selfRef;
            if (self == null) { return; }
            string name = self.View.E_SaveNameInputField.text;
            LSClientHelper.SaveReplay(self.Room(), name);
        }

        private static void OnJumpReplay(EntityRef<DlgLSRoom> selfRef)
        {
            DlgLSRoom self = selfRef;
            if (self == null) { return; }
            if (!int.TryParse(self.View.EjumpToCountInputField.text, out int toFrame))
            {
                Log.Error($"invalid jump frame: {self.View.EjumpToCountInputField.text}");
                return;
            }
            LSClientHelper.JumpReplay(self.Room(), toFrame);
        }

        private static void OnReplaySpeed(EntityRef<DlgLSRoom> selfRef)
        {
            DlgLSRoom self = selfRef;
            if (self == null) { return; }
            LSReplayUpdater lsReplayUpdater = self.Room().GetComponent<LSReplayUpdater>();
            lsReplayUpdater.ChangeReplaySpeed();
            Text speedText = UIFindHelper.FindDeepChild<Text>(self.View.EspeedButton.gameObject, "Text");
            if (speedText != null)
            {
                speedText.text = $"X{lsReplayUpdater.ReplaySpeed}";
            }
        }
    }
}
