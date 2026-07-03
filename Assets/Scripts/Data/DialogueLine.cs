using System;

namespace TheTravelingGirl.Data
{
    /// <summary>
    /// 单句对话数据（纯数据，无逻辑）
    /// Script 持有 DialogueLine[]，按顺序播放
    /// </summary>
    [Serializable]
    public class DialogueLine
    {
        /// <summary>
        /// 说话人 ID（对应 CharacterData.id）。
        /// 留空表示旁白/无说话人。
        /// </summary>
        public string speakerId;

        /// <summary>台词正文</summary>
        public string text;

        /// <summary>语音资源路径（可选）。例如 "Voice/ch01_001"</summary>
        public string voicePath;
    }
}
