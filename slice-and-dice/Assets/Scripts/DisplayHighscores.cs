using System.Collections;
using TMPro;
using UnityEngine;

public class DisplayHighscores : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private bool isGameOverScreen;
    private long _score;
    private string _template;

    private HighScores _myScores;

    private void Start()
    {
        _template = text.text;
        if (isGameOverScreen)
        {
            var gameManager = FindObjectOfType<GameManager>();
            _score = gameManager ? gameManager.Score : 0;
            SetText(_score.ToString(), "...checking if high score...");
        }
        else
        {
            SetText("...loading...", "");
        }

        _myScores = GetComponent<HighScores>();
        StartCoroutine("RefreshHighscores");
    }

    private void SetText(string score, string message)
    {
        text.text = _template.Replace("{{score}}", score).Replace("{{was_high_score}}", message);
    }

    public void SetScoresToMenu(PlayerScore[] highscoreList)
    {
        if (highscoreList.Length > 0)
        {
            var highScore = highscoreList[0].Score;
            if (isGameOverScreen)
            {
                if (_score >= highScore)
                    SetText(_score.ToString(), "Take A Screenshot! You Set The HighScore!");
                else
                    SetText(_score.ToString(), "Current HighScore " + highScore);
            }
            else
            {
                SetText(highScore.ToString(), "");
            }
        }
        else
        {
            if (isGameOverScreen)
                SetText(_score.ToString(), "Failed to load HighScores :(");
            else
                SetText("0", "");
        }
    }

    private IEnumerator RefreshHighscores()
    {
        while (true)
        {
            _myScores.DownloadScores();
            yield return new WaitForSeconds(30);
        }
    }
}