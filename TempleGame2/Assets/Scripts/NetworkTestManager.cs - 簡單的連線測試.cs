// NetworkTestManager.cs - 修正版
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // 新增這行

public class NetworkTestManager : MonoBehaviour
{
    [Header("測試UI")]
    public Button hostButton;
    public Button clientButton;
    public TextMeshProUGUI statusText; // 改成 TextMeshProUGUI

    async void Start()
    {
        // 初始化Unity Services
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        // 設置按鈕
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);

        statusText.text = "準備就緒 - 選擇Host或Client";
        Debug.Log("網路測試管理器初始化完成");
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        statusText.text = "作為Host啟動 (神明端)";
        Debug.Log("Host模式啟動");
    }

    void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        statusText.text = "作為Client啟動 (信眾端)";
        Debug.Log("Client模式啟動");
    }

    void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    void OnDisable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    void OnClientConnected(ulong clientId)
    {
        Debug.Log($"客戶端連接: {clientId}");
        statusText.text = $"連接成功! Client ID: {clientId}";
    }
}