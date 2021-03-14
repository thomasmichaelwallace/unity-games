using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI gameOver;
    [SerializeField] private Transform player;
    private bool _gameOver;

    private float _time;

    private void Update()
    {
        if (_gameOver)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) SceneManager.LoadScene(0);
            return;
        }

        _time += Time.unscaledDeltaTime;
        score.text = ((int) _time).ToString();

        if (!(player.position.y < -30)) return;

        _gameOver = true;
        gameOver.gameObject.SetActive(true);
        gameOver.text = gameOver.text.Replace("$score", ((int) _time).ToString());
        player.gameObject.SetActive(false);
    }
}