using System;

namespace TheTravelingGirl.Data
{
    /// <summary>
    /// 一段完整剧本（纯数据，无逻辑）
    /// 由若干 DialogueLine 组成,按数组顺序播放
    /// 例如一个 Script 对应一个场景/章节
    /// </summary>
    [Serializable]
    public class Script
    {
        /// <summary>唯一 ID。例如 "ch01_intro"、"ch02_heroine_a_route_01"</summary>
        public string id;

        /// <summary>显示名（用于编辑器内识别 / 存档 UI）</summary>
        public string displayName;

        /// <summary>对话内容,按数组顺序播放</summary>
        public DialogueLine[] lines;
    }
}
