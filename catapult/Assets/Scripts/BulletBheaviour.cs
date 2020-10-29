using UnityEngine;
using UnityEngine.InputSystem;

public class BulletBheaviour : MonoBehaviour
{
    private float _fullSpeed;
    private bool _isSlowing;
    private readonly float _slowSpeed = 0.1f;
    private float _slowRate = 2f;
    private float _fastRate = 5f;

    private void Start()
    {
        _fullSpeed = Time.timeScale;
    }

    private void Update()
    {
        if (_isSlowing && Time.timeScale > _slowSpeed)
        {
            Time.timeScale -= _slowRate * Time.deltaTime;
        } else if (!_isSlowing && Time.timeScale < _fullSpeed)
        {
            Time.timeScale += _fastRate * Time.deltaTime;
        }
    }

    public void SlowInput(InputAction.CallbackContext context)
    {
        _isSlowing = context.ReadValueAsButton();
    }
}