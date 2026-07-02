using System;
using System.IO;
using UnityEngine;

namespace TheTravelingGirl.Save
{
    /// <summary>
    /// 存档数据（JsonUtility 友好）
    /// 想加新字段直接在这里加，旧的 .json 仍然能读（缺字段为默认值）
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public string saveId;                // 存档槽 ID
        public string sceneName;             // 当前场景名
        public float playTimeSeconds;        // 累计游玩时长
        public string currentDialogueId;     // 当前对话段 ID（用于恢复剧情）
        public int currentLineIndex;         // 当前台词在段中的索引
        public string[] unlockedFlags;       // 玩家解锁的剧情标记
        public long savedAtUnixTime;         // 保存时刻（UTC 秒）
    }

    /// <summary>
    /// 简单的 JSON 存档系统
    /// 文件位置：Application.persistentDataPath/Saves/save_<slotId>.json
    /// 不同平台路径：
    ///   Windows: %userprofile%/AppData/LocalLow/<company>/<product>/Saves/
    ///   macOS:   ~/Library/Application Support/<company>/<product>/Saves/
    ///   Linux:   ~/.config/unity3d/<company>/<product>/Saves/
    /// </summary>
    public static class SaveSystem
    {
        private const string SaveFolder = "Saves";
        private const string SaveFilePrefix = "save_";
        private const string SaveFileExtension = ".json";

        public static string GetSaveDirectory()
        {
            return Path.Combine(Application.persistentDataPath, SaveFolder);
        }

        public static string GetSavePath(string slotId)
        {
            return Path.Combine(
                GetSaveDirectory(),
                $"{SaveFilePrefix}{slotId}{SaveFileExtension}"
            );
        }

        public static bool Exists(string slotId)
        {
            return File.Exists(GetSavePath(slotId));
        }

        public static bool Save(string slotId, SaveData data)
        {
            if (string.IsNullOrEmpty(slotId))
            {
                Debug.LogError("[SaveSystem] slotId is null or empty.");
                return false;
            }
            if (data == null)
            {
                Debug.LogError("[SaveSystem] data is null.");
                return false;
            }

            try
            {
                data.savedAtUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                data.saveId = slotId;

                string json = JsonUtility.ToJson(data, prettyPrint: true);
                string path = GetSavePath(slotId);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, json);
                Debug.Log($"[SaveSystem] Saved slot '{slotId}' -> {path}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Save failed: {e}");
                return false;
            }
        }

        public static SaveData Load(string slotId)
        {
            try
            {
                string path = GetSavePath(slotId);
                if (!File.Exists(path)) return null;
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<SaveData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Load failed: {e}");
                return null;
            }
        }

        public static bool Delete(string slotId)
        {
            try
            {
                string path = GetSavePath(slotId);
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Delete failed: {e}");
                return false;
            }
        }

        public static string[] ListSlots()
        {
            try
            {
                string dir = GetSaveDirectory();
                if (!Directory.Exists(dir)) return Array.Empty<string>();

                var files = Directory.GetFiles(dir, $"{SaveFilePrefix}*{SaveFileExtension}");
                var slots = new System.Collections.Generic.List<string>(files.Length);
                foreach (var f in files)
                {
                    var name = Path.GetFileNameWithoutExtension(f);
                    if (name.StartsWith(SaveFilePrefix))
                    {
                        slots.Add(name.Substring(SaveFilePrefix.Length));
                    }
                }
                return slots.ToArray();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] ListSlots failed: {e}");
                return Array.Empty<string>();
            }
        }
    }
}
