using UnityEngine;

namespace ET
{
    public class ClientFixedUpdate: Singleton<ClientFixedUpdate>, ISingletonUpdate
    {

        protected float logicTimer = 0f;
        private float timeScale = 1;

        public string GetFrameCountAndTimeScale()
        {
            return $"{ClientFrame}/{timeScale.ToString("f1")}";
        }

        public long ClientFrame { get; set; }

        public void SetTimeScale(float scale)
        {
            Log.Warning($"set time scale : {scale.ToString("f2")}");
            timeScale = scale;
        }
        
        
        public void Update()
        {
            logicTimer += Time.deltaTime * timeScale;
            while (logicTimer >= DefineCore.FixedDeltaTime)
            {
                logicTimer -= DefineCore.FixedDeltaTime;
                // Debug.Log($"{Time.realtimeSinceStartup.ToString("f4")}, {(Time.realtimeSinceStartup - lastTime).ToString("f4")}, {logicDeltaTime.ToString("f4")}");
                // lastTime = Time.realtimeSinceStartup;
                ClientFrame++;
                this.FixedUpdate();
            }
        }

        /// <summary>
        /// 固定帧驱动
        /// </summary>
        private void FixedUpdate()
        {
            Game.FixedUpdate();

        }

        public override void Dispose()
        {
            ClientFrame = 0;
            base.Dispose();
        }
    }
}
