using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [EnableMethod]
    public class RoomItemComponent : MonoBehaviour
    {
        public Text RoomNameText;
        public Text ModeText;
        public Text PlayerCountText;
        public Text StatusText;
        public Button JoinButton;

        public RoomInfo RoomInfo;
        public Action OnJoinClick;

        public void Awake()
        {
            RoomNameText = transform.Find("RoomNameText").GetComponent<Text>();
            ModeText = transform.Find("ModeText").GetComponent<Text>();
            PlayerCountText = transform.Find("PlayerCountText").GetComponent<Text>();
            StatusText = transform.Find("StatusText").GetComponent<Text>();
            JoinButton = transform.Find("JoinButton").GetComponent<Button>();

            JoinButton.onClick.AddListener(() => OnJoinClick?.Invoke());
        }

        public void SetRoomInfo(RoomInfo roomInfo)
        {
            RoomInfo = roomInfo;
            RoomNameText.text = roomInfo.RoomName;
            ModeText.text = roomInfo.Mode.ToString();
            PlayerCountText.text = $"{roomInfo.PlayerInfo.Count}/{roomInfo.MaxPlayers}";
            StatusText.text = roomInfo.Status.ToString();
        }

        public void OnDestroy()
        {
            if (JoinButton != null)
            {
                JoinButton.onClick.RemoveAllListeners();
            }
        }
    }
}