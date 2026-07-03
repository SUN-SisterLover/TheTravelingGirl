using System;

namespace TheTravelingGirl.Data
{
    /// <summary>
    /// 背景数据（纯数据，无逻辑）
    /// 通过 id 被 DialogueLine.backgroundId 引用
    /// </summary>
    [Serializable]
    public class BackgroundData
    {
        /// <summary>唯一 ID。例如 "bg_classroom"、"bg_rooftop_sunset"</summary>
        public string id;

        /// <summary>玩家可见的显示名（用于编辑器内识别，运行时不一定展示）</summary>
        public string displayName;

        /// <summary>背景图片资源路径（Resources/ 下的相对路径）。例如 "Backgrounds/classroom_day"</summary>
        public string imagePath;
    }
}
