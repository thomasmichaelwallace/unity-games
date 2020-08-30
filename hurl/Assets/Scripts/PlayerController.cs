using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Rigidbody ball;

    private readonly float speed = 2.5f;
    private readonly float strength = 5f;
    private readonly float angle = 0.5f; // raise at 2:1
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 position = transform.position;
        position.x += horizontal * speed * Time.deltaTime;
        position.z += vertical * speed * Time.deltaTime;
        transform.position = position;
        
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 impact = new Vector3(0, angle * strength, -strength);
            ball.AddForce(impact, ForceMode.VelocityChange);
        }
    }
}
