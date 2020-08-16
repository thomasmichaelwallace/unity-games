using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // TODO: make these feel more reactive
    // OPTION: target velocity rather than speed
    private readonly float speed = 10f;

    private readonly float turnSpeed = 20f;
    private readonly float gravity = 10f;

    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Input.GetButton("Fire2"))
        {
            // rotating
            float hoizontal = Input.GetAxis("Horizontal");
            Vector3 rotation = new Vector3(0, hoizontal, 0) * turnSpeed;
            transform.eulerAngles += (rotation * Time.deltaTime);
        }
        else
        {
            // walking
            float hoizontal = Input.GetAxis("Horizontal");
            Vector3 movement = transform.TransformDirection(Vector3.right) * hoizontal * speed;
            characterController.Move(movement * Time.deltaTime);
        }

        if (!characterController.isGrounded)
        {
            // falling
            characterController.Move(Vector3.down * gravity * Time.deltaTime);
        }
    }
}