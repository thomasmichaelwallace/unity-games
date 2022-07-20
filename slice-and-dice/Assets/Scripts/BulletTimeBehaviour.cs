using UnityEngine;
using UnityEngine.UI;

public class BulletTimeBehaviour : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float powerLoseSeconds;
    private bool _isSlow;
    private float _targetFixedUpdateTime;
    private float _slowTimeScale = 0.1f;
    private float _powerRate;
    private GameManager _gameManager;
    private readonly Color32 _usingColor = new(243, 120, 120, 130);
    private readonly Color32 _holdingColor = new(249, 249, 197, 130);

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _targetFixedUpdateTime = Time.fixedDeltaTime;
        _powerRate = (1/0.9f) / powerLoseSeconds;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && _gameManager.Power > 0)
        {
            _isSlow = true;
            Time.timeScale = _slowTimeScale;
            Time.fixedDeltaTime = _targetFixedUpdateTime * _slowTimeScale;
        }

        var forceOut = false;
        if (_isSlow)
        {
            _gameManager.Power -= (_powerRate * Time.unscaledDeltaTime);

            if (_gameManager.Power < 0) forceOut = true;
        }

        if (Input.GetButtonUp("Jump") || forceOut)
        {
            _isSlow = false;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = _targetFixedUpdateTime;
        }

        var s = image.transform.localScale;
        s.x = Mathf.Max(_gameManager.Power, 0f);
        image.transform.localScale = s;
        image.color = _isSlow ? _usingColor : _holdingColor;
    }
}