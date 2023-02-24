using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpinBehaviour : MonoBehaviour
{
    private float _rotation;
    private bool _isRotating;
    
    private void Update()
    {
        if (_isRotating)
        {
            _rotation += Time.deltaTime * (360f / 10f); // rotations a second
        }
        else
        {
            _rotation = 0;
        }
        
        transform.rotation = Quaternion.Euler(0, 0, _rotation);
    }

    public void SetRotating(bool on)
    {
        _isRotating = on;
    }
}
