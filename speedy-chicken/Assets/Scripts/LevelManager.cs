using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public RectTransform arrow;
    public EffectBarBehaviour effects;
    
    public float arrowOffset;
    private BikeBehaviour _bike;
    private Vector3 _deliveryPosition = Vector3.zero;

    public GameObject titleCard;
    public GameObject titleChicken;
    public GameObject gameOverCard;
    public AudioSource bgm;
    public AudioSource doorBell;

    private int _score;
    private float _timer = 105; // seconds
    private bool _hideGps;

    private enum States
    {
        Title,
        Play,
        GameOver,
    }

    private States _state = States.Title;

    private SpawnerBehaviour _spawner;
    public static LevelManager Manager { get; private set; }

    public bool tinyDelivery;
    public bool bouncy;
    public int multiplier = 1;
    
    public bool IsPlayable { get; private set; }

    private Transform _camera;
    
    private void Awake()
    {
        if (Manager != null && Manager != this)
        {
            Destroy(this);
            return;
        }

        Manager = this;

        _spawner = FindFirstObjectByType<SpawnerBehaviour>();
        _bike = FindFirstObjectByType<BikeBehaviour>();
        _camera = FindFirstObjectByType<CameraSpinBehaviour>().transform;
    }

    private void Start()
    {
        IsPlayable = false;
        gameOverCard.SetActive(false);
        titleChicken.SetActive(true);
        titleCard.SetActive(true);
    }

    private void Update()
    {
        UpdateTimer();
        scoreText.text = $"Score: {_score} (x{multiplier})";

        if (_state == States.Title)
        {
            // do the title!
            if (Input.GetButtonDown("Jump"))
            {
                // press to start!
                titleCard.SetActive(false);
                _state = States.Play;
                IsPlayable = true;
                NextDelivery();
                Destroy(titleChicken);
                bgm.Play();
            }

            return;
        }

        if (_state == States.GameOver)
        {
            if (Input.GetButtonDown("Jump"))
            {
                // restart!
                SceneManager.LoadScene("Main");
            }

            return;
        }
        
        
        // timer
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            gameOverCard.SetActive(true);
            IsPlayable = false;
            _timer = 0;
            _state = States.GameOver;
            var gameOverText = gameOverCard.GetComponentInChildren<TextMeshProUGUI>();
            gameOverText.text = gameOverText.text.Replace("{{score}}", _score.ToString());
        }

        // arrow
        var delta = _deliveryPosition - _bike.transform.position;
        delta.z = 0;
        var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

        if (_hideGps)
        {
            angle += 180f; // exactly the wrong direction.
        }
        
        var cameraOffset = _camera.rotation.eulerAngles.z;
        arrow.rotation = Quaternion.Euler(0, 0, angle + arrowOffset - cameraOffset);
        
    }

    private void NextDelivery()
    {
        _deliveryPosition = _spawner.Spawn();
    }

    private void UpdateTimer()
    {
        timerText.text = _timer.ToString("F0") + "s Remaining!";
    }

    public void Score()
    {
        _score += multiplier;
        NextDelivery();
        effects.StackEffect();
        doorBell.Play();
    }

    public void SetGps(bool on)
    {
        _hideGps = !on;
    }
}