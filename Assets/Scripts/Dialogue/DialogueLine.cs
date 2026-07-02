using System;
using UnityEngine;

namespace TheTravelingGirl.Dialogue
{
    /// <summary>
    /// 单句对话数据（可被 JsonUtility 序列化）
    /// </summary>
    [Serializable]
    public class DialogueLine
    {
        [Tooltip("说话人名字，留空表示旁白")]
        public string speaker;

        [TextArea(2, 5)]
        [Tooltip("台词正文")]
        public string text;

        [Tooltip("立绘资源 ID（例如 'hero_smile'），由资源加载器解析")]
        public string portraitId;

        [Tooltip("背景资源 ID（例如 'bg_classroom'）")]
        public string backgroundId;

        [Tooltip("语音 / 音效资源 ID（可选）")]
        public string audioId;
    }

    /// <summary>
    /// 一段完整对话：有序的 DialogueLine 集合
    /// 通常一个 DialogueSequence 对应一个剧情节点
    /// </summary>
    [Serializable]
    public class DialogueSequence
    {
        [Tooltip("剧情节点的唯一 ID，便于从存档恢复")]
        public string id;

        [Tooltip("对话内容，按数组顺序播放")]
        public DialogueLine[] lines;
    }
}
