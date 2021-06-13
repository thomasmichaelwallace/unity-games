using UnityEngine;
using UnityEngine.SceneManagement;

public class FaderManager : MonoBehaviour
{
    private CanvasGroup _canvas;
    private bool _fadedIn;
    private bool _fadingOut;
    private int _target;
    private bool _waiting;

    private void Start()
    {
        _canvas = GetComponent<CanvasGroup>();
        Time.timeScale = 1; // restore time
    }

    private void Update()
    {
        if (_waiting) return;

        if (_fadingOut)
        {
            _canvas.alpha += Time.unscaledDeltaTime * 2f;
            if (_canvas.alpha >= 1)
            {
                _waiting = true;
                SceneManager.LoadScene(_target);
            }
        }

        if (!_fadedIn)
        {
            _canvas.alpha -= Time.unscaledDeltaTime * 2f;
            if (_canvas.alpha <= 0) _fadedIn = true;
        }
    }

    public void GotoScene(int sceneIndex)
    {
        if (_waiting) return;
        _target = sceneIndex;
        _fadingOut = true;
    }
}