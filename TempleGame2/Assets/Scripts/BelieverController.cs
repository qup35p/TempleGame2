using System;
using UnityEngine;

[System.Serializable]
public class BelieverData
{
    public string wish = "";
    public int sincerity = 100;
    public int yinDe = 0;
    public int yeLi = 0;
    // 其他欄位：attemptCount, lastResponseTimestamp 等
    public int failCount = 0;
}

public class BelieverController : MonoBehaviour
{
    public BelieverData data = new BelieverData();

    public static event Action<BelieverData> OnWishSubmitted;

    public delegate void DataChanged(BelieverData data);
    public event DataChanged OnDataChanged;

    // 新增：取得當前願望
    public string CurrentWish => data.wish;

    // 新增：設定願望
    public void SetWish(string wishText)
    {
        data.wish = FormatWish(wishText);
        Debug.Log("願望設定: " + data.wish);
        NotifyChange();
    }

    public void SubmitWish(string wishText)
    {
        data.wish = FormatWish(wishText);
        Debug.Log("願望提交: " + data.wish);
        OnWishSubmitted?.Invoke(data);
        NotifyChange();
    }

    string FormatWish(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "";
        var trimmed = raw.Trim();
        trimmed = System.Text.RegularExpressions.Regex.Replace(trimmed, @"\s+", " ");
        if (trimmed.Length > 140) trimmed = trimmed.Substring(0, 140);
        return trimmed;
    }

    void NotifyChange()
    {
        OnDataChanged?.Invoke(data);
    }

    public void ModifySincerity(int delta)
    {
        data.sincerity = Mathf.Clamp(data.sincerity + delta, 0, 100);
        Debug.Log($"誠意度變更: {data.sincerity}");
        NotifyChange();
    }

    public void ModifyYinDe(int delta)
    {
        data.yinDe += delta;
        NotifyChange();
    }

    public void ModifyYeLi(int delta)
    {
        data.yeLi += delta;
        NotifyChange();
    }
}