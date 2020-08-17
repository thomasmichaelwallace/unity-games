using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawsController : MonoBehaviour
{
    private readonly float attackDistance = 2.0f;
    private readonly float attackRadius = 1f;
    private readonly float strength = 1f;

    private Animator[] animators;
    private bool isAttacking;

    private void Start()
    {
        animators = GetComponentsInChildren<Animator>();
        isAttacking = false;
    }

    private void Update()
    {
        bool fireButtonDown = Input.GetButton("Fire1");
        if (fireButtonDown && !isAttacking)
        {
            isAttacking = true;
            foreach (Animator animator in animators)
            {
                animator.SetBool("isAttacking", true);
            }
        }
        else if (!fireButtonDown && isAttacking)
        {
            isAttacking = false;
            foreach (Animator animator in animators)
            {
                animator.SetBool("isAttacking", false);
            }
        }

        if (isAttacking)
        {
            Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward) * attackDistance);
            Debug.DrawLine(transform.position + transform.TransformDirection(Vector3.left) * attackRadius, transform.position + transform.TransformDirection(Vector3.left) * attackRadius + transform.TransformDirection(Vector3.forward) * attackDistance);
            Debug.DrawLine(transform.position + transform.TransformDirection(Vector3.right) * attackRadius, transform.position + transform.TransformDirection(Vector3.right) * attackRadius + transform.TransformDirection(Vector3.forward) * attackDistance);
            RaycastHit hit;
            if (
                Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, attackDistance)
                || Physics.Raycast(transform.position + transform.TransformDirection(Vector3.left) * attackRadius, transform.TransformDirection(Vector3.forward), out hit, attackDistance)
                || Physics.Raycast(transform.position + transform.TransformDirection(Vector3.right) * attackRadius, transform.TransformDirection(Vector3.forward), out hit, attackDistance)
            )
            {
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                if (enemy)
                {
                    enemy.TakeDamage(strength);
                }
            }
        }
    }
}