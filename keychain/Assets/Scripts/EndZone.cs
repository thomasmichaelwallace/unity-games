using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndZone : MonoBehaviour
{
    private bool _fired;
    private bool _inside;
    private float _timer;

    private Quaternion _initialRotation;
    private Quaternion _feedbackRotation;
    private Quaternion _firedRotation;

    private FaderManager _fader;

    private void Start()
    {
        _initialRotation = transform.localRotation;
        var euler = _initialRotation.eulerAngles;
        euler.z -= 10f;
        _feedbackRotation = Quaternion.Euler(euler);
        euler.z += 30f;
        _firedRotation = Quaternion.Euler(euler);

        _fader = FindObjectOfType<FaderManager>();
    }

    private void Update()
    {
        if (_fired) return;
        if (!_inside) return;
        
        transform.localRotation = _firedRotation;
        
        var nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        // SceneManager.LoadScene(nextLevel);
        _fader.GotoScene(nextLevel);
        _fired = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _timer = 0;
        Time.timeScale = 1;
        transform.localRotation = _initialRotation;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_fired) return;
        const float holdTime = 0.5f;

        transform.localRotation = _feedbackRotation;

        _timer += Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Max(0.01f, holdTime - _timer);

        if (_timer > holdTime) _inside = true;
    }
}