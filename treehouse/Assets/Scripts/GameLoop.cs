using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Transform player;

    private int _state = 1; // 1 playing, 2 game over
    private float _time;

    private void Update()
    {
        if (_state == 1)
        {
            _time += Time.deltaTime;
            text.text = $"{(int) _time}s";

            // game over
            if (player.position.y < -8)
            {
                title.text = "<color=#F2A154>~ Game Over ~</color>\nYou Survived " + text.text + "\nPress [Space] To Restart\n\n";
                _state = 0;
            }
        }

        if (_state == 0)
        {
            if (Input.GetButton("Jump")) SceneManager.LoadScene(0);
        }
    }
}