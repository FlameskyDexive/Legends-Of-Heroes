using System;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [EnableClass]
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
            this.RoomNameText = this.transform.Find("RoomNameText")?.GetComponent<Text>();
            this.ModeText = this.transform.Find("ModeText")?.GetComponent<Text>();
            this.PlayerCountText = this.transform.Find("PlayerCountText")?.GetComponent<Text>();
            this.StatusText = this.transform.Find("StatusText")?.GetComponent<Text>();
            this.JoinButton = this.transform.Find("JoinButton")?.GetComponent<Button>();

            if (this.JoinButton != null)
            {
                this.JoinButton.onClick.AddListener(() => this.OnJoinClick?.Invoke());
            }
        }

        public void SetRoomInfo(RoomInfo roomInfo)
        {
            this.RoomInfo = roomInfo;
            if (this.RoomNameText != null) this.RoomNameText.text = roomInfo.RoomName;
            if (this.ModeText != null) this.ModeText.text = roomInfo.Mode.ToString();
            if (this.PlayerCountText != null) this.PlayerCountText.text = $"{roomInfo.PlayerInfo.Count}/{roomInfo.MaxPlayers}";
            if (this.StatusText != null) this.StatusText.text = roomInfo.Status.ToString();
        }

        public void OnDestroy()
        {
            if (this.JoinButton != null)
            {
                this.JoinButton.onClick.RemoveAllListeners();
            }
        }
    }
}
