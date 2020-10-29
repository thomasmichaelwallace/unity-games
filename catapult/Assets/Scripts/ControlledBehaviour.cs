using UnityEngine;
using UnityEngine.InputSystem;

public class ControlledBehaviour : MonoBehaviour
{
    private readonly float _acceleration = 10f;
    private readonly float _bulletFactor = 2.5f;
    private readonly float _minRotation = 5f;
    
    private int _target;
    private float _to;
    private float _velocity;

    private void Update()
    {
        float f = Time.timeScale < 1 ? _bulletFactor : 1;
        float dt = Time.deltaTime * f;
        
        var rotation = transform.rotation.eulerAngles;
        rotation.x += (_velocity * Time.deltaTime) * 10f + _minRotation * Time.deltaTime;
        rotation.y -= (_velocity * Time.deltaTime) * 10f - _minRotation * Time.deltaTime;;
        rotation.z += _minRotation * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rotation);

        var x = transform.position.x;
        if (Mathf.Approximately(_to, x))
        {
            _velocity = 0;
            return;
        }

        var delta = Mathf.Abs(x - _to);
        var snap = _acceleration * dt;
        if (delta < snap) // && Mathf.Abs(_velocity) < _acceleration)
        {
            transform.position = new Vector3(_to, 0, 0f);
            return;
        }

        var sign = 0;
        var stopping = Mathf.Pow(_velocity, 2f) / (2 * _acceleration);
        if (delta > stopping)
            sign = 1; // accelerate
        else
            sign = -1;

        if (x > _to) sign *= -1; // heading right
        _velocity += sign * snap;
        x += _velocity * dt;

        if (x > 2.2 && _velocity > 0) _velocity = -_velocity;
        if (x < -2.2 && _velocity < 0) _velocity = -_velocity;

        transform.position = new Vector3(x, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("dead");
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
        if (context.performed) Move(1);
    }
}