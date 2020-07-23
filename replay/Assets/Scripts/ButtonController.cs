using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public Transform Water;
    public float Speed = 1f;

    private bool _isPressed = false;

    // Update is called once per frame
    void Update()
    {
        if (_isPressed && Water.localScale.y > 0)
        {
            Water.localScale -= Vector3.up * Speed * Time.deltaTime;
            if (Water.localScale.y <= 0) Water.gameObject.SetActive(false);
        }
        if (_isPressed && transform.localScale.y > 0)
        {
            transform.localScale -= Vector3.up * Speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPressed = true;
        }
    }
}
