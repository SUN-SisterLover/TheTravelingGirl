namespace TheTravelingGirl.Core
{
    /// <summary>
    /// 游戏整体状态。跨系统协调用（例如存档、菜单、自动模式）。
    /// </summary>
    public enum GameState
    {
        Title,        // 标题画面
        Playing,      // 自由探索 / 剧情中(无对话)
        Dialogue,     // 对话进行中
        Menu,         // 菜单打开
        Paused        // 暂停
    }
}
