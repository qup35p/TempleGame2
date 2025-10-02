// Scripts/UIManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_InputField wishInput;
    public Button submitBtn;
    public Button retryBtn;
    public BelieverController believerController;

    void Start()
    {
        submitBtn.onClick.AddListener(OnSubmitClicked);
        retryBtn.onClick.AddListener(OnRetryClicked);
    }

    void OnSubmitClicked()
    {
        string raw = wishInput.text;
        if (string.IsNullOrWhiteSpace(raw))
        {
            Debug.Log("請輸入願望");
            // TODO: 顯示 UI 提示
            return;
        }
        if (!ValidateWish(raw))
        {
            Debug.Log("願望格式不被接受");
            // TODO: 顯示提示
            return;
        }
        believerController.SubmitWish(raw);
        // 切換 UI 至等待神明回應
        UIState_ShowWaiting(true);
    }

    bool ValidateWish(string text)
    {
        // 例：簡單敏感詞過濾、最小長度
        if (text.Length < 2) return false;
        string[] banned = new string[] { "badword1", "badword2" }; // 替換成真實詞彙
        foreach (var b in banned) if (text.Contains(b)) return false;
        return true;
    }

    void OnRetryClicked()
    {
        wishInput.text = "";
        UIState_ShowWaiting(false);
    }

    void UIState_ShowWaiting(bool on)
    {
        // 實作將 WishPanel 隱藏並顯示 WaitingPanel
    }
}
