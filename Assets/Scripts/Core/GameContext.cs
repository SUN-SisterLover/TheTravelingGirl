namespace TheTravelingGirl.Core
{
    /// <summary>
    /// 全局服务容器（轻量版 service locator）
    /// 各 MonoBehaviour 在 Awake 中注册自己，UI / 其他系统可按需取用
    ///
    /// 为什么不用完整 DI（Zenject / VContainer）:
    ///   - 1 人项目,过度工程化反而拖慢迭代
    ///   - 显式属性比容器反射注入更易调试
    ///   - 需要时随时可换成 DI 框架,API 不需要改
    ///
    /// 使用方式:
    ///   - 服务方: Awake 里赋值 (例如 GameContext.DialogueRunner = this)
    ///   - 使用方: 直接读取 GameContext.SomeService
    /// </summary>
    public static class GameContext
    {
        /// <summary>当前生效的对话运行器（可能为 null）。Awake 时注册,OnDestroy 时清空</summary>
        public static TheTravelingGirl.Runtime.DialogueRunner DialogueRunner { get; set; }

        /// <summary>当前游戏状态。默认 Title</summary>
        public static GameState State { get; set; } = GameState.Title;
    }
}
