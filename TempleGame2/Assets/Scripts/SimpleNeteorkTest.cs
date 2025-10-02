using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class SimpleNetworkTest : MonoBehaviour
{
    [Header("UI元件")]
    public Button hostButton;
    public Button clientButton;
    public Text joinCodeDisplayText;
    public InputField joinCodeInputField;

    [Header("狀態顯示")]
    public Text statusText;

    private string currentJoinCode = "";

    async void Start()
    {
        Debug.Log("=== 初始化Unity Services ===");

        // 初始化Unity Services
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            statusText.text = "準備完成 - 選擇創建或加入房間";
            Debug.Log("Unity Services 初始化成功");
        }
        catch (System.Exception e)
        {
            statusText.text = "初始化失敗: " + e.Message;
            Debug.LogError("Unity Services 初始化失敗: " + e);
            return;
        }

        // 設置按鈕事件
        if (hostButton != null)
        {
            hostButton.onClick.AddListener(CreateRelayRoom);
            Debug.Log("Host按鈕事件已設置");
        }

        if (clientButton != null)
        {
            clientButton.onClick.AddListener(JoinRelayRoom);
            Debug.Log("Client按鈕事件已設置");
        }

        // 監聽網路事件
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        Debug.Log("=== 初始化完成 ===");
    }

    async void CreateRelayRoom()
    {
        Debug.Log("=== 開始創建Relay房間 ===");
        statusText.text = "創建房間中...";

        try
        {
            // 創建Relay分配 (最多1個額外玩家 = 總共2人)
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

            // 取得加入代碼
            currentJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // 設置Unity Transport
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            // 啟動Host
            NetworkManager.Singleton.StartHost();

            // 顯示房間代碼
            if (joinCodeDisplayText != null)
            {
                joinCodeDisplayText.text = "房間代碼: " + currentJoinCode;
            }

            statusText.text = "房間已創建！等待信眾加入...";
            Debug.Log($"房間創建成功！代碼: {currentJoinCode}");
        }
        catch (RelayServiceException e)
        {
            statusText.text = "創建房間失敗: " + e.Reason;
            Debug.LogError($"Relay錯誤: {e}");
        }
        catch (System.Exception e)
        {
            statusText.text = "創建房間失敗";
            Debug.LogError($"錯誤: {e}");
        }
    }

    async void JoinRelayRoom()
    {
        if (joinCodeInputField == null)
        {
            Debug.LogError("JoinCodeInputField 未設置");
            return;
        }

        string joinCode = joinCodeInputField.text.Trim().ToUpper();

        if (string.IsNullOrEmpty(joinCode))
        {
            statusText.text = "請輸入房間代碼";
            return;
        }

        Debug.Log($"=== 嘗試加入房間: {joinCode} ===");
        statusText.text = "加入房間中...";

        try
        {
            // 加入Relay
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            // 設置Unity Transport
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            // 啟動Client
            NetworkManager.Singleton.StartClient();

            statusText.text = "正在連接到神明...";
            Debug.Log("Client啟動成功");
        }
        catch (RelayServiceException e)
        {
            statusText.text = "加入房間失敗: " + e.Reason;
            Debug.LogError($"Relay錯誤: {e}");
        }
        catch (System.Exception e)
        {
            statusText.text = "加入房間失敗";
            Debug.LogError($"錯誤: {e}");
        }
    }

    void OnClientConnected(ulong clientId)
    {
        Debug.Log($"玩家連接: {clientId}");

        if (NetworkManager.Singleton.IsHost)
        {
            statusText.text = "信眾已加入！遊戲開始";
        }
        else if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            statusText.text = "成功連接到神明！";
        }
    }

    void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"玩家斷線: {clientId}");
        statusText.text = "連接中斷";
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