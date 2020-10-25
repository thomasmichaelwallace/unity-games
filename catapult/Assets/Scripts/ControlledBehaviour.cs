using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlledBehaviour : MonoBehaviour
{
    private int _target = 0;
    
    private float _to = 0;
    private float _velocity = 0;
    private float _acceleration = 5f;
    
    void Update()
    {
        Debug.Log(_velocity);
        
        var x = transform.position.x;
        if (Mathf.Approximately(_to, x))
        {
            _velocity = 0;
            return;
        }

        float delta = Mathf.Abs(x - _to);
        float snap = _acceleration * Time.deltaTime;
        if (delta < snap && Mathf.Abs(_velocity) < _acceleration)
        {
            // do snap
            transform.position = new Vector3(_to, 0, 0f);
            return;
        }

        int sign = 0;
        float stopping =Mathf.Pow(_velocity, 2f) / (2 * _acceleration);        
        if (delta > stopping)
        {
            sign = 1; // accelerate
        }
        else
        {
            sign = -1;
        }
        
        if (x > _to)
        {
            sign *= -1; // heading right
        }
        _velocity += sign * snap;
        x += _velocity * Time.deltaTime;
        
        transform.position = new Vector3(x, 0, 0);
    }

    private void Move(int direction)
    {
        _target += direction;
        if (_target < -1) _target = -1;
        if (_target > 1) _target = 1;
        _to = _target * 2f;
        
        Debug.Log(_to);
    }
    
    public void Left(InputAction.CallbackContext context)
    {
        if (context.performed) Move(-1);
    }

    public void Right(InputAction.CallbackContext context)
    {
        if(context.performed) Move(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("dead");
    }
}
