using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public Transform MyWater; // starts up
    public SwitchController TheirSwitch;
    public bool StartPressed = true;

    public float Speed = 1f;

    private Collider _myCollider;
    private bool _iAmPressed = false;

    public void DoSwitch()
    {
        _iAmPressed = !_iAmPressed;
        _myCollider.enabled = !_iAmPressed;
        if (!_iAmPressed) MyWater.gameObject.SetActive(true);
        if (_iAmPressed)
        {
            transform.position -= Vector3.up * 0.09f;
            // transform.localScale.Set(0.6f, 0.01f, 0.6f); // pressed.
        }
        else
        {
            transform.position += Vector3.up * 0.09f;
            // transform.localScale.Set(0.6f, 0.1f, 0.6f);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        _myCollider = GetComponent<Collider>();
        _iAmPressed = StartPressed;
        if (_iAmPressed)
        {
            MyWater.localScale.Scale(new Vector3(1, 0, 1));
            MyWater.gameObject.SetActive(false);
            _myCollider.enabled = false;
            transform.position -= Vector3.up * 0.09f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DoSwitch();
            TheirSwitch.DoSwitch();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_iAmPressed)
        {
            // water and button down
            if (MyWater.localScale.y > 0)
            {
                MyWater.localScale -= Vector3.up * Speed * Time.deltaTime;
                if (MyWater.localScale.y <= 0) MyWater.gameObject.SetActive(false);
            }
        }
        else
        {
            // water and button up
            if (MyWater.localScale.y < 1)
            {
                MyWater.localScale += Vector3.up * Speed * Time.deltaTime;
                if (MyWater.localScale.y > 1) MyWater.localScale.Set(1f, 1f, 1f);
            }
        }
    }
}