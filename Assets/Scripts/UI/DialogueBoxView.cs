using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using TheTravelingGirl.Data;
using TheTravelingGirl.Runtime;

namespace TheTravelingGirl.UI
{
    /// <summary>
    /// 对话框 UI 视图
    /// 订阅 DialogueRunner 的事件,显示说话人 + 台词文本
    /// 包含可选的打字机效果
    /// </summary>
    public class DialogueBoxView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DialogueRunner runner;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI speakerText;
        [SerializeField] private TextMeshProUGUI bodyText;

        [Header("Typewriter")]
        [SerializeField] private bool useTypewriter = true;
        [Range(5f, 120f)]
        [SerializeField] private float charactersPerSecond = 30f;

        private Coroutine typewriterRoutine;
        private StringBuilder textBuffer = new StringBuilder(256);

        private void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            // runner 在 Inspector 没拖时,自动从场景里找
            if (runner == null) runner = FindFirstObjectByType<DialogueRunner>();
            Hide();
        }

        private void OnEnable()
        {
            if (runner != null)
            {
                runner.OnLineShown += HandleLineShown;
                runner.OnSequenceEnd += HandleSequenceEnd;
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
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
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
