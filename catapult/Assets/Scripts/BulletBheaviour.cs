using UnityEngine;
using UnityEngine.InputSystem;

public class BulletBheaviour : MonoBehaviour
{
    private float _fullSpeed;
    private bool _isSlowing;
    private readonly float _slowSpeed = 0.1f;

    private void Start()
    {
        _fullSpeed = Time.timeScale;
    }

    private void Update()
    {
        if (_isSlowing)
            Time.timeScale = _slowSpeed;
        else
            Time.timeScale = _fullSpeed;
    }

    public void SlowInput(InputAction.CallbackContext context)
    {
        _isSlowing = context.ReadValueAsButton();
    }
}