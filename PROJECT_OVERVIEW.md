# Recall 專案總覽

## 專案簡介
**Recall** 是一款以 **記憶時間軸（Memory Timeline）** 為核心的回合制策略遊戲。  
玩家的每個行動都會被記錄，並可在之後重用這些「記憶行動（Echo Cards）」，形成連招與策略。

### 核心玩法
- 玩家與敵人以 **回合制**執行行動：`Attack`, `Block`, `Charge`。
- 每個回合玩家有 **行動點數 (AP)**，行動會消耗 AP。
- 玩家行為會記錄到 **Memory Timeline**，並可在後續回合中「重播」這些記憶行動（Echo）。
- 敵人可以干擾玩家的記憶時間軸，導致記憶動作失效或扭曲。

---

## 模組劃分
專案採用 **分層架構**，便於 Console MVP 和 Godot 前端共用邏輯：


### **RecallCore**
- **責任**：遊戲核心邏輯與資料結構。
- **包含**：
  - `GameManager`：控制回合流程。
  - `Player`, `Enemy`：角色資料（HP、AP）。
  - `IAction`：動作介面（`Attack`, `Block`, `Charge`）。
  - `MemoryTimeline`：記錄並回放行為。

### **RecallConsole**
- **責任**：純 Console 模擬。
- **特色**：`dotnet run` 即可模擬 2~3 回合戰鬥，顯示玩家輸入的行動與結果。

---

## 開發路線圖

### **MVP 階段**
1. **核心動作系統**  
   - 定義 `IAction`。
   - 實作 `AttackAction`, `BlockAction`, `ChargeAction`。
   - 完成基本戰鬥流程（玩家 & 敵人輪流行動）。

2. **Console 測試戰鬥**  
   - Console 顯示狀態（HP, AP）。
   - 玩家輸入 `1=Attack, 2=Block, 3=Charge`。

3. **Memory Timeline 雛形**  
   - 使用 `List<ActionRecord>` 記錄玩家過去 3 回合行為。
   - Console 顯示過去行為。

### **之後**
- 加入「Echo」機制。
- 與 Godot Mono 前端整合。

---

## 開發任務清單（TODO）

### **RecallCore**
- [ ] 定義 `IAction` 介面。
- [ ] 建立 `AttackAction`, `BlockAction`, `ChargeAction`，並包含 `Cost` 和 `Execute()`。
- [ ] 擴展 `GameManager`：
  - 管理玩家與敵人的回合。
  - 驗證 AP 是否足夠。
- [ ] 增加 `MemoryTimeline` 類別。

### **RecallConsole**
- [ ] 在 `Program.cs` 顯示玩家/敵人狀態。
- [ ] 讀取玩家輸入並執行相應動作。
- [ ] 在每回合結束後顯示 Memory Timeline。

---

## 使用說明
- **編譯測試**：  
  ```bash
  cd RecallConsole
  dotnet run


