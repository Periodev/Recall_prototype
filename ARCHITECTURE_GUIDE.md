## 模組分層

```
Recall_prototype/
├─ RecallCore/               # 核心邏輯 (無 UI)
│   ├─ Actions/              # IAction + 具體行為 (Attack, Block, Charge, Clear)
│   ├─ Entities/             # Actor (Player/Enemy) 與狀態管理
│   ├─ Memory/                # TimelineManager + MemoryTimeline + ActionRecord
│   ├─ Game/                  # GameLoop (回合流程驅動)
│   └─ Common/                # 全局常數與設定 (GameConstants)
├─ RecallConsole/            # Console 測試程式入口 (Program.cs)
└─ Godot/                    # Godot Mono UI 層 (调用 RecallCore)
```

---

## 核心類別

- **IAction**

  - 定義 `Execute(Actor actor, Actor target)`。

- **Actor**（抽象基底）

  - 欄位：`HP`, `AP`, `shield`。
  - 方法：
    - `AddShield(int amount)` → 增加護盾值。
    - `TakeDamage(int amount)` → 先扣護盾，再扣 HP。
    - `CanAct()`、`GetMaxHP()` 等接口。

- **GameConstants**

  - 存放所有全局參數：
    - 初始 AP、最大 Memory 大小、回合上限等。

- **MemoryTimeline**

  - 基礎行為紀錄：封裝單次 `ActionRecord`。

- **TimelineManager**

  - 可視窗口管理與事件推送：維護固定長度隊列，支援記錄、移除與通知。

- **GameLoop**

  - 回合驅動核心：處理玩家與敵方交替行動、AP 更新、記憶推入與 Echo 卡生成。

---

## Echo 機制

- **並行執行介面**

  - 在 `RecallSystem` 中提供 `ExecuteParallelActions(IEnumerable<IAction> actions)` Stub。
  - 當前：選擇記憶窗口內的若干動作，同步並行執行。
  - 未來擴充：可注入不同策略（如 Basic/Random 等）。

- **Echo 卡生成**

  - 每個回合結束時，根據 `TimelineManager` 當前隊列狀態產生對應 Echo 卡（Parallel 類型）。

---

## 日誌與偵錯

- **game.log**
  - 全回合交互流程輸出：行為紀錄、傷害計算、護盾變化。
  - 方便逐步回放與問題定位。

---

## 測試與持續整合

- **單元測試**

  - 使用 **NUnit** 針對核心邏輯（Actions, Actor, MemoryTimeline 等）撰寫測試。
  - 已達 91 個測項，涵蓋護盾、Charge、Attack、Echo Stub 等。

- **CI/CD Pipeline**

  - 待補：GitHub Actions 配置，包含：
    1. `dotnet build`
    2. `dotnet test`
    3. Console 驗證與打包部署(itch.io/Steam)。

---

以上即為最新的架構指南，已同步現有程式碼結構和設計預留。

