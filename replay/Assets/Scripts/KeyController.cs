using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    public Transform Gate;
    public float Speed = 1f;

    private bool _isCollected = false;
    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(30, 0, 0) * Time.deltaTime);

        if (_isCollected && Gate.localScale.y > 0)
        {
            Gate.localScale -= Vector3.up * Speed * Time.deltaTime;
            if (Gate.localScale.y <= 0)
            {
                Gate.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _renderer.enabled = false;
            _isCollected = true;
        }
    }
}
