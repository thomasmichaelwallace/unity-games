using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverBehaviourScript : MonoBehaviour
{
    private GameManager _gameManager;
    private bool _uploaded;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (!_uploaded)
        {
            _uploaded = true;
            if (_gameManager)
            {
                var score = _gameManager.Score;
                HighScores.UploadScore(Guid.NewGuid().ToString(), score);
            }
            else
            {
                Debug.LogWarning("could not upload score as gameover instant");
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (_gameManager) _gameManager.EndGame();
            SceneManager.LoadScene("Title");
        }
    }
}