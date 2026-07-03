using System;

namespace TheTravelingGirl.Data
{
    /// <summary>
    /// 角色数据（纯数据，无逻辑）
    /// 通过 id 被 DialogueLine.speakerId 引用
    /// </summary>
    [Serializable]
    public class CharacterData
    {
        /// <summary>唯一 ID，用于在 DialogueLine 中引用。例如 "hero"、"heroine_a"</summary>
        public string id;

        /// <summary>玩家可见的显示名。例如 "小鞠"、"Misaki"</summary>
        public string displayName;

        /// <summary>默认立绘资源路径（Resources/ 下的相对路径）。例如 "Portraits/hero_smile"</summary>
        public string defaultPortraitPath;
    }
}
