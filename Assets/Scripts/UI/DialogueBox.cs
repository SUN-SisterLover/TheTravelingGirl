using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheTravelingGirl.UI
{
    /// <summary>
    /// 对话框 UI 组件
    /// 挂在一个 Canvas 下的子物体上，包含说话人名 + 台词文本 + 立绘
    /// 包含打字机效果（可关）
    /// </summary>
    public class DialogueBox : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI speakerText;
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private Image portraitImage;

        [Header("Typewriter")]
        [SerializeField] private bool useTypewriter = true;
        [Range(5f, 120f)]
        [SerializeField] private float charactersPerSecond = 30f;

        private Coroutine _typewriterRoutine;
        private string _pendingText;

        public bool IsVisible =>
            canvasGroup != null && canvasGroup.alpha > 0.5f && gameObject.activeSelf;

        private void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            Hide();
        }

        /// <summary>
        /// 显示一句对话
        /// </summary>
        public void Show(Dialogue.DialogueLine line, Action onComplete)
        {
            gameObject.SetActive(true);
            if (canvasGroup != null) canvasGroup.alpha = 1f;

            if (speakerText != null) speakerText.text = line.speaker ?? string.Empty;
            if (portraitImage != null) portraitImage.enabled = false; // TODO: 接入资源管理器

            _pendingText = line.text ?? string.Empty;

            if (_typewriterRoutine != null) StopCoroutine(_typewriterRoutine);
            _typewriterRoutine = null;

            if (useTypewriter && bodyText != null)
            {
                bodyText.text = string.Empty;
                _typewriterRoutine = StartCoroutine(TypewriterRoutine(_pendingText, onComplete));
            }
            else
            {
                if (bodyText != null) bodyText.text = _pendingText;
                onComplete?.Invoke();
            }
        }

        /// <summary>
        /// 跳过打字机动画，立即显示完整文本
        /// </summary>
        public void CompleteImmediately()
        {
            if (_typewriterRoutine != null)
            {
                StopCoroutine(_typewriterRoutine);
                _typewriterRoutine = null;
            }
            if (bodyText != null) bodyText.text = _pendingText ?? string.Empty;
        }

        /// <summary>
        /// 隐藏对话框
        /// </summary>
        public void Hide()
        {
            if (_typewriterRoutine != null)
            {
                StopCoroutine(_typewriterRoutine);
                _typewriterRoutine = null;
            }
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        private IEnumerator TypewriterRoutine(string fullText, Action onComplete)
        {
            if (bodyText == null) yield break;

            bodyText.text = string.Empty;
            float interval = 1f / Mathf.Max(1f, charactersPerSecond);

            // 用 StringBuilder 避免每帧字符串分配
            var sb = new System.Text.StringBuilder(fullText.Length);
            foreach (char c in fullText)
            {
                sb.Append(c);
                bodyText.text = sb.ToString();
                yield return new WaitForSeconds(interval);
            }

            _typewriterRoutine = null;
            onComplete?.Invoke();
        }
    }
}
