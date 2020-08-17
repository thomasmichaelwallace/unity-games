using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawsController : MonoBehaviour
{
    private readonly float attackDistance = 3f;
    private readonly float strength = 1f;
    private readonly float hitForce = 1f;

    private Animator[] animators;
    private bool isAttacking;

    private void Start()
    {
        animators = GetComponentsInChildren<Animator>();
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