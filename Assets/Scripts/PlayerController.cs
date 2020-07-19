using Cinemachine;
using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float Speed = 2f;
    public float JumpHeight = 1.5f;
    public float Gravity = -9.0f;
    public Vector3 Drag = new Vector3(1, 1, 1);
    public float WaterDrag = 2.0f;
    public float Buoyancy = 0.8f;
    public float TurnTime = 0.1f;
    public float CoyoteTime = 0.1f;
    public Transform Camera;
    public Text Score;
    public int MaxScore = 10;
    public Text MessageText;
    public CanvasGroup Fader;
    public float FadeDuration = 1f;
    public float WaitDuration = 1f;
    public GameObject Evils;
    public CinemachineFreeLook VirtualCamera;

    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _isGrounded = true;
    private float _turnVelocity;
    private bool _inWater = false;
    private float _coyoteTime;
    private int _collected = 0;
    private float _fader;
    private bool _showMessage;
    private Vector3 _reset;
    private int _level = 0;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _collected = 0;
        _reset = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        SetScore();
        Reset();
    }

    void Update()
    {
        

        if (_showMessage)
        {
            ShowMessage();
            return;
        }


        _isGrounded = _controller.isGrounded;

        // walking
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        if (move.magnitude >= 0.1f)
        {
            float turnAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + Camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, turnAngle, ref _turnVelocity, TurnTime);
            move = Quaternion.Euler(0f, turnAngle, 0f) * Vector3.forward;

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            _controller.Move(move.normalized * Time.deltaTime * Speed);
        }
        VirtualCamera.m_XAxis.Value = -Input.GetAxis("CamHorizontal");

        // jumping
        if (_isGrounded && _velocity.y < 0) _velocity.y = 0f;
        if (_isGrounded)
        {
            _coyoteTime = CoyoteTime;
        }
        else if (_coyoteTime > 0)
        {
            _coyoteTime -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Fire1") && _coyoteTime > 0)
        {
            _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity);
            _coyoteTime = 0;
        }

        // boyancy
        if (!_inWater)
        {
            _velocity.y += Gravity * Time.deltaTime;
        }
        else
        {
            _velocity.y /= 1 + WaterDrag * Time.deltaTime;
            _velocity.y -= Buoyancy * Gravity * Time.deltaTime;
        }

        // slide
        _velocity.x /= 1 + Drag.x * Time.deltaTime;
        _velocity.y /= 1 + Drag.y * Time.deltaTime;
        _velocity.z /= 1 + Drag.z * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        // infinate fall
        if (transform.position.y < -15) DoDie();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SolidDeath"))
        {
            other.GetComponent<Collider>().isTrigger = false;
            DoDie();
        }
        if (other.CompareTag("Water"))
        {
            _inWater = true;
        }
        if (other.CompareTag("Collect"))
        {
            other.gameObject.SetActive(false);
            _collected += 1;
            SetScore();
        }
        if (other.CompareTag("Catch"))
        {
            other.isTrigger = false;
            DoCatch();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            _inWater = false;
        }
    }

    private void DoDie()
    {
        _showMessage = true;
        MessageText.text = "";
    }

    private void DoCatch()
    {
        _showMessage = true;
        MessageText.text = @"
You found me!
No Fair!
I Want a rematch.
";
        _level += 1;
    }

    private void SetScore()
    {
        Score.text = $"{_collected}/{MaxScore} Found";
    }

    private void ShowMessage()
    {
        _fader += Time.deltaTime;
        Fader.alpha = _fader / FadeDuration;

        if (
            (MessageText.text == "" && _fader > FadeDuration)
            || (_fader > FadeDuration + WaitDuration)
            )
        {
            Reset();
            _showMessage = false;
            Fader.alpha = 0;
            _fader = 0;
        }
    }

    private void Reset()
    {
        transform.position = _reset;
        _velocity = new Vector3();

        int level = 0;
        foreach (Transform child in Evils.transform)
        {
            Debug.Log(level);
            if (level != _level)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
            level += 1;
        }
    }
}
