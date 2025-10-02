using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class SimpleNetworkTest : MonoBehaviour
{
    [Header("UI元件")]
    public Button hostButton;
    public Button clientButton;
    public InputField joinCodeInputField;

    [Header("狀態顯示")]
    public Text statusText;
    public Text connectedPlayersText;
    public Text joinCodeDisplayText;

    private string currentJoinCode = "";

    void Start()
    {
        Debug.Log("=== 遊戲啟動 START ===");
        StartCoroutine(InitializeServices());
    }

    IEnumerator InitializeServices()
    {
        Debug.Log("開始初始化...");

        // 初始化顯示
        UpdateUI("正在初始化...", 0, "");

        // 初始化Unity Services
        var initTask = UnityServices.InitializeAsync();
        yield return new WaitUntil(() => initTask.IsCompleted);

        if (initTask.Exception != null)
        {
            Debug.LogError("初始化失敗: " + initTask.Exception);
            UpdateUI("初始化失敗", 0, "");
            yield break;
        }

        Debug.Log("Unity Services 初始化完成");

        // 登入
        var signInTask = AuthenticationService.Instance.SignInAnonymouslyAsync();
        yield return new WaitUntil(() => signInTask.IsCompleted);

        if (signInTask.Exception != null)
        {
            Debug.LogError("登入失敗: " + signInTask.Exception);
            UpdateUI("登入失敗", 0, "");
            yield break;
        }

        Debug.Log("登入成功: " + AuthenticationService.Instance.PlayerId);
        UpdateUI("準備完成", 0, "");

        // 設置按鈕
        if (hostButton != null)
        {
            hostButton.onClick.RemoveAllListeners();
            hostButton.onClick.AddListener(() => StartCoroutine(CreateRoom()));
            Debug.Log("Host按鈕已設置");
        }

        if (clientButton != null)
        {
            clientButton.onClick.RemoveAllListeners();
            clientButton.onClick.AddListener(() => StartCoroutine(JoinRoom()));
            Debug.Log("Client按鈕已設置");
        }

        // 設置網路回調
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            Debug.Log("網路回調已設置");
        }
        else
        {
            Debug.LogError("找不到 NetworkManager!");
        }

        Debug.Log("=== 初始化完成 ===");
    }

    IEnumerator CreateRoom()
    {
        Debug.Log("=== 創建房間 ===");
        UpdateUI("創建中...", 0, "");

        SetButtonsEnabled(false);

        // 創建 Relay
        var allocTask = RelayService.Instance.CreateAllocationAsync(1);
        yield return new WaitUntil(() => allocTask.IsCompleted);

        if (allocTask.Exception != null)
        {
            Debug.LogError("創建失敗: " + allocTask.Exception);
            UpdateUI("創建失敗", 0, "");
            SetButtonsEnabled(true);
            yield break;
        }

        var allocation = allocTask.Result;
        Debug.Log("Allocation 創建成功");

        // 取得代碼
        var codeTask = RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        yield return new WaitUntil(() => codeTask.IsCompleted);

        if (codeTask.Exception != null)
        {
            Debug.LogError("取得代碼失敗: " + codeTask.Exception);
            UpdateUI("取得代碼失敗", 0, "");
            SetButtonsEnabled(true);
            yield break;
        }

        currentJoinCode = codeTask.Result;
        Debug.Log("房間代碼: " + currentJoinCode);

        // 設置 Transport
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
        );

        Debug.Log("Transport 設置完成");

        // 啟動 Host
        bool started = NetworkManager.Singleton.StartHost();
        Debug.Log("StartHost 結果: " + started);

        if (started)
        {
            UpdateUI("房間已創建", 1, "代碼: " + currentJoinCode);
        }
        else
        {
            UpdateUI("啟動失敗", 0, "");
            SetButtonsEnabled(true);
        }
    }

    IEnumerator JoinRoom()
    {
        string code = joinCodeInputField != null ? joinCodeInputField.text.Trim().ToUpper() : "";

        if (string.IsNullOrEmpty(code))
        {
            Debug.LogError("請輸入房間代碼");
            UpdateUI("請輸入代碼", 0, "");
            yield break;
        }

        Debug.Log("=== 加入房間: " + code + " ===");
        UpdateUI("加入中...", 0, "");

        SetButtonsEnabled(false);

        // 加入 Relay
        var joinTask = RelayService.Instance.JoinAllocationAsync(code);
        yield return new WaitUntil(() => joinTask.IsCompleted);

        if (joinTask.Exception != null)
        {
            Debug.LogError("加入失敗: " + joinTask.Exception);
            UpdateUI("加入失敗", 0, "");
            SetButtonsEnabled(true);
            yield break;
        }

        var joinAllocation = joinTask.Result;
        Debug.Log("JoinAllocation 成功");

        // 設置 Transport
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(
            joinAllocation.RelayServer.IpV4,
            (ushort)joinAllocation.RelayServer.Port,
            joinAllocation.AllocationIdBytes,
            joinAllocation.Key,
            joinAllocation.ConnectionData,
            joinAllocation.HostConnectionData
        );

        Debug.Log("Transport 設置完成");

        // 啟動 Client
        bool started = NetworkManager.Singleton.StartClient();
        Debug.Log("StartClient 結果: " + started);

        if (started)
        {
            UpdateUI("連接中...", 0, "");
        }
        else
        {
            UpdateUI("啟動失敗", 0, "");
            SetButtonsEnabled(true);
        }
    }

    void OnClientConnected(ulong clientId)
    {
        Debug.Log($"✅ 玩家連接: {clientId}");

        int count = NetworkManager.Singleton.ConnectedClients.Count;
        Debug.Log($"總玩家數: {count}");

        if (NetworkManager.Singleton.IsHost)
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                UpdateUI("等待玩家...", count, "代碼: " + currentJoinCode);
            }
            else
            {
                UpdateUI("玩家已加入!", count, "代碼: " + currentJoinCode);
            }
        }
        else if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            UpdateUI("連接成功!", count, "");
        }
    }

    void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"❌ 玩家斷線: {clientId}");

        int count = NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening
            ? NetworkManager.Singleton.ConnectedClients.Count
            : 0;

        UpdateUI("玩家離開", count, "");
    }

    void UpdateUI(string status, int playerCount, string joinCode)
    {
        Debug.Log($"[UI更新] 狀態:{status} | 玩家:{playerCount} | 代碼:{joinCode}");

        if (statusText != null)
        {
            statusText.text = status;
        }
        else
        {
            Debug.LogWarning("statusText 是 null");
        }

        if (connectedPlayersText != null)
        {
            connectedPlayersText.text = $"玩家: {playerCount}/2";
        }
        else
        {
            Debug.LogWarning("connectedPlayersText 是 null");
        }

        if (joinCodeDisplayText != null)
        {
            joinCodeDisplayText.text = joinCode;
        }
        else if (!string.IsNullOrEmpty(joinCode))
        {
            Debug.LogWarning("joinCodeDisplayText 是 null");
        }
    }

    void SetButtonsEnabled(bool enabled)
    {
        if (hostButton != null) hostButton.interactable = enabled;
        if (clientButton != null) clientButton.interactable = enabled;
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}