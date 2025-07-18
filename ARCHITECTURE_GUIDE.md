# ARCHITECTURE_GUIDE

本文件定義了 **Recall 專案的工程架構與程式碼規範**。  
目的是讓開發者（人類或 AI，例如 Cursor）能快速理解專案結構並維持一致性。

---

## 1. 專案總覽

專案包含兩個主要模組：
- **RecallCore**  
  - 核心遊戲邏輯（動作、角色、記憶系統、GameManager）。
  - 無 UI，純邏輯，可被 Console 測試或 Godot Mono 重用。
- **RecallConsole**  
  - 測試專案（Console App）。
  - 使用 `dotnet run` 驗證回合流程與記憶時間軸。

---

## 2. 資料夾結構

Recall_prototype/
├── RecallCore/
│ ├── Actions/
│ │ ├── IAction.cs
│ │ ├── AttackAction.cs
│ │ ├── BlockAction.cs
│ │ └── ChargeAction.cs
│ ├── Entities/
│ │ ├── Player.cs
│ │ └── Enemy.cs
│ ├── Memory/
│ │ ├── MemoryTimeline.cs
│ │ └── ActionRecord.cs
│ ├── GameManager.cs
│ └── RecallCore.csproj
├── RecallConsole/
│ ├── Program.cs
│ └── RecallConsole.csproj
├── PROJECT_OVERVIEW.md
└── ARCHITECTURE_GUIDE.md

yaml
複製
編輯

---

## 3. 命名空間 (Namespace)

每個資料夾對應一個命名空間：

- `RecallCore.Actions` → IAction、AttackAction、BlockAction、ChargeAction
- `RecallCore.Entities` → Player、Enemy
- `RecallCore.Memory` → MemoryTimeline、ActionRecord
- `RecallCore` → GameManager

**原則**：  
- 類別的命名空間必須與檔案所在資料夾一致。

---

## 4. 程式碼規範

### 4.1 檔案與類別
- 每個檔案只包含一個主要類別/介面。
- 檔案名稱必須與類別名稱一致。

### 4.2 命名規則
- **公開屬性/方法/類別**：PascalCase（例：`public int Health { get; set; }`）
- **私有欄位**：`_camelCase`（例：`private int _health;`）
- **介面**：以 `I` 開頭（例：`IAction`）。

### 4.3 註解
- 使用 **XML Documentation** (`///`) 描述類別與方法的用途。

---

## 5. 開發流程建議

1. **RecallCore 優先**  
   先在 RecallCore 定義遊戲邏輯（例如 IAction、GameManager），Console 只是測試入口。
2. **保持 Console 無邏輯**  
   Console 僅用於輸入/輸出，邏輯完全在 RecallCore。
3. **Memory Timeline**  
   所有玩家行為需記錄到 `MemoryTimeline`，以便未來回放（Echo 卡牌）。

---

## 6. 測試方法

- 切換到 Console 專案：
  ```bash
  cd RecallConsole
  dotnet run