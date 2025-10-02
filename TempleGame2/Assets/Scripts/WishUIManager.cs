using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WishUIManager : MonoBehaviour
{
    public TMP_InputField wishInput;
    public Button submitButton;
    public GameObject retryButtonGO;
    public BelieverController believerController;

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmitClicked);
        retryButtonGO.SetActive(false);

        // 訂閱信徒資料更新事件（修正：加上參數）
        believerController.OnDataChanged += UpdateUI;
    }

    void OnDestroy()
    {
        // 記得取消訂閱，避免記憶體洩漏
        believerController.OnDataChanged -= UpdateUI;
    }

    void OnSubmitClicked()
    {
        string wish = wishInput.text.Trim();
        if (string.IsNullOrEmpty(wish))
        {
            Debug.Log("請輸入願望！");
            return;
        }

        Debug.Log("願望輸入：" + wish);

        // 觸發信徒更新
        believerController.SetWish(wish);

        // 顯示 Retry 按鈕
        retryButtonGO.SetActive(true);
    }

    public void OnRetryClicked()
    {
        wishInput.text = "";
        retryButtonGO.SetActive(false);
    }

    // 修正：加上 BelieverData 參數
    void UpdateUI(BelieverData data)
    {
        // 同步顯示信徒的願望
        wishInput.text = data.wish;
        Debug.Log($"UI 已更新：願望={data.wish}, 誠意={data.sincerity}");

        // 你也可以更新其他 UI 元素，例如：
        // sincerityText.text = $"誠意度: {data.sincerity}";
        // yinDeText.text = $"陰德: {data.yinDe}";
    }
}