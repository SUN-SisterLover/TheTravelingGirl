namespace TheTravelingGirl.Core
{
    /// <summary>
    /// 全局服务容器（轻量版 service locator）
    /// 各 MonoBehaviour 在 Awake 中注册自己，UI / 其他系统可按需取用
    /// </summary>
    public static class GameContext
    {
        /// <summary>当前生效的对话运行器（可能为 null）</summary>
        public static TheTravelingGirl.Runtime.DialogueRunner DialogueRunner { get; set; }

        /// <summary>当前游戏状态</summary>
        public static GameState State { get; set; } = GameState.Title;
    }
}
