using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody fieldBall;
    public GameObject playerBall;
    private readonly float _angle = 0.5f; // raise at 2:1
    private readonly float _effortRate = 2f;
    private readonly float _maxEffort = 4f;

    private readonly float _speed = 2.5f;
    private readonly float _strength = 10f;
    private readonly Vector3 _ballOffset = new Vector3(0f,0.15f, -0.5f);

    private float _effort;
    private bool _hasBall = false;

    private void OnCollisionEnter(Collision other)
    {
        if (other.rigidbody == fieldBall)
        {
            _hasBall = true;
            fieldBall.isKinematic = true;
            fieldBall.constraints = RigidbodyConstraints.None;
        }
    }

    private void FixedUpdate()
    {
        if (_hasBall)
        {
            fieldBall.position = transform.position + _ballOffset;
        }
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
        
        // hit
        if (_hasBall)
        {
            if (Input.GetButton("Fire1"))
            {
                _effort += _effortRate * Time.deltaTime;
                _effort = Mathf.Min(_effort, _maxEffort);
            }
            else if (_effort > 0)
            {
                fieldBall.isKinematic = false;
                fieldBall.constraints = RigidbodyConstraints.FreezePositionX;
                _hasBall = false;
                
                var impact = new Vector3(0, _angle * _strength * _effort, -_strength * _effort);
                fieldBall.AddForce(impact); // , ForceMode.VelocityChange);

                _effort = 0;
            }
            else
            {
                _effort = 0;
            }
        }
    }
}