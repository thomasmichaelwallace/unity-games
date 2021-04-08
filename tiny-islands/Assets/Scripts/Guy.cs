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

    private Quaternion _deadAngle;
    private float _drownSpeed;
    private bool _isDrowning;
    private bool _isDying;
    private bool _isJumping;
    private bool _isSaved;
    private float _sqrBoatLength;
    private float _sqrJumpLength;
    private float _yVelocity;

    private void Awake()
    {
        _sqrJumpLength = Mathf.Pow(jumpLength, 2);
        _sqrBoatLength = Mathf.Pow(boatRadius, 2);

        var deadZ = Random.Range(0, 360);
        _deadAngle = Quaternion.Euler(90f, 0, deadZ);

        _drownSpeed = 90f / drownTime;
    }

    private void Update()
    {
        if (_isSaved) return;

        var t = transform;
        var position = t.position;

        if (_isJumping)
        {
            var current = new Vector3(position.x, 0, position.z);
            var target = boat.position;
            target.y = 0;

            current = Vector3.MoveTowards(current, target, jumpSpeed * Time.deltaTime);
            _yVelocity -= 9.81f * Time.deltaTime;
            current.y = position.y + _yVelocity * Time.deltaTime;

            if (current.y <= 0f)
            {
                _isDrowning = true;
                _isJumping = false;
                _sqrJumpLength = 1f;
                current.y = 0;
            }

            var distance = (position - boat.position).sqrMagnitude;
            if (distance < _sqrBoatLength)
            {
                _isSaved = true;
                Destroy(gameObject);
            }

            t.position = current;
            return;
        }

        // waiting to be saved
        if (!_isDying)
        {
            var distance = (position - boat.position).sqrMagnitude;
            if (distance < _sqrJumpLength)
            {
                _yVelocity = jumpHeight;
                _isJumping = true;
                return;
            }
        }

        // drowning
        if (_isDrowning)
        {
            var rotation = t.rotation;
            rotation = Quaternion.RotateTowards(rotation, _deadAngle, _drownSpeed * Time.deltaTime);
            t.rotation = rotation;
            if (!Mathf.Approximately(rotation.eulerAngles.x, 90f)) return;

            _isDying = true;
            _isDrowning = false;
            return;
        }

        // sinking
        position.y -= Water.RisingSpeed * Time.deltaTime;
        t.position = position;
        if (position.y <= 0f && !_isDying)
        {
            _isDrowning = true;
            _sqrJumpLength = 1f;
        }
        else if (position.y <= SinkPoint && _isDying)
        {
            Destroy(gameObject);
        }
    }
}