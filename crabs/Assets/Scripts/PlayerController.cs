using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MeshRenderer[] Claws;
    public Material Hightlight;

    private float speed = 10f;
    private float lookSpeed = 20f;
    private float attackDistance = 10f;
    private float strength = 1f;

    private CharacterController characterController;
    private Material clawMaterial;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        clawMaterial = Claws[0].material;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButton("Fire2"))
        {
            float hoizontal = Input.GetAxis("Horizontal");
            Vector3 rotation = new Vector3(0, hoizontal, 0) * lookSpeed;
            transform.eulerAngles += (rotation * Time.deltaTime);
        }
        else
        {
            float hoizontal = Input.GetAxis("Horizontal");
            Vector3 movement = transform.TransformDirection(Vector3.right) * hoizontal * speed;
            characterController.Move(movement * Time.deltaTime);
        }

        if (Input.GetButton("Fire1"))
        {
            foreach (MeshRenderer claw in Claws)
            {
                claw.material = Hightlight;
            }

            // Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward) * attackDistance);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, attackDistance))
            {
                var enemy = hit.transform.GetComponent<EnemyController>();
                if (enemy)
                {
                    enemy.TakeDamage(strength);
                }
            }
        }
        else
        {
            foreach (MeshRenderer claw in Claws)
            {
                claw.material = clawMaterial;
            }
        }
    }
}