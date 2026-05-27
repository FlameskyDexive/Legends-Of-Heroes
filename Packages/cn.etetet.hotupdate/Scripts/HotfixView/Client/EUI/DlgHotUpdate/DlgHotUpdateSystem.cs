namespace ET.Client
{
    [FriendOf(typeof(DlgHotUpdate))]
    public static class DlgHotUpdateSystem
    {
        public static void RegisterUIEvent(this DlgHotUpdate self)
        {
        }

        public static void ShowWindow(this DlgHotUpdate self, Entity contextData = null)
        {
        }

        public static void OnPatchDownloadProgress(this DlgHotUpdate self, int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            float progress = totalDownloadBytes <= 0 ? 0f : (float)currentDownloadBytes / totalDownloadBytes;
            self.View.E_ImgLoadingImage.fillAmount = progress;
            self.View.E_TxtLoadingText.text = $"{FormatBytes(currentDownloadBytes)}/{FormatBytes(totalDownloadBytes)}, {(progress * 100).ToString("f2")}%";
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024L) return $"{bytes}B";
            if (bytes < 1024L * 1024) return $"{bytes / 1024f:f2}KB";
            if (bytes < 1024L * 1024 * 1024) return $"{bytes / 1024f / 1024f:f2}MB";
            return $"{bytes / 1024f / 1024f / 1024f:f2}GB";
        }
    }
}
