using UnityEngine;

public class BikeBehaviour : MonoBehaviour
{
    public GameObject fire;
    
    private const float MaxSpeed = 20f;
    private const float Acceleration = 2 * MaxSpeed;
    private const float SpeedUpMaxSpeed = 2 * MaxSpeed;
    private const float SpeedUpAcceleration = 2 * Acceleration;

    private float _maxSpeed = MaxSpeed;
    private float _acceleration = Acceleration;
    private float _inputFactor = 1;
    
    private bool _shootFire;
    private bool _doBounce;
    private float _fireTimer = 1f;
    private Transform _camera;
    private AudioSource _sound;

    private Rigidbody2D _rigidbody;

    public void SpeedUpEffect(bool on)
    {
        if (on)
        {
            _maxSpeed = SpeedUpMaxSpeed;
            _acceleration = SpeedUpAcceleration;
        }
        else
        {
            _maxSpeed = MaxSpeed;
            _acceleration = Acceleration;
        }
    }

    public void ReverseInputEffect(bool on)
    {
        if (on)
        {
            _inputFactor = -1;
        }
        else
        {
            _inputFactor = 1;
        }
    }

    public void FireEffect(bool on)
    {
        if (on)
        {
            _fireTimer = 0;
            _shootFire = true;
        }
        else
        {
            _shootFire = false;
        }
    }

    public void Bounce()
    {
        _doBounce = true;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _camera = FindFirstObjectByType<CameraSpinBehaviour>().transform;
        _sound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_doBounce)
        {
            _rigidbody.velocity *= -1.5f;
            if (_rigidbody.velocity.SqrMagnitude() > Mathf.Pow(SpeedUpMaxSpeed * 2f, 2f))
            {
                _rigidbody.velocity *= ((SpeedUpMaxSpeed * 2f) / Mathf.Abs(_rigidbody.velocity.magnitude));
            }
            _doBounce = false;
        }
        else
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var target = new Vector2(horizontal, vertical) * (_maxSpeed * _inputFactor);
            
            if (!LevelManager.Manager.IsPlayable) target = Vector2.zero;
            
            var rotated = _camera.rotation * target;
            rotated.z = 0;

            var delta = Time.deltaTime * _acceleration;
            var velocity = Vector2.MoveTowards(_rigidbody.velocity, rotated, delta);
            _rigidbody.velocity = velocity;
            
            if (velocity.sqrMagnitude > 0.1f)
            {
                var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                _rigidbody.rotation = angle;
            }
        }

        if (_rigidbody.velocity.sqrMagnitude > 0.1f)
        {
            if (!_sound.isPlaying) _sound.Play();
            _sound.pitch = (_rigidbody.velocity.sqrMagnitude / Mathf.Pow(MaxSpeed, 2f));
        }
        else
        {
            if (_sound.isPlaying) _sound.Stop();
        }

        if (_shootFire)
        {
            _fireTimer -= Time.deltaTime;
            if (_fireTimer <= 0)
            {
                _fireTimer = 1f;
                Instantiate(fire);
            }
        }
    }
}