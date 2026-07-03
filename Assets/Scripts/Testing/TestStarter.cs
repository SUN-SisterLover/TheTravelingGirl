using UnityEngine;
using UnityEngine.InputSystem;
using TheTravelingGirl.Data;
using TheTravelingGirl.Core;

namespace TheTravelingGirl.Testing
{
    /// <summary>
    /// 手挂测试启动器
    /// 挂到任意 GameObject 上,Start() 创建并播放一段测试对话
    /// Update() 监听 Space/Enter/鼠标左键 推进
    ///
    /// 假设场景里已经有 GameManager(挂了 DialogueRunner)
    /// </summary>
    public class TestStarter : MonoBehaviour
    {
        private void Start()
        {
            if (GameContext.DialogueRunner == null)
            {
                Debug.LogError("[TestStarter] 找不到 DialogueRunner。请确认场景里有 GameManager 挂了 DialogueRunner 脚本。");
                return;
            }

            var script = new Script
            {
                id = "manual_test",
                displayName = "测试剧本",
                lines = new[]
                {
                    new DialogueLine { speakerId = "", text = "第一章" },
                    new DialogueLine { speakerId = "小鞠", text = "你好,旅行者。" },
                    new DialogueLine { speakerId = "小鞠", text = "今天天气真好,要不要一起走走?" },
                    new DialogueLine { speakerId = "", text = "她微微一笑,阳光洒在她的发丝上。" }
                }
            };

            GameContext.DialogueRunner.Play(script);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            bool advance =
                (keyboard?.spaceKey.wasPressedThisFrame ?? false) ||
                (keyboard?.enterKey.wasPressedThisFrame ?? false) ||
                (mouse?.leftButton.wasPressedThisFrame ?? false);

            if (advance && GameContext.DialogueRunner != null && GameContext.DialogueRunner.IsRunning)
            {
                GameContext.DialogueRunner.Advance();
            }
        }
    }
}
