using System;
using UnityEngine;
using TheTravelingGirl.UI;

namespace TheTravelingGirl.Dialogue
{
    /// <summary>
    /// 对话流程控制器
    /// 负责：
    ///   1. 接收 DialogueSequence
    ///   2. 按顺序通知 DialogueBox 显示每一行
    ///   3. 等待玩家点击 / 按键推进
    /// </summary>
    public class DialogueController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DialogueBox dialogueBox;

        /// <summary>对话段开始播放时触发</summary>
        public event Action OnSequenceStart;

        /// <summary>每句台词开始显示时触发（可在 UI 层订阅切换立绘 / 背景）</summary>
        public event Action<DialogueLine> OnLineShown;

        /// <summary>对话段播放完毕时触发</summary>
        public event Action OnSequenceEnd;

        private DialogueLine[] _currentLines;
        private int _index;
        private bool _isTyping;
        private Coroutine _waitForInputRoutine;

        public bool IsRunning => _currentLines != null && _currentLines.Length > 0;

        private void Awake()
        {
            if (dialogueBox == null)
            {
                dialogueBox = FindFirstObjectByType<DialogueBox>();
            }
        }

        /// <summary>
        /// 开始播放一段对话
        /// </summary>
        public void PlaySequence(DialogueSequence sequence)
        {
            if (sequence == null || sequence.lines == null || sequence.lines.Length == 0)
            {
                Debug.LogWarning("[DialogueController] PlaySequence: empty sequence.");
                return;
            }

            _currentLines = sequence.lines;
            _index = 0;
            _isTyping = false;
            if (dialogueBox != null) dialogueBox.gameObject.SetActive(true);

            OnSequenceStart?.Invoke();
            ShowCurrentLine();
        }

        /// <summary>
        /// 玩家按下"继续"键（鼠标左键 / 空格 / 回车）时调用
        ///   - 还在打字：直接显示完整文本
        ///   - 已经显示完整：进入下一句
        ///   - 已是最后一句：结束对话
        /// </summary>
        public void Advance()
        {
            if (!IsRunning) return;

            if (_isTyping)
            {
                CompleteCurrentLineImmediately();
                return;
            }

            _index++;
            if (_index >= _currentLines.Length)
            {
                EndSequence();
            }
            else
            {
                ShowCurrentLine();
            }
        }

        /// <summary>
        /// 强制终止当前对话
        /// </summary>
        public void Stop()
        {
            if (!IsRunning) return;
            EndSequence();
        }

        private void ShowCurrentLine()
        {
            var line = _currentLines[_index];
            _isTyping = true;
            OnLineShown?.Invoke(line);

            if (dialogueBox != null)
            {
                dialogueBox.Show(line, OnLineTypingFinished);
            }
            else
            {
                Debug.LogWarning("[DialogueController] No DialogueBox assigned; skipping visual.");
                _isTyping = false;
            }
        }

        private void OnLineTypingFinished()
        {
            _isTyping = false;
        }

        private void CompleteCurrentLineImmediately()
        {
            if (dialogueBox != null) dialogueBox.CompleteImmediately();
            _isTyping = false;
        }

        private void EndSequence()
        {
            _currentLines = null;
            _index = 0;
            _isTyping = false;
            if (dialogueBox != null) dialogueBox.Hide();
            OnSequenceEnd?.Invoke();
        }
    }
}
