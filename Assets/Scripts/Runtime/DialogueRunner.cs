using System;
using UnityEngine;
using TheTravelingGirl.Data;
using TheTravelingGirl.Core;

namespace TheTravelingGirl.Runtime
{
    /// <summary>
    /// 对话运行器
    /// 接收 Script,按顺序发出事件,由 UI 层订阅后渲染
    /// 不直接控制 UI —— 事件驱动,UI 可替换
    ///
    /// 设计取舍:
    ///   - 用 events 不用 direct UI reference,UI 类可以独立测试 / 替换
    ///   - Play() 把 State 切到 Dialogue,但 Stop() 不切回去
    ///     —— 终止对话后可能回到 Title / Playing / 主菜单,调用方更清楚该切到哪
    /// </summary>
    public class DialogueRunner : MonoBehaviour
    {
        public Script CurrentScript { get; private set; }
        public int CurrentLineIndex { get; private set; }
        public bool IsRunning => CurrentScript != null;

        public DialogueLine CurrentLine =>
            IsRunning && CurrentLineIndex >= 0 && CurrentLineIndex < CurrentScript.lines.Length
                ? CurrentScript.lines[CurrentLineIndex]
                : null;

        /// <summary>每句台词开始显示时触发。订阅者:UI 层 (DialogueBoxView 等)</summary>
        public event Action<DialogueLine> OnLineShown;

        /// <summary>一段对话开始播放时触发。订阅者:游戏状态机 / 自动保存 / 统计</summary>
        public event Action OnSequenceStart;

        /// <summary>一段对话播放完毕时触发。订阅者:UI (隐藏对话框) / 自动推进</summary>
        public event Action OnSequenceEnd;

        private void Awake()
        {
            // 注册到全局服务容器
            GameContext.DialogueRunner = this;
        }

        private void OnDestroy()
        {
            // 只清自己 —— 避免销毁时把别人注册的 runner 也清掉
            if (GameContext.DialogueRunner == this) GameContext.DialogueRunner = null;
        }

        /// <summary>
        /// 开始播放一段 Script
        /// </summary>
        public void Play(Script script)
        {
            if (script == null || script.lines == null || script.lines.Length == 0)
            {
                Debug.LogWarning("[DialogueRunner] Play called with empty script.");
                return;
            }
            CurrentScript = script;
            CurrentLineIndex = 0;
            GameContext.State = GameState.Dialogue;
            OnSequenceStart?.Invoke();
            EmitLine();
        }

        /// <summary>
        /// 推进到下一句。已是最后一句则结束。
        /// </summary>
        public void Advance()
        {
            if (!IsRunning) return;
            CurrentLineIndex++;
            if (CurrentLineIndex >= CurrentScript.lines.Length)
            {
                Stop();
            }
            else
            {
                EmitLine();
            }
        }

        /// <summary>
        /// 强制终止当前对话。
        /// 注意:不会改 GameContext.State —— 调用方决定回到 Title / Playing 等
        /// </summary>
        public void Stop()
        {
            if (!IsRunning) return;
            CurrentScript = null;
            CurrentLineIndex = 0;
            OnSequenceEnd?.Invoke();
        }

        private void EmitLine()
        {
            OnLineShown?.Invoke(CurrentLine);
        }
    }
}
