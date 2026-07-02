using UnityEngine;

namespace TheTravelingGirl.Core
{
    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameState
    {
        Title,        // 标题画面
        Playing,      // 自由探索
        Dialogue,     // 对话进行中
        Menu,         // 菜单打开
        Paused        // 暂停
    }

    /// <summary>
    /// 全局游戏管理器（单例）
    /// 跨场景持久存在，挂到一个名为 "GameManager" 的 GameObject 上
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Meta")]
        [SerializeField] private string gameVersion = "0.1.0";

        [Header("State")]
        [SerializeField] private GameState initialState = GameState.Title;

        public string GameVersion => gameVersion;
        public GameState State { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            State = initialState;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>
        /// 切换游戏状态
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (State == newState) return;
            State = newState;
            Debug.Log($"[GameManager] State -> {newState}");
        }
    }
}
