using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentController : MonoBehaviour
{
    public Transform target;
    public GameObject exploder;
    public Animator stick;

    private readonly float health = 2f;
    private readonly float activeDistance = 30f;
    private readonly float maxSpeed = 3f;
    private readonly float maxAcceleration = 2000f;
    private readonly float aheadness = 0.5f;
    
    private readonly Vector3 _deathOffset = new Vector3(0f, 0.75f, 0.5f);
    private readonly Vector3 _deathScale = new Vector3(1f, 1f, 1.5f) / 2.5f;

    private Rigidbody _rigidbody;
    
    void Start()
    {
        stick = GetComponentInChildren<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    private void FixedUpdate()
    {
        Vector3 direction = target.position - transform.position;
        if (direction.y > 0) direction.y = 0; // do not fly.
        if (direction.magnitude > activeDistance) return;
        
        
        if (direction.z < 0)
        {
            // behind
            float offsetX = 2 * Mathf.Sign(direction.x);
            Vector3 offset = new Vector3(-offsetX, 0, -0.5f);
            Vector3 point = (target.position + offset);
            direction = point - transform.position;
        } else if (Mathf.Abs(direction.x) > 2f)
        {
            // side
            Vector3 offset = new Vector3(0, 0, -2.5f);
            Vector3 point = (target.position + offset);
            direction = point - transform.position;
        }

        if (direction.z > 0 && direction.magnitude < _deathScale.magnitude)
        {
            stick.SetTrigger("strike");
            LayerMask layerMask = LayerMask.GetMask("Player");
            var colliders = Physics.OverlapBox(transform.position + _deathOffset, _deathScale, Quaternion.identity, layerMask);
            int i = 0;
            while (i < colliders.Length)
            {
                Collider other = colliders[i];
                PlayerController player = other.GetComponent<PlayerController>();
                if (player)
                {
                    player.GetHit();
                }
                else
                {
                    if (other.CompareTag("Ball"))
                    {
                        HitBall(other.attachedRigidbody);
                    }
                }
                
                i += 1;
            }
        }
        else
        {
            float y = direction.y;
            direction = Vector3.ClampMagnitude(direction, 1f);
            direction.y = y; // allow falling
            var targetVelocity = direction * maxSpeed;
            _rigidbody.velocity = Vector3.MoveTowards(_rigidbody.velocity, targetVelocity, maxAcceleration);   
        }
    }

    private void HitBall(Rigidbody ball)
    {
        float angle = 0.5f;
        float strength = 30;
        float _effort = 1f;
        var impact = new Vector3(0, angle * strength * _effort, strength * _effort);
        ball.AddForce(impact);
    }

    public void GetHit()
    {
        Instantiate(exploder, transform.position, transform.rotation, transform.parent);
        _rigidbody.detectCollisions = false;
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            float speed = other.rigidbody.velocity.magnitude;
            if (speed > health)
            {
                GetHit();    
            }
            else
            {
                HitBall(other.rigidbody);
            }
        }
    }
}