using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScreenBehaviour : MonoBehaviour
{
    public TextMeshProUGUI score;
    
    private TextMeshProUGUI _text;
    private readonly float _fadeRate = 3f;
    
    private CanvasGroup _canvas;
    
    private int _count = 3;
    private float _timer = 0;

    private bool _isStart = true;
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
        if (_isEnd) return;
        _text.text = "GAME OVER!" + "\n" + score.text + "\n" + "<size=25%>Press {space} to retry</size>";
        _isEnd = true;
    }
    
    public void RestartInput(InputAction.CallbackContext context)
    {
        if (!_isEnd) return;
        if (!context.performed) return;
        SceneManager.LoadScene(0);
    }
}
