using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentController : MonoBehaviour
{
    public Transform player;
    public GameObject exploder;

    private readonly float effort = 5;

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
    
}