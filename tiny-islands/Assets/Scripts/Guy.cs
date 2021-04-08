using UnityEngine;

public class Guy : MonoBehaviour
{
    private const float SinkPoint = -11;

    [SerializeField] private Transform boat;
    [SerializeField] private float jumpLength;
    [SerializeField] private float drownTime = 2f;

    private Quaternion _deadAngle;
    private bool _isDrowning;
    private bool _isDying;
    private bool _isSaved;
    private float _sqrJumpLength;
    private float _drownSpeed;

    private void Awake()
    {
        var deadZ = Random.Range(0, 360);
        _deadAngle = Quaternion.Euler(90f, 0, deadZ);
        _sqrJumpLength = Mathf.Pow(jumpLength, 2);
        _drownSpeed = 90f / drownTime;
    }

    private void Update()
    {
        if (_isSaved) return;

        // waiting to be saved
        if (!_isDying)
        {
            var distance = (transform.position - boat.position).sqrMagnitude;
            if (distance < _sqrJumpLength)
            {
                _isSaved = true;
                Destroy(gameObject);
                return;
            }
        }

        // drowning
        var t = transform;
        if (_isDrowning)
        {
            var rotation = t.rotation;
            rotation = Quaternion.RotateTowards(rotation, _deadAngle, _drownSpeed * Time.deltaTime);
            t.rotation = rotation;
            if (Mathf.Approximately(rotation.eulerAngles.x, 90f))
            {
                _isDying = true;
                _isDrowning = false;
            }

            return;
        }

        // sinking
        var position = t.position;
        position.y -= Water.RisingSpeed * Time.deltaTime;
        t.position = position;
        if (position.y <= 0f && !_isDying)
            _isDrowning = true;
        else if (position.y <= SinkPoint && _isDying) Destroy(gameObject);
    }
}