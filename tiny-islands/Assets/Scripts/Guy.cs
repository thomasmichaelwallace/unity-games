using UnityEngine;

public class Guy : MonoBehaviour
{
    private const float SinkPoint = -11;

    [SerializeField] private Transform boat;
    [SerializeField] private float jumpLength;
    [SerializeField] private float drownTime = 2f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpSpeed = 1f;
    [SerializeField] private float boatRadius = 1f;
    [SerializeField] private float waterJumpHeight = 0.5f;
    [SerializeField] private float waterJumpLength = 1f;

    private Quaternion _deadAngle;
    private float _drownSpeed;
    private float _jumpVelocity;
    private float _sqrBoatLength;
    private float _sqrJumpLength;
    private float _sqrWaterJumpLength;
    private State _state = State.Waiting;

    private void Awake()
    {
        _sqrJumpLength = Mathf.Pow(jumpLength, 2);
        _sqrWaterJumpLength = Mathf.Pow(waterJumpLength, 2);
        _sqrBoatLength = Mathf.Pow(boatRadius, 2);

        var deadZ = Random.Range(0, 360);
        _deadAngle = Quaternion.Euler(90f, 0, deadZ);

        _drownSpeed = 90f / drownTime;
    }


    private void Update()
    {
        if (_state == State.Saved || _state == State.Dead) return;

        var t = transform;
        var position = t.position;

        // can be saved
        if (_state == State.Waiting || _state == State.Drowning)
        {
            var distance = (position - boat.position).sqrMagnitude;
            if (_state == State.Waiting && distance < _sqrJumpLength)
            {
                _jumpVelocity = jumpHeight;
                _state = State.Jumping;                
            } else if (distance < _sqrWaterJumpLength)
            {
                _jumpVelocity = waterJumpHeight;
                _state = State.Jumping;               
            }
        }

        // sinking (either on land, or after drowning)
        if (_state == State.Waiting || _state == State.Sinking)
        {
            position.y -= Water.RisingSpeed * Time.deltaTime;
            t.position = position;

            if (position.y <= 0f && _state == State.Waiting)
            {
                _state = State.Drowning;
            }
            else if (position.y <= SinkPoint && _state == State.Sinking)
            {
                _state = State.Dead;
                Destroy(gameObject);
            }
        }

        // drowning
        if (_state == State.Drowning)
        {
            var rotation = t.rotation;
            rotation = Quaternion.RotateTowards(rotation, _deadAngle, _drownSpeed * Time.deltaTime);
            t.rotation = rotation;

            if (Mathf.Abs(Quaternion.Angle(rotation, _deadAngle)) <= 1f) _state = State.Sinking;
        }

        // jumping
        if (_state == State.Jumping)
        {
            var current = new Vector3(position.x, 0, position.z);
            var target = boat.position;
            target.y = 0;

            current = Vector3.MoveTowards(current, target, jumpSpeed * Time.deltaTime);
            _jumpVelocity -= 9.81f * Time.deltaTime;
            current.y = position.y + _jumpVelocity * Time.deltaTime;

            if (current.y <= 0f)
            {
                _state = State.Drowning;
                current.y = 0;
            }

            var distance = (current - boat.position).sqrMagnitude;
            if (distance < _sqrBoatLength)
            {
                _state = State.Saved;
                Destroy(gameObject);
            }

            t.position = current;
        }
        
    }

    private enum State
    {
        Waiting,
        Jumping,
        Saved,
        Drowning,
        Sinking,
        Dead
    }
}