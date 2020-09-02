﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentController : MonoBehaviour
{
    public Transform player;
    public GameObject exploder;

    private readonly float effort = 5;
    private readonly float health = 2f;

    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        Vector3 direction = (player.position - transform.position).normalized;
    }

    public void GetHit()
    {
        Instantiate(exploder, transform.position, transform.rotation);
        _rigidbody.detectCollisions = false;
        gameObject.SetActive(false);
        Destroy(this);
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
                float angle = 0.5f;
                float strength = 40;
                float _effort = 1f;
                var impact = new Vector3(0, angle * strength * _effort, strength * _effort);
                other.rigidbody.AddForce(impact);
                Debug.Log($"Ouch! {speed}");   
            }
        }
    }
}