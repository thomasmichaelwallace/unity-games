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
        bool turning = Input.GetButton("Fire2"); // alt

        float hoizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (Mathf.Abs(vertical) > Mathf.Abs(hoizontal))
        {
            // vertical is alternative to turning
            hoizontal = vertical;
            turning = true;
        }

        if (turning)
        {
            Vector3 rotation = new Vector3(0, hoizontal, 0) * TurnSpeed;
            transform.eulerAngles += (rotation * Time.deltaTime);
        }
        else
        {
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