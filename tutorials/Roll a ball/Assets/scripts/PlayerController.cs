using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Text countText;
    public Text winText;

    private Rigidbody rb;
    private int count;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winText.text = "";
    }

    void FixedUpdate()
    {
        float moveHoizontal = Input.GetAxis("Horizontal");
        float moveVeritcal = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHoizontal, 0.0f, moveVeritcal);
        rb.AddForce(movement * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
        }
    }

    private void SetCountText()
    {
        countText.text = $"Count: {count}";
        if (count >= 12)
        {
            winText.text = "You Win!";
        }
    }
}
