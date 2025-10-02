using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // 公開方法：用於載入指定名稱的場景
    public void LoadSceneByName(string name)
    {
        // 確保場景名稱有效，並執行載入
        SceneManager.LoadScene(name);
    }
}