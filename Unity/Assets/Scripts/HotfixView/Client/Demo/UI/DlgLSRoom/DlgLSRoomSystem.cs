using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [EntitySystemOf(typeof(DlgLSRoom))]
    [FriendOf(typeof(DlgLSRoom))]
    public static partial class DlgLSRoomSystem
    {

        public static void RegisterUIEvent(this DlgLSRoom self)
        {
            Room room = self.Room();

            if (room == null)
            {
                return;
            }
            
            if (room.IsReplay)
            {
                self.View.EGReplayRectTransform.gameObject.SetActive(true);
                self.View.EGPlayRectTransform.gameObject.SetActive(false);
           
                self.View.EjumpButton.AddListener(self.Root(), self.JumpReplay);
                self.View.EspeedButton.AddListener(self.Root(),self.OnReplaySpeedClicked);
                self.View.EframecountText.text = self.Room().Replay.FrameInputs.Count.ToString();
            }
            else
            {
                self.View.EGReplayRectTransform.gameObject.SetActive(false);
                self.View.EGPlayRectTransform.gameObject.SetActive(true);
                self.View.E_SaveReplayButton.AddListener(self.Root(),self.OnSaveReplay);
            }
        }

        public static void ShowWindow(this DlgLSRoom self, Entity contextData = null)
        {
           
        }
        
        
        [EntitySystem]
        private static void Awake(this ET.Client.DlgLSRoom self)
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

        private static void OnSaveReplay(this DlgLSRoom self)
        {
            string name = self.View.E_SaveNameInputField.text;

            LSClientHelper.SaveReplay(self.Room(), name);
        }

        private static void JumpReplay(this DlgLSRoom self)
        {
            int toFrame = int.Parse(self.View.EjumpToCountInputField.text);
            LSClientHelper.JumpReplay(self.Room(), toFrame);
        }

        private static void OnReplaySpeedClicked(this DlgLSRoom self)
        {
            LSReplayUpdater lsReplayUpdater = self.Room().GetComponent<LSReplayUpdater>();
            lsReplayUpdater.ChangeReplaySpeed();
            self.View.EspeedButton.gameObject.Get<GameObject>("Text").GetComponent<Text>().text = $"X{lsReplayUpdater.ReplaySpeed}";
        }
     
    }
}
