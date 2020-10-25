using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlledBehaviour : MonoBehaviour
{
    private int _target = 0;
    private float _to = 0;
    
    void Update()
    {
        Vector3 p = transform.position;
        p.x = _to;
        transform.position = p;
    }

    private void Move(int direction)
    {
        _target += direction;
        if (_target < -1) _target = -1;
        if (_target > 1) _target = 1;
        _to = _target * 2f;
    }
    
    public void Left(InputAction.CallbackContext context)
    {
        if (context.performed) Move(-1);
    }

    public void Right(InputAction.CallbackContext context)
    {
        if(context.performed) Move(1);
    }
}
