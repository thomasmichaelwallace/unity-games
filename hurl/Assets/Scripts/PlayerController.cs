using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody ball;

    public float angle;
    public float strength;
    public float maxSpeed;
    public float maxAcceleration;

    public Animator _stick;


    private readonly Vector3 _ballOffset = new Vector3(0f, 0.15f, -0.5f);
    private readonly Vector3 _deathOffset = new Vector3(0f, 0.75f, -0.5f);
    private readonly Vector3 _deathScale = new Vector3(1f, 1f, 1.5f) / 2f;
    private readonly float _effortRate = 8f;
    private readonly float _maxEffort = 4f;

    private readonly float _speed = 2.5f;

    private float _effort;
    private bool _hasBall;
    private Vector3 _inputs = Vector3.zero;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _stick = GetComponentInChildren<Animator>();
        ball.sleepThreshold = 0f;
    }

    private void Update()
    {
        // movement
        _inputs = Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")), 1f);
        var acceleration = _inputs * (maxAcceleration * Time.deltaTime);
        _rigidbody.AddForce(acceleration, ForceMode.Acceleration);

        if (_hasBall)
        {
            // shooting
            if (Input.GetButton("Fire1"))
            {
                _effort += _effortRate * Time.deltaTime;
                _effort = Mathf.Min(_effort, _maxEffort);
            }
            else if (_effort > 0)
            {
                ball.isKinematic = false;
                ball.detectCollisions = true;
                ball.constraints = RigidbodyConstraints.FreezePositionX;
                _hasBall = false;

                var impact = new Vector3(0, angle * strength * _effort, -strength * _effort);
                ball.AddForce(impact); // , ForceMode.VelocityChange);

                _effort = 0;
            }
            else
            {
                _effort = 0;
            }
        }
        else
        {
            // is attacking
            if (Input.GetButton("Fire1"))
            {
                _stick.SetTrigger("strike");

                LayerMask layerMask = LayerMask.GetMask("Opponents");
                var colliders = Physics.OverlapBox(transform.position + _ballOffset, _deathScale, Quaternion.identity, layerMask);
                var i = 0;
                while (i < colliders.Length)
                {
                    OpponentController opponent = colliders[i].GetComponent<OpponentController>();
                    if (opponent) opponent.GetHit();
                    i += 1;
                }
            }
            else
            {
                _stick.ResetTrigger("strike");    
            }
        }
    }

    private void FixedUpdate()
    {
        var targetVelocity = _inputs * maxSpeed;
        _rigidbody.velocity = Vector3.MoveTowards(_rigidbody.velocity, targetVelocity, maxAcceleration);

        if (_hasBall) ball.MovePosition(transform.position + _ballOffset);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.rigidbody == ball)
        {
            _hasBall = true;
            ball.isKinematic = true;
            ball.detectCollisions = false;
            ball.constraints = RigidbodyConstraints.None;
        }
    }
}