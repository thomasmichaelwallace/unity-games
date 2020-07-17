using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public float switchDuration = 1f;
    public float canvasDuration = 1f;
    public float holdDuration = 1f;
    public Text switchText;
    public CanvasGroup winCanvas;
    public CanvasGroup loseCanvas;
    public Text winText;
    public Material metalMaterial;
    public Material glassMaterial;

    private Rigidbody m_Rigidbody;
    private MeshRenderer m_MeshRenderer;
    private bool m_IsMetal;
    private bool m_InDeath;
    private bool m_IsLose;
    private bool m_IsWin;
    private float m_Switch;
    private float m_CanvasTimer;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_IsMetal = true;
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            winText.text = "You win the game.";
        } else
        {
            winText.text = "You win this level.";
        }
    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        m_Rigidbody.AddForce(movement * speed);
    }

    private void Update()
    {
        if (m_IsLose || m_IsWin)
        {
            m_CanvasTimer += Time.deltaTime;
            var canvas = m_IsLose ? loseCanvas : winCanvas;
            canvas.alpha = m_CanvasTimer / canvasDuration;

            if (m_CanvasTimer > canvasDuration + holdDuration)
            {
                int index = SceneManager.GetActiveScene().buildIndex;
                if (m_IsWin)
                {
                    SceneManager.LoadScene(index + 1);
                }
                else
                {
                    SceneManager.LoadScene(index);
                }
            }

        }
        else
        {
            m_Switch += Time.deltaTime;
            float remaining = switchDuration - m_Switch;
            string style = m_IsMetal ? "Invincible" : "Vulnerable";
            switchText.text = $"{ style } for {remaining, 0:F1} seconds";
            if (remaining < 0)
            {
                m_Switch = 0;
                SwitchMaterial();
            }

            if (m_Rigidbody.transform.position.y < -50)
            {
                // fallen off the world.
                m_IsLose = true;
            }
        }
    }

    private void SwitchMaterial()
    {
        m_IsMetal = !m_IsMetal;
        m_MeshRenderer.material = m_IsMetal ? metalMaterial : glassMaterial;
        SetDeath();
    }

    private void SetDeath(bool? value = null)
    {
        if (value != null) m_InDeath = (bool)value;
        if (m_InDeath && !m_IsMetal) m_IsLose = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Killer")) SetDeath(true);
        if (other.CompareTag("Finish") && !m_IsLose) m_IsWin = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Killer") && m_InDeath) SetDeath(false);
    }
}
