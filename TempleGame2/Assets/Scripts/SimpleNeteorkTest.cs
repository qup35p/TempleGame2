using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SimpleNetworkTest : MonoBehaviour
{
    public Button hostButton;
    public Button clientButton;

    void Start()
    {
        hostButton.onClick.AddListener(OnHostButtonClicked);
        clientButton.onClick.AddListener(OnClientButtonClicked);
        Debug.Log("腳本初始化完成");
    }

    void OnHostButtonClicked()
    {
        Debug.Log("Host按鈕被點擊");
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host啟動成功");
        }
    }

    void OnClientButtonClicked()
    {
        Debug.Log("Client按鈕被點擊");
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client啟動成功");
        }
    }
}
