using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class ScreenBehaviour : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private readonly float _fadeRate = 3f;
    
    private CanvasGroup _canvas;
    
    private int _count = 3;
    private float _timer = 0;

    private bool _isStart = false;
    private bool _isEnd = false;
    
    void Start()
    {
        _canvas = GetComponent<CanvasGroup>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    void Update()
    {
        if (_isStart)
        {
            if (_count > 0)
            {
                _timer += Time.deltaTime;
                if (_timer >= 1)
                {
                    _count -= 1;
                    string line = _count == 0 ? "FIRE!" : _count.ToString();
                    _text.text = line;
                    _timer = 0;
                }
            }
            else if (_canvas.alpha > 0)
            {
                _canvas.alpha -= Time.deltaTime * _fadeRate;
            }
            else
            {
                _isStart = false;
            }
        } else if (_isEnd)
        {
            if (_canvas.alpha < 1) _canvas.alpha += Time.deltaTime * _fadeRate;
        }
    }

    public void GameOver()
    {
        _text.text = "GAME OVER!";
        _isEnd = true;
    }
}
