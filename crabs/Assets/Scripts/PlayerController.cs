using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float Speed = 15f;

    [SerializeField]
    private float TurnSpeed = 40f;

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
            Vector3 rotation = new Vector3(0, hoizontal, 0) * TurnSpeed;
            transform.eulerAngles += (rotation * Time.deltaTime);
        }
        else
        {
            // walking
            float hoizontal = Input.GetAxis("Horizontal");
            Vector3 movement = transform.TransformDirection(Vector3.right) * hoizontal * Speed;
            characterController.Move(movement * Time.deltaTime);
        }

        if (!characterController.isGrounded)
        {
            // falling
            characterController.Move(Vector3.down * gravity * Time.deltaTime);
        }
    }
}