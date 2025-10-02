using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_InputField wishInput;   // 輸入欄
    public Button submitButton;        // 確認按鈕
    public GameObject retryButtonGO;   // Retry 按鈕（GameObject）

    void Start()
    {
        // 綁定按鈕
        submitButton.onClick.AddListener(OnSubmitClicked);
        retryButtonGO.SetActive(false);  // 預設隱藏 Retry
    }

    void OnSubmitClicked()
    {
        string wish = wishInput.text.Trim();

        if (string.IsNullOrEmpty(wish))
        {
            Debug.Log("請輸入願望");
            return;
        }

        Debug.Log("願望輸入：" + wish);

        // 顯示 Retry 按鈕
        retryButtonGO.SetActive(true);
    }

    public void OnRetryClicked()
    {
        // 清空輸入欄
        wishInput.text = "";
        retryButtonGO.SetActive(false);
    }
}
