using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] [Min(0.1f)] private float maxSpeed = 1f;
    [SerializeField] [Min(0.1f)] private float acceleration = 1f;
    [SerializeField] private Transform platform;
    [SerializeField] private Transform inputSpace;

    private Vector2 _input;
    private Rigidbody _rigidbody;
    private Vector3 _lastTargetVelocity = new Vector3(-1, 0, -1);

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _input.x = Input.GetAxis("Horizontal");
        _input.y = Input.GetAxis("Vertical");
        if (_input.magnitude > 1) _input.Normalize();
    }

    private void FixedUpdate()
    {
        var velocity = _rigidbody.velocity;

        var forward = inputSpace.forward;
        forward.y = 0;
        forward.Normalize();
        var right = inputSpace.right;
        right.y = 0;
        right.Normalize();
        var input = (_input.y * forward + _input.x * right) * maxSpeed;

        var normal = platform.up;
        var targetVelocity = Vector3.ProjectOnPlane(input, normal);
        targetVelocity.y = 0;
        
        var rotation = Quaternion.FromToRotation(Vector3.up, normal);
        if (targetVelocity.sqrMagnitude > 0.01f) _lastTargetVelocity = targetVelocity;
        rotation *= Quaternion.LookRotation(_lastTargetVelocity);
        _rigidbody.MoveRotation(rotation);

        targetVelocity.y = velocity.y; // maintain gravity
        _rigidbody.velocity = Vector3.MoveTowards(velocity, targetVelocity, acceleration * Time.deltaTime);
    }
}