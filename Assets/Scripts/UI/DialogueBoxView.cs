using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using TheTravelingGirl.Data;
using TheTravelingGirl.Core;
using TheTravelingGirl.Runtime;

namespace TheTravelingGirl.UI
{
    /// <summary>
    /// 对话框 UI 视图
    /// 订阅 DialogueRunner 的事件,显示说话人 + 台词文本
    /// 包含可选的打字机效果
    ///
    /// 设计要点:
    ///   - runner 优先取 Inspector 引用,未设置时 FindFirstObjectByType 自动找
    ///   - 事件订阅放在 OnEnable / 反订阅在 OnDisable,避免脚本禁用时回调泄漏
    ///   - 打字机用 StringBuilder 累积,避免每帧字符串拼接产生 GC 分配
    /// </summary>
    public class DialogueBoxView : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("可选:未拖时会在 Awake 自动从场景里找 DialogueRunner")]
        [SerializeField] private DialogueRunner runner;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI speakerText;
        [SerializeField] private TextMeshProUGUI bodyText;

        [Header("Typewriter")]
        [SerializeField] private bool useTypewriter = true;
        [Range(5f, 120f)]
        [SerializeField] private float charactersPerSecond = 30f;

        private Coroutine typewriterRoutine;
        // 复用一个 StringBuilder,避免每次打字机重新分配
        private readonly StringBuilder textBuffer = new StringBuilder(256);

        private void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (runner == null) runner = FindFirstObjectByType<DialogueRunner>();
            Hide();
        }

        private void OnEnable()
        {
            // 兜底:Awake 时 FindFirstObjectByType 可能找不到 runner
            // (GameManager 的 Awake 还没跑)。再从全局服务容器查一次
            if (runner == null) runner = GameContext.DialogueRunner;

            // 订阅事件。注意:OnEnable / OnDisable 配对,
            // 防止脚本被禁用后 runner 仍回调到这里
            if (runner != null)
            {
                runner.OnLineShown += HandleLineShown;
                runner.OnSequenceEnd += HandleSequenceEnd;
            }
            else
            {
                Debug.LogWarning(
                    "[DialogueBoxView] runner 引用为空,无法订阅事件。" +
                    "请在 Inspector 把 GameManager 拖到 Runner 字段," +
                    "或确保场景里有挂了 DialogueRunner 的 GameObject。");
            }
        }

        private void OnDisable()
        {
            if (runner != null)
            {
                runner.OnLineShown -= HandleLineShown;
                runner.OnSequenceEnd -= HandleSequenceEnd;
            }
        }

        public void Show(DialogueLine line)
        {
            if (line == null) return;

            gameObject.SetActive(true);
            if (canvasGroup != null) canvasGroup.alpha = 1f;
            if (speakerText != null) speakerText.text = line.speakerId ?? string.Empty;

            // 打断上一次未完成的打字机,避免新句子被旧协程污染
            if (typewriterRoutine != null) StopCoroutine(typewriterRoutine);
            typewriterRoutine = null;

            string fullText = line.text ?? string.Empty;
            if (useTypewriter && bodyText != null)
            {
                bodyText.text = string.Empty;
                typewriterRoutine = StartCoroutine(TypewriterRoutine(fullText));
            }
            else
            {
                if (bodyText != null) bodyText.text = fullText;
            }
        }

        public void Hide()
        {
            if (typewriterRoutine != null) StopCoroutine(typewriterRoutine);
            typewriterRoutine = null;
            // 只关视觉 (alpha=0),不动 GameObject 的激活状态
            // —— 否则 GameObject 不激活时 OnEnable 不跑,事件订阅就没了
            if (canvasGroup != null) canvasGroup.alpha = 0f;
        }

        private void HandleLineShown(DialogueLine line) => Show(line);
        private void HandleSequenceEnd() => Hide();

        private IEnumerator TypewriterRoutine(string fullText)
        {
            if (bodyText == null) yield break;

            bodyText.text = string.Empty;
            float interval = 1f / Mathf.Max(1f, charactersPerSecond);

            textBuffer.Clear();
            foreach (char c in fullText)
            {
                textBuffer.Append(c);
                bodyText.text = textBuffer.ToString();
                yield return new WaitForSeconds(interval);
            }

            typewriterRoutine = null;
        }
    }
}
