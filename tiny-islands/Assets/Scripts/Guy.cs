using UnityEngine;

public class Guy : MonoBehaviour
{
    private const float SinkPoint = -11;
    private const float BoatRadius = 0.5f;
    private const float DrownTime = 7f;
    private const float JumpHeight = 5f;
    private const float JumpSpeed = 4f;
    private const float WaterJumpHeight = 1f;
    private const float WaterJumpLength = 1.5f;

    [SerializeField] private float jumpLength;
    private Transform _boat;
    private Quaternion _deadAngle;
    private float _drownSpeed;
    private float _jumpVelocity;
    private Level _level;
    private float _sqrBoatLength;
    private float _sqrJumpLength;
    private float _sqrWaterJumpLength;
    private State _state = State.Sleeping;

    private void Awake()
    {
        _sqrJumpLength = Mathf.Pow(jumpLength, 2);
        _sqrWaterJumpLength = Mathf.Pow(WaterJumpLength, 2);
        _sqrBoatLength = Mathf.Pow(BoatRadius, 2);

        var deadZ = Random.Range(0, 360);
        _deadAngle = Quaternion.Euler(90f, 0, deadZ);

        _drownSpeed = 90f / DrownTime;

        _boat = FindObjectOfType<BoatInput>().transform;
        _level = FindObjectOfType<Level>();
    }


    private void Update()
    {
        if (_state == State.Sleeping || _state == State.Saved || _state == State.Dead) return;

        var t = transform;
        var position = t.position;

        // can be saved
        if (_state == State.Waiting || _state == State.Drowning)
        {
            var distance = (position - _boat.position).sqrMagnitude;
            if (_state == State.Waiting && distance < _sqrJumpLength)
            {
                _jumpVelocity = JumpHeight;
                _state = State.Jumping;
            }
            else if (distance < _sqrWaterJumpLength)
            {
                _jumpVelocity = WaterJumpHeight;
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
            var target = _boat.position;
            target.y = 0;

            current = Vector3.MoveTowards(current, target, JumpSpeed * Time.deltaTime);
            _jumpVelocity -= 9.81f * Time.deltaTime;
            current.y = position.y + _jumpVelocity * Time.deltaTime;

            if (current.y <= 0f)
            {
                _state = State.Drowning;
                current.y = 0;
            }

            var distance = (current - _boat.position).sqrMagnitude;
            if (distance < _sqrBoatLength)
            {
                _state = State.Saved;
                _level.Score();
                Destroy(gameObject);
            }

            t.position = current;
        }
    }

    public void StartSinking()
    {
        if (_state == State.Sleeping) _state = State.Waiting;
    }

    private enum State
    {
        Sleeping,
        Waiting,
        Jumping,
        Saved,
        Drowning,
        Sinking,
        Dead
    }
}