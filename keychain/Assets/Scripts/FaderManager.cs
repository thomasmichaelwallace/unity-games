using UnityEngine;
using UnityEngine.SceneManagement;

public class FaderManager : MonoBehaviour
{
    private CanvasGroup _canvas;
    private FadeState _state;
    private int _target;

    private void Start()
    {
        _canvas = GetComponent<CanvasGroup>();
        // first state
        _canvas.alpha = 1;
        _state = FadeState.FadeToTrans;
        // restore time (just in case)
        Time.timeScale = 1;
    }

    private void Update()
    {
        switch (_state)
        {
            case FadeState.White:
            case FadeState.Trans:
                return; // static
            case FadeState.FadeToTrans:
            {
                _canvas.alpha -= Time.unscaledDeltaTime * 2f;
                if (_canvas.alpha <= 0) _state = FadeState.Trans;
                break;
            }
            case FadeState.FadeToWhite:
            {
                _canvas.alpha += Time.unscaledDeltaTime * 2f;
                if (_canvas.alpha >= 1)
                {
                    _state = FadeState.White;
                    SceneManager.LoadScene(_target);
                }

                break;
            }
        }
    }

    public void GotoScene(int sceneIndex)
    {
        if (_state != FadeState.Trans) return; // fading
        _target = sceneIndex;
        _state = FadeState.FadeToWhite;
    }

    private enum FadeState
    {
        White,
        FadeToTrans,
        Trans,
        FadeToWhite
    }
}