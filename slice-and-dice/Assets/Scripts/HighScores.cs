using System;
using System.Collections;
using UnityEngine;

public class HighScores : MonoBehaviour
{
    private const string WebURL = "https://eo6srz25g9rxbu1.m.pipedream.net";

    private static HighScores _instance;
    private DisplayHighscores _myDisplay;

    private PlayerScore[] _scoreList;

    private void Awake()
    {
        _instance = this;
        _myDisplay = GetComponent<DisplayHighscores>();
    }

    public static void UploadScore(string username, long score)
    {
        _instance.StartCoroutine(_instance.DatabaseUpload(username, score));
    }

    private IEnumerator DatabaseUpload(string username, long score)
    {
        var www = new WWW(WebURL + "/add/" + WWW.EscapeURL(username) + "/" + score);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
            DownloadScores();
        else
            Debug.LogError("Error uploading" + www.error);
    }

    public void DownloadScores()
    {
        StartCoroutine(nameof(DatabaseDownload));
    }

    private IEnumerator DatabaseDownload()
    {
        var www = new WWW(WebURL + "/pipe/0/10");
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            OrganizeInfo(www.text);
            _myDisplay.SetScoresToMenu(_scoreList);
        }
        else
        {
            Debug.LogError("Error uploading" + www.error);
        }
    }

    private void OrganizeInfo(string rawData)
    {
        var entries = rawData.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
        _scoreList = new PlayerScore[entries.Length];
        for (var i = 0; i < entries.Length; i++)
        {
            var entryInfo = entries[i].Split(new[] {'|'});
            var username = entryInfo[0];
            var score = long.Parse(entryInfo[1]);
            _scoreList[i] = new PlayerScore(username, score);
            Debug.Log(_scoreList[i].Username + ": " + _scoreList[i].Score);
        }
    }
}

public struct PlayerScore
{
    public readonly string Username;
    public readonly long Score;

    public PlayerScore(string username, long score)
    {
        Username = username;
        Score = score;
    }
}