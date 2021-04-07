using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatInput : MonoBehaviour
{
    [SerializeField] private float power = 1f;
    [SerializeField] private float turnPower = 1f;
    
    private Rigidbody _rb;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        var forward = Input.GetAxis("Vertical") * power * Time.deltaTime;
        var turn = Input.GetAxis("Horizontal") * turnPower * Time.deltaTime;
        _rb.AddForce(transform.forward * forward);
        _rb.AddTorque(0, turn, 0);
    }
}
