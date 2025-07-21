# 《Recall》開發日誌 - 2025-07-21

---

## 📌 事項類 (Issues / To-Do)
- [#1] `ExecuteParallelActions()` 尚未實作
- [#2] Echo 並行測試覆蓋尚不完整
- [#3] GameConstants 的使用規範待全面自動化
- [#4] Recall 當回合選取限制需補充 UI 提示
- [#5] Godot UI 尚未串接 Core 模型
- [#6] GitHub Actions CI Pipeline 尚未建立

---

## 📊 進度追蹤 (Milestone Progress)

| 階段                | 子任務完成率 | 狀態     | 預計完成日 | 實際完成日 |
|---------------------|--------------|----------|------------|------------|
| **Prototype-Proto** | 4/4 (100%)   | ✅ 完成  | 2025-07-28 | -          |
| **Prototype**       | 5/9 (55%)    | 穩定推進 | 2025-08-10 | -          |
| **Alpha**           | 0/5 (0%)     | 未開始   | 2025-08-30 | -          |
| **Beta**            | 0/3 (0%)     | 未開始   | 2025-09-15 | -          |
| **MVP**             | 0/2 (0%)     | 未開始   | 2025-09-30 | -          |

---

## 🗓 行程表 (Daily Log)

### 2025-07-21

**今日完成：**

🧠 **Echo 記憶重現與行為邏輯**
- ✅ 檢查 `EchoExecutor.ExecuteEchoCard()`：確認 Echo 僅消耗卡內蓄力，正確配對攻擊與重擊。
- ✅ 完整測試混合行動組合行為（A/C 混合、C 多餘、攻擊多餘）。
- ✅ 修正測試如 `EchoWithMixedActions_ShouldHandleCorrectly` 使其行為與邏輯一致。

🧪 **單元測試標準化**
- ✅ 移動 `GameConstants` 至 `RecallCore/Constants`，統一管理
- ✅ 將所有測試檔案中的數字（HP、攻擊力、護盾值）全部改為常數運算式
- ✅ 玩家與敵人傷害參數完全分離
- ✅ 所有預期值不再寫死，全部改為變數計算
- ✅ 所有測項更新為常數對應，**91 項測試全部通過**

🔁 **Recall 限制規則更新**
- ✅ 設定新規則：Recall 僅能選取「過去回合」的操作，當前回合輸入動作不允許被回溯。
- ✅ 修改 Timeline 判斷與選取範圍，排除當回合行為。
- ✅ 測試邏輯與回響生成同步調整。

📄 **開發規範更新**
- `development_guidelines.md` 新增：所有測試應使用 local constants，不得硬編碼。
- 協助你建立搜尋與範例推廣規範。

**進度狀態**：**正常**

**延遲原因 / 決策紀錄：**
- Echo 的 Charge/Attack 行為需先理清邏輯，Recall 限制調整延後 `ExecuteParallelActions()` 撰寫。

**明日計劃：**
- 撰寫 `ExecuteParallelActions()` 具體邏輯
- 建立 Echo 並行重現的測試場景（多目標 / 混合型）
- 開始構建 Godot UI 綁定初步測試版本

---

## 📅 行程預計表
- **Prototype-Proto**：2025-07-10 ～ 2025-07-28
- **Prototype**：2025-07-29 ～ 2025-08-10
- **Alpha**：2025-08-11 ～ 2025-08-30
- **Beta**：2025-09-01 ～ 2025-09-15
- **MVP**：2025-09-16 ～ 2025-09-30
