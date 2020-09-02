using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeBehaviour : MonoBehaviour
{
    private readonly float minForce = 250f;
    private readonly float maxForce = 1500f;
    private readonly float maxHeight = 2f;
    
    void Start()
    {
        float force = UnityEngine.Random.Range(minForce, maxForce);
        float impactHeight = UnityEngine.Random.Range(0, maxHeight);

        Vector3 centre = transform.position;        
        centre.y += impactHeight;
        centre.z += impactHeight / 2;
        
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var child in rigidbodies)
        {
            child.AddExplosionForce(force, centre, impactHeight, impactHeight / 2);
        }
    }
}
