using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToStart : MonoBehaviour
{
    private bool _fired;
    
    private FaderManager _fader;

    private void Start()
    {
        _fader = FindObjectOfType<FaderManager>();
    }

    private void Update()
    {
        if (_fired) return;
        if (Input.GetMouseButtonDown(0))
        {
            _fired = true;
            var first = SceneManager.GetActiveScene().buildIndex + 1;
            _fader.GotoScene(first);
        }
    }
}