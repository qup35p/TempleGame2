// Scripts/BelieverController.cs
using System;
using UnityEngine;

[Serializable]
public class BelieverData
{
    public string wish = "";
    public int sincerity = 100; // 誠意度 0-100
    public int yinDe = 0; // 陰德
    public int yeLi = 0; // 業力
}

public class BelieverController : MonoBehaviour
{
    public BelieverData data = new BelieverData();

    public static event Action<BelieverData> OnWishSubmitted; // 跟神明端溝通的事件接口

    // UI 可以呼叫這個方法提交願望
    public void SubmitWish(string wishText)
    {
        data.wish = FormatWish(wishText);
        Debug.Log("願望提交: " + data.wish);
        OnWishSubmitted?.Invoke(data);
    }

    string FormatWish(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "";
        var trimmed = raw.Trim();
        // 範例格式化：限制長度、把多個空白變1個
        trimmed = System.Text.RegularExpressions.Regex.Replace(trimmed, @"\s+", " ");
        if (trimmed.Length > 140) trimmed = trimmed.Substring(0, 140);
        return trimmed;
    }

    // 誠意度調整
    public void ModifySincerity(int delta)
    {
        data.sincerity = Mathf.Clamp(data.sincerity + delta, 0, 100);
        Debug.Log($"誠意度變更: {data.sincerity}");
    }

    // 其他值修改
    public void ModifyYinDe(int delta) { data.yinDe += delta; }
    public void ModifyYeLi(int delta) { data.yeLi += delta; }
}
