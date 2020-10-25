using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletBheaviour : MonoBehaviour
{
    private bool _isSlowing = false;
    
    private float _fullSpeed;
    private float _slowSpeed = 0.1f;
    
    void Start()
    {
        _fullSpeed = Time.timeScale;
    }
    
    void Update()
    {
        if (_isSlowing)
        {
            Time.timeScale = _slowSpeed;
        }
        else
        {
            Time.timeScale = _fullSpeed;
        }
    }

    public void SlowInput(InputAction.CallbackContext context)
    {
        _isSlowing = context.ReadValueAsButton();
    }
}
