using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawsController : MonoBehaviour
{
    public Material Hightlight;

    private readonly float attackDistance = 3f;
    private readonly float strength = 1f;
    private readonly float hitForce = 1f;

    private MeshRenderer[] claws;
    private Material clawMaterial;
    private bool isAttacking;

    private void Start()
    {
        claws = GetComponentsInChildren<MeshRenderer>();
        clawMaterial = claws[0].material;
        isAttacking = false;
    }

    private void Update()
    {
        // TODO: replace "hightlight" with attack animation
        // OPTION: add more interesting left vs right combat
        bool fireButtonDown = Input.GetButton("Fire1");
        if (fireButtonDown && !isAttacking)
        {
            isAttacking = true;
            foreach (MeshRenderer claw in claws)
            {
                claw.material = Hightlight;
            }
        }
        else if (!fireButtonDown && isAttacking)
        {
            isAttacking = false;
            foreach (MeshRenderer claw in claws)
            {
                claw.material = clawMaterial;
            }
        }

        if (isAttacking)
        {
            Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward) * attackDistance);
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, attackDistance))
            {
                if (hit.rigidbody)
                {
                    Vector3 direction = hit.transform.position - transform.position;
                    hit.rigidbody.AddForce(direction * hitForce);
                }
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                if (enemy)
                {
                    enemy.TakeDamage(strength);
                }
            }
        }
    }
}