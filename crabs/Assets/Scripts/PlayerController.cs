using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public CanvasGroup Damaged;
    public RectTransform HealthBar;
    public CanvasGroup DeadScreen;
    public TextMeshProUGUI DeadText;

    // TODO: make these feel more reactive
    // OPTION: target velocity rather than speed
    private readonly float speed = 15f;

    private readonly float turnSpeed = 40f;
    private readonly float gravity = 10f;
    private float damageHide = 0.5f;
    private float damageShow = 0;
    private float damagePerSecond = 10f;
    private float totalHeath = 100f;
    private float health = 100f;
    private Vector2 barSize;
    private int killCount = 0;

    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        health = totalHeath;
        barSize = HealthBar.sizeDelta;
    }

    private void Update()
    {
        if (damageShow > 0)
        {
            damageShow -= Time.deltaTime * damageHide;
            Damaged.alpha = damageShow;
        }

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

    public void DoDamage()
    {
        damageShow = 1f;
        health -= damagePerSecond * Time.deltaTime;
        HealthBar.sizeDelta = new Vector2(barSize.x * health / totalHeath, barSize.y);

        if (health <= 0)
        {
            DeadText.text = DeadText.text.Replace("{{kills}}", killCount.ToString());
            DeadScreen.alpha = 1;
        }
    }

    public void AddKill()
    {
        killCount += 1;
    }
}