// NetworkConnectionTest.cs - 雙人測試用
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkConnectionTest : MonoBehaviour
{
    [Header("連線狀態顯示")]
    public Text connectionStatusText;      // 改為 Text
    public Text playerRoleText;            // 改為 Text
    public Text connectedPlayersText;      // 改為 Text

    void Update()
    {
        if (NetworkManager.Singleton == null) return;

        // 更新連線狀態
        if (NetworkManager.Singleton.IsHost)
        {
            playerRoleText.text = "角色: 神明 (Host)";
            connectionStatusText.text = "狀態: Host運行中";
            connectedPlayersText.text = $"已連接玩家數: {NetworkManager.Singleton.ConnectedClients.Count}";
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            playerRoleText.text = "角色: 信眾 (Client)";
            connectionStatusText.text = "狀態: Client已連接";
            connectedPlayersText.text = $"已連接玩家數: {NetworkManager.Singleton.ConnectedClients.Count}";
        }
        else
        {
            playerRoleText.text = "角色: 未連接";
            connectionStatusText.text = "狀態: 未啟動";
            connectedPlayersText.text = "已連接玩家數: 0";
        }
    }
}
