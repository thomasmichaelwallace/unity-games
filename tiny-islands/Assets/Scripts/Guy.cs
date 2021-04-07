using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guy : MonoBehaviour
{
    [SerializeField] private Transform boat;
    [SerializeField] private float jumpLength;

    private bool _isSaved;
    private float _sqrJumpLength;

    private void Awake()
    {
        _sqrJumpLength = Mathf.Pow(jumpLength, 2);
    }

    void Update()
    {
        if (_isSaved)
        {
            return;
        }
        
        var distance = (transform.position - boat.position).sqrMagnitude;
        if (distance < _sqrJumpLength)
        {
            _isSaved = true;
            Destroy(gameObject);
        }
    }
}
