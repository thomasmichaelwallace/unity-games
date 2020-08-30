using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody ball;
    public Transform deathField;

    public float angle = 0.8f; // raise at 2:1
    public float strength = 250f;    
    
    private readonly Vector3 _ballOffset = new Vector3(0f, 0.15f, -0.5f);
    private readonly float _effortRate = 8f;
    private readonly float _maxEffort = 4f;

    private readonly float _speed = 2.5f;
    
    private float _effort;
    private bool _hasBall;

    private void Start()
    {
        ball.sleepThreshold = 0f;
    }

    private void Update()
    {
        // movement
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");

        var position = transform.position;
        position.x += horizontal * _speed * Time.deltaTime;
        position.z += vertical * _speed * Time.deltaTime;
        transform.position = position;

        if (_hasBall)
        {
            ball.MovePosition(transform.position + _ballOffset);
            
            // shooting
            if (Input.GetButton("Fire1"))
            {
                _effort += _effortRate * Time.deltaTime;
                _effort = Mathf.Min(_effort, _maxEffort);
            }
            else if (_effort > 0)
            {
                ball.isKinematic = false;
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
                deathField.gameObject.SetActive(true);
                LayerMask layerMask = LayerMask.GetMask("Opponents");
                var colliders = Physics.OverlapBox(deathField.transform.position, deathField.transform.localScale / 2,
                    Quaternion.identity, layerMask);
                var i = 0;
                while (i < colliders.Length)
                {
                    colliders[i].gameObject.SetActive(false);
                    i += 1;
                }
            }
            else if (deathField.gameObject.activeSelf)
            {
                deathField.gameObject.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.rigidbody == ball)
        {
            _hasBall = true;
            ball.isKinematic = true;
            ball.constraints = RigidbodyConstraints.None;
        }
    }
}