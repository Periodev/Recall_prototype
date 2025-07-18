# 架構指南 (Architecture Guide)

此文件為系統設計與模組劃分參考。

## 模組分層
Recall_prototype/
├─ RecallCore/ # 核心邏輯 (無 UI)
│ ├─ Actions/ # IAction + 具體行為 (Attack, Block, Charge)
│ ├─ Entities/ # Actor (Player/Enemy)
│ ├─ Memory/ # MemoryTimeline + ActionRecord
│ ├─ Game/ # GameLoop (控制回合流程)
│ └─ ...
├─ RecallConsole/ # Console 測試程式
│ └─ Program.cs
├─ Godot/ # 未來 Godot Mono 介面

markdown
複製
編輯

## 核心類別
- `IAction`：定義 `Execute(Actor actor, Actor target)`
- `Actor`：抽象基底類別 (HP, Energy, ActionList)
  - `Player`、`Enemy` 繼承之
- `MemoryTimeline`：紀錄行為序列，用於 Echo 機制
- `GameLoop`：負責回合驅動，支援 Console 測試與後續 Godot UI

## 目標
- Console 版本可直接 `dotnet run` 測試。
- **不依賴 Godot**，可獨立運行。
- 支援記錄所有回合結果 (`game.log`)。

---