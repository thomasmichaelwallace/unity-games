using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBehaviour : MonoBehaviour
{
    private const float LevelUpSpeed = 1f;
    private const float FallSpeedRatio = 0.75f;
    private const float MinSpeed = 2f;
    [SerializeField] private ScoreBehaviour score;
    [SerializeField] private GameObject gameOverText;
    private AudioSource _audio;
    private float _fallSpeed = 6f;
    private bool _isGameOver;
    private bool _isStopping;
    private float _speed;

    private void Awake()
    {
        SetSpeed(MinSpeed);
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!_isStopping) return;
        if (_isGameOver)
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                var index = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(index);
            }

            return;
        }

        _speed -= Time.deltaTime * Mathf.Max(1, _speed);
        if (_speed <= 0)
        {
            _isGameOver = true;
            gameOverText.SetActive(true);
        }
    }

    public void SetDistance(int distance)
    {
        score.SetScore(distance);
    }

    public float GetSpeed()
    {
        return _speed;
    }

    private void SetSpeed(float speed)
    {
        _speed = speed;
        _fallSpeed = _speed * FallSpeedRatio;
    }

    public void SpeedUp()
    {
        SetSpeed(_speed + LevelUpSpeed);
        _audio.Play();
    }

    public bool FallIsDead()
    {
        _speed -= _fallSpeed * Time.deltaTime;
        if (!(_speed < MinSpeed)) return false;
        _isStopping = true;
        return true;
    }
}