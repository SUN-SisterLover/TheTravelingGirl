# The Traveling Girl — 代码约定

## 命名空间
- 根命名空间: `TheTravelingGirl`
- 子命名空间镜像文件夹结构:
  - `Data/*.cs`     → `TheTravelingGirl.Data`
  - `Runtime/*.cs`  → `TheTravelingGirl.Runtime`
  - `UI/*.cs`       → `TheTravelingGirl.UI`
  - `Save/*.cs`     → `TheTravelingGirl.Save`
  - `Config/*.cs`   → `TheTravelingGirl.Config`

## 目录职责
- `Core/`     — 基础类型 (GameState 枚举) + 全局服务容器 (GameContext)
- `Data/`     — 纯数据类 (POCO),无逻辑,无 MonoBehaviour
- `Runtime/`  — MonoBehaviour,引擎胶水层 (DialogueRunner)
- `UI/`       — uGUI 视图组件 (DialogueBoxView)
- `Save/`     — 存档序列化
- `Config/`   — 玩家设置

## 数据层规则
- 用 `[Serializable]` 标注
- 用 public 字段(`JsonUtility` 要求)
- 不使用 Dictionary / 多态 / 接口字段(`JsonUtility` 限制)
- 尽量不依赖 `UnityEngine`(保持引擎无关)

## 总体原则
- **简单优先**:能明文就明文,不加密/不混淆
- **清晰分层**:Data / Runtime / UI 各司其职
- **事件驱动**:Runtime 通过事件通知 UI,UI 不直接调用 Runtime 业务逻辑
