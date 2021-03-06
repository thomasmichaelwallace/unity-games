﻿using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody ball;

    // TODO: ball can't one shot
    public float angle;
    public float strength;
    public float maxSpeed;
    public float maxAcceleration;
    public Transform power;
    public Animator stick;
    
    private readonly Vector3 _ballOffset = new Vector3(0f, 0.15f, -0.5f);
    private readonly Vector3 _deathOffset = new Vector3(0f, 0.75f, -0.5f);
    private readonly Vector3 _deathScale = new Vector3(1f, 1f, 1.5f) / 2f;
    private readonly float _effortRate = 1f;
    private readonly float _maxEffort = 4f;
    
    private float _effort;
    private bool _hasBall;
    private Vector3 _inputs = Vector3.zero;
    private Rigidbody _rigidbody;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _rigidbody = GetComponent<Rigidbody>();
        stick = GetComponentInChildren<Animator>();
        ball.sleepThreshold = 0f;
    }

    private void Update()
    {
        // movement
        _inputs = Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")), 1f);
        
        if (_hasBall)
        {
            float powerHeight = (2 / _maxEffort) * _effort;
            Vector3 powerScale = power.localScale;
            powerScale.y = powerHeight;
            power.localScale = powerScale;
            Vector3 powerPos = power.position;
            powerPos.y = powerHeight / 2;
            power.position = powerPos;
            
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
                ball.AddForce(impact);
                
                stick.SetTrigger("strike");

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
                stick.SetTrigger("strike");

                LayerMask layerMask = LayerMask.GetMask("Opponents");
                var colliders = Physics.OverlapBox(transform.position + _deathOffset, _deathScale, Quaternion.identity, layerMask);
                var i = 0;
                while (i < colliders.Length)
                {
                    OpponentController opponent = colliders[i].GetComponent<OpponentController>();
                    if (opponent)
                    {
                        _gameManager.Kill();
                        opponent.GetHit();
                        Destroy(opponent.gameObject, 1f);
                    }
                    i += 1;
                }
            }
            else
            {
                stick.ResetTrigger("strike");    
            }
        }

        if (_hasBall != power.gameObject.activeSelf) power.gameObject.SetActive(_hasBall);
    }

    public void GetHit()
    {
        if (_hasBall)
        {
            _hasBall = false;
            ball.isKinematic = false;
            ball.detectCollisions = true;
            ball.constraints = RigidbodyConstraints.FreezePositionX;
        }
        Vector3 velocity = _rigidbody.velocity;
        velocity.x = -velocity.x;
        velocity.z = 100; // bounce!
        _rigidbody.velocity = velocity;
    }

    private void FixedUpdate()
    {
        var targetVelocity = _inputs * maxSpeed;
        float y = _rigidbody.velocity.y;
        Vector3 velocity = Vector3.MoveTowards(_rigidbody.velocity, targetVelocity, maxAcceleration);
        velocity.y = y;
        _rigidbody.velocity = velocity; 

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