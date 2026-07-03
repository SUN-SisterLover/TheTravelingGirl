using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TheTravelingGirl.Data;
using TheTravelingGirl.Runtime;
using TheTravelingGirl.UI;

namespace TheTravelingGirl.Testing
{
    /// <summary>
    /// 最小验证 Bootstrap —— 仅供 DialogueTest.unity 测试用
    ///
    /// 用法:
    ///   - DialogueTest.unity 里有个挂了本脚本的 GameObject
    ///   - 按 Play,代码自动建好 Canvas/对话框/Runner 并播测试对话
    ///   - 任意键 / 鼠标左键 推进对话
    ///
    /// 验证完成后可以整段删掉(连场景一起),不影响生产代码
    ///
    /// 反射设置 DialogueBoxView 私有字段:
    ///   - 不污染 DialogueBoxView 的公开 API
    ///   - 仅为测试性 bootstrap 服务,生产代码不依赖
    /// </summary>
    public class DialogueTestBootstrap : MonoBehaviour
    {
        [Header("Test Content")]
        [Tooltip("格式: 'speakerId|text'，speakerId 留空表示旁白")]
        [SerializeField] private string[] testLines = new[]
        {
            "|第一章",
            "小鞠|你好,旅行者。",
            "小鞠|今天天气真好,要不要一起走走?",
            "|她微微一笑,阳光洒在她的发丝上。"
        };

        [Header("Wiring")]
        [SerializeField] private bool wireRunner = true;

        private void Start()
        {
            var canvas = BuildCanvas();
            Debug.Log("[DialogueTestBootstrap] Canvas built.");
            var (panel, speaker, body) = BuildDialoguePanel(canvas.transform);
            var runner = wireRunner ? BuildRunner() : null;
            AttachView(panel, speaker, body, runner);
            Debug.Log("[DialogueTestBootstrap] UI + Runner wired.");

            if (runner != null)
            {
                runner.Play(CreateTestScript());
                Debug.Log("[DialogueTestBootstrap] Dialogue playing. 按 Space / Enter / 鼠标左键 推进。");
                StartCoroutine(AdvanceOnInput(runner));
            }
            else
            {
                Debug.LogWarning("[DialogueTestBootstrap] wireRunner = false, nothing to play.");
            }
        }

        /// <summary>
        /// 显式设 TMP 字体。AddComponent 创建的 TextMeshProUGUI font 默认为 null,
        /// 不报错但完全不画字,场景看起来"啥都没有"
        /// </summary>
        private static void ConfigureFont(TextMeshProUGUI text, string role)
        {
            var font = TMP_Settings.defaultFontAsset;
            if (font != null)
            {
                text.font = font;
            }
            else
            {
                Debug.LogError(
                    $"[DialogueTestBootstrap] {role} 的 TMP 默认字体找不到! " +
                    "请打开 Window > TextMeshPro > Import TMP Essential Resources 后重试。");
            }
        }

        private static Canvas BuildCanvas()
        {
            var go = new GameObject("DialogueTestCanvas");
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        private static (GameObject panel, TextMeshProUGUI speaker, TextMeshProUGUI body) BuildDialoguePanel(Transform parent)
        {
            // 面板
            var panelGo = new GameObject("DialoguePanel", typeof(RectTransform), typeof(CanvasGroup));
            panelGo.transform.SetParent(parent, false);
            var rt = panelGo.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.anchoredPosition = new Vector2(0, 40);
            rt.sizeDelta = new Vector2(-60, 240);

            var bg = panelGo.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.75f);

            // 说话人
            var speakerGo = new GameObject("SpeakerText", typeof(RectTransform));
            speakerGo.transform.SetParent(panelGo.transform, false);
            var srt = speakerGo.GetComponent<RectTransform>();
            srt.anchorMin = new Vector2(0, 1);
            srt.anchorMax = new Vector2(1, 1);
            srt.pivot = new Vector2(0.5f, 1);
            srt.anchoredPosition = new Vector2(20, -10);
            srt.sizeDelta = new Vector2(-40, 40);

            var speaker = speakerGo.AddComponent<TextMeshProUGUI>();
            ConfigureFont(speaker, "SpeakerText");
            speaker.fontSize = 32;
            speaker.color = new Color(0.7f, 0.9f, 1f);
            speaker.fontStyle = FontStyles.Bold;
            speaker.enableWordWrapping = false;
            speaker.overflowMode = TextOverflowModes.Overflow;
            speaker.alignment = TextAlignmentOptions.MidlineLeft;

            // 台词
            var bodyGo = new GameObject("BodyText", typeof(RectTransform));
            bodyGo.transform.SetParent(panelGo.transform, false);
            var brt = bodyGo.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0, 0);
            brt.anchorMax = new Vector2(1, 1);
            brt.pivot = new Vector2(0.5f, 0.5f);
            brt.offsetMin = new Vector2(20, 20);
            brt.offsetMax = new Vector2(-20, -50);

            var body = bodyGo.AddComponent<TextMeshProUGUI>();
            ConfigureFont(body, "BodyText");
            body.fontSize = 28;
            body.color = Color.white;
            body.enableWordWrapping = true;
            body.alignment = TextAlignmentOptions.TopLeft;

            return (panelGo, speaker, body);
        }

        private static DialogueRunner BuildRunner()
        {
            var go = new GameObject("GameManager");
            return go.AddComponent<DialogueRunner>();
        }

        private static void AttachView(GameObject panel, TextMeshProUGUI speaker, TextMeshProUGUI body, DialogueRunner runner)
        {
            var view = panel.AddComponent<DialogueBoxView>();
            // 反射设私有 [SerializeField] 字段 —— 见类注释
            var type = typeof(DialogueBoxView);
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            type.GetField("runner", flags)?.SetValue(view, runner);
            type.GetField("speakerText", flags)?.SetValue(view, speaker);
            type.GetField("bodyText", flags)?.SetValue(view, body);
        }

        private Script CreateTestScript()
        {
            var lines = new DialogueLine[testLines.Length];
            for (int i = 0; i < testLines.Length; i++)
            {
                var parts = testLines[i].Split(new[] { '|' }, 2);
                lines[i] = new DialogueLine
                {
                    speakerId = parts[0],
                    text = parts.Length > 1 ? parts[1] : string.Empty
                };
            }
            return new Script
            {
                id = "test_script",
                displayName = "测试剧本",
                lines = lines
            };
        }

        private static IEnumerator AdvanceOnInput(DialogueRunner runner)
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            while (true)
            {
                bool advance =
                    (keyboard?.spaceKey.wasPressedThisFrame ?? false) ||
                    (keyboard?.enterKey.wasPressedThisFrame ?? false) ||
                    (mouse?.leftButton.wasPressedThisFrame ?? false);
                if (advance && runner.IsRunning) runner.Advance();
                yield return null;
            }
        }
    }
}
