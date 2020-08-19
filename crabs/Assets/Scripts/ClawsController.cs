using UnityEngine;

public class ClawsController : MonoBehaviour
{
    [SerializeField]
    private float strength = 1f;

    private readonly float attackDistance = 2.0f;
    private readonly float attackRadius = 1f;
    private readonly float rightJabDelay = 0.1f;

    private bool isAttacking;
    private Animator leftClaw;
    private Animator rightClaw;
    private float jabTimer;

    private void Start()
    {
        Animator[] animators = GetComponentsInChildren<Animator>();
        leftClaw = animators[0];
        rightClaw = animators[1];
    }

    private void Update()
    {
        bool fireButtonDown = Input.GetButton("Fire1");

        if (fireButtonDown && !isAttacking)
        {
            isAttacking = true;
            leftClaw.SetBool("isAttacking", true);
            jabTimer = rightJabDelay;
        }
        else if (!fireButtonDown && isAttacking)
        {
            isAttacking = false;
            leftClaw.SetBool("isAttacking", false);
            rightClaw.SetBool("isAttacking", false);
        }

        if (isAttacking && jabTimer > 0)
        {
            // delay right claw jab animation to give a '1-2' feeling
            jabTimer -= Time.deltaTime;
            if (jabTimer <= 0) rightClaw.SetBool("isAttacking", true);
        }

        if (isAttacking)
        {
            // crabs are wide, take three casts
            if (
                Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, attackDistance)
                || Physics.Raycast(transform.position + transform.TransformDirection(Vector3.left) * attackRadius, transform.TransformDirection(Vector3.forward), out hit, attackDistance)
                || Physics.Raycast(transform.position + transform.TransformDirection(Vector3.right) * attackRadius, transform.TransformDirection(Vector3.forward), out hit, attackDistance)
            )
            {
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                if (enemy) enemy.TakeDamage(strength);
            }
        }
    }
}