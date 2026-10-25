# Unity 遊戲匯出與 iPhone 實機部署（含 Xcode 常見錯誤排除）

將 Unity 專案匯出並部署至 iPhone 測試時，Unity 並不會直接產生可安裝的 iOS 檔案（.ipa），而是先輸出原始碼與 Xcode 專案目錄，再透過 macOS 上的 Xcode 進行編譯、簽署與安裝。以下為完整的建置流程與常見錯誤排除指南。

---

## 一、事前準備與裝置設定

在開始匯出專案前，請確認開發環境與手機皆已完成基礎配置：

1. 安裝 Unity iOS 模組：開啟 Unity Hub，點選目前使用版本的齒輪圖示，選擇「Add Modules」並勾選安裝「iOS Build Support」。
2. 安裝 Xcode：從 Mac App Store 下載並安裝 Xcode，並於「Xcode > Settings > Accounts」中登入個人的 Apple ID（免費帳號即可進行實機測試）。
3. 開啟 iPhone 開發者模式：在 iOS 16 及後續版本中，需於 iPhone 前往「設定 > 隱私權與安全性 > 開發者模式（Developer Mode）」，將其開啟並重新啟動手機。
4. 信任電腦：使用傳輸線將 iPhone 連接至 Mac，保持手機螢幕解鎖，並在跳出提示時點選「信任這部電腦」並輸入手機密碼。

---

## 二、從 Unity 匯出 Xcode 專案

1. 切換目標平台：在 Unity 頂部選單開啟「File > Build Profiles」（或舊版的 Build Settings），於左側選擇「iOS」並切換為使用中狀態（Active）。
2. 檢查開發環境路徑：若面板中出現「No Xcode automatically detected」警告，可點擊「Run in Xcode」旁的「Browse」按鈕手動選取 Mac 應用程式資料夾中的 Xcode，或直接於匯出後手動開啟專案。
3. 建立乾淨的匯出資料夾：點擊右下角的「Build」按鈕。
   【關鍵避坑提示】：切勿將匯出資料夾儲存在開啟了 iCloud 同步的「桌面（Desktop）」或「文件（Documents）」中，且資料夾路徑請勿包含空白字元。建議直接保存在使用者根目錄下（例如 /Users/你的名稱/HW3），避免 macOS 附加隱藏的延伸屬性導致後續腳本執行失敗。

---

## 三、Xcode 專案設定與簽署

匯出完成後，打開資料夾並雙擊「Unity-iPhone.xcodeproj」啟動 Xcode。

1. 忽略更新建議警告：左側導覽列若出現藍底的「Update to recommended settings」警告，請勿點擊更新。一旦套用 Xcode 的建議設定，將會開啟嚴格的模組檢查，導致 UnityFramework 出現大量標頭檔（Header）編譯錯誤。
2. 選取實機作為執行目標：在 Xcode 視窗最上方的中間位置，點擊目前的裝置選單（預設可能為模擬器或 Any iOS Device），並切換為已連接的實體 iPhone。
3. 設定憑證與 Bundle Identifier：
   - 按下快捷鍵 Cmd + 1 切換至左側的 Project Navigator（資料夾圖示），點選最頂端的藍色「Unity-iPhone」專案圖示。
   - 若中間沒有看到 TARGETS 欄位，請點擊主編輯區左上角的小側邊欄圖示展開清單。
   - 在「TARGETS」欄位下選取「Unity-iPhone」，並切換至「Signing & Capabilities」分頁。
   - 勾選「Automatically manage signing」，並在「Team」下拉選單選擇你的個人帳號（Personal Team）。
   - 將「Bundle Identifier」從預設的「com.DefaultCompany.專案名稱」修改為獨一無二的名稱（例如 com.yourname.vrhw3），避免因預設名稱已被他人註冊而無法產生描述檔（Provisioning Profile）。

---

## 四、Xcode 常見編譯錯誤與排除方法

在按下左上角 Play（Cmd + R）進行編譯時，若遇到以下紅字錯誤，可依照對應步驟排除：

### 錯誤 1：Communication with Apple failed / No profiles for '...' were found
- 發生原因：未連接實機、未將實機設為目標裝置，或使用了重複的預設 Bundle ID。
- 解決方法：確認 iPhone 已解鎖並選為頂部執行目標（Run Destination），將 Bundle Identifier 修改為獨一無二的名稱後，點擊「Try Again」。

### 錯誤 2：Command PhaseScriptExecution failed with a nonzero exit code（於 GameAssembly）
- 發生原因：Xcode 15/16 預設開啟沙盒限制，阻擋 Unity IL2CPP 轉換腳本；或資料夾帶有 iCloud 權限屬性。
- 解決方法：
  1. 點選「PROJECT > Unity-iPhone > Build Settings」（上方篩選切至 All），在右上角搜尋「User Script Sandboxing」並將其改為「No」（建議連同 TARGETS 下的 GameAssembly 也一併確認為 No）。
  2. 若仍失敗，開啟 Mac 終端機（Terminal），輸入「xattr -cr /你的專案資料夾路徑」清除隱藏權限屬性。

### 錯誤 3：double-quoted include "LifeCycleListener.h" in framework header 等 13 項 UnityFramework 錯誤
- 發生原因：誤點了「Update to recommended settings」，觸發嚴格的 Clang 模組驗證。
- 解決方法：
  - 方法 A（最快）：刪除匯出資料夾，直接回 Unity 重新 Build 一份乾淨的 Xcode 專案，並忽略更新設定警告。
  - 方法 B（手動復原）：於「PROJECT > Unity-iPhone > Build Settings」搜尋「Quoted Include In Framework Header」改為「No」；接著點選「TARGETS > UnityFramework > Build Settings」，將「Defines Module」改為「No」，最後按 Shift + Cmd + K 清除快取（Clean Build Folder）再重新編譯。

---

## 五、鑰匙圈授權與手機端信任憑證

1. 授權鑰匙圈簽署（codesign）：當編譯來到最後階段時，macOS 會跳出視窗顯示「codesign wants to access key "Apple Development: ..." in your keychain」。請輸入「Mac 的開機登入密碼」（非 Apple ID 密碼），並用滑鼠點擊左下角的「永遠允許（Always Allow）」。切勿直接按鍵盤 Enter（預設會觸發單次「允許」，導致每次編譯都要重複輸入數次密碼）。
2. 在 iPhone 上信任開發者憑證：當 App 圖示成功出現在 iPhone 主畫面後，首次點擊若提示「不受信任的開發者」，請在 iPhone 上前往「設定 > 一般 > VPN 與裝置管理」，點選「開發者 App」下方的 Apple ID，並點選「信任」。完成後即可正常開啟並遊玩你的 Unity 遊戲。
