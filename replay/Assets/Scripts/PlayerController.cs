﻿using Cinemachine;
using UnityEngine;
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
    public int MaxScore = 24;
    public Text MessageText;
    public CanvasGroup Fader;
    public float FadeDuration = 1f;
    public float WaitDuration = 1f;
    public GameObject Evils;

    // public GameObject EvilMessage;
    public CinemachineFreeLook VirtualCamera;

    private CharacterController _controller;
    private Vector3 _velocity;
    private Animator _animator;
    private bool _isGrounded = true;
    private float _turnVelocity;
    private bool _inWater = false;
    private float _coyoteTime;
    private int _collected = 0;
    private float _fader;
    private bool _showMessage;
    private Vector3 _reset;
    private int _level = 0;
    private bool _win = false;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = gameObject.GetComponentInChildren<Animator>();
        _collected = 0;
        _reset = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        SetScore();
        Reset();
        // EvilMessage.SetActive(false);
    }

    private void Update()
    {
        if (_showMessage)
        {
            ShowMessage();
            return;
        }

        _isGrounded = _controller.isGrounded;

        // walking
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        bool isWalking = !Mathf.Approximately(move.sqrMagnitude, 0f);
        if (isWalking)
        {
            float turnAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + Camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, turnAngle, ref _turnVelocity, TurnTime);
            move = Quaternion.Euler(0f, turnAngle, 0f) * Vector3.forward;

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            _controller.Move(move.normalized * Time.deltaTime * Speed);
        }
        _animator.SetBool("IsWalking", isWalking);

        // camera
        // VirtualCamera.m_XAxis.Value = -Input.GetAxis("CamHorizontal");

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
        if (Input.GetButtonDown("Jump") && _coyoteTime > 0)
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
            other.gameObject.SetActive(false);
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
        _level += 1;
        if (_level >= Evils.transform.childCount)
        {
            if (_collected < MaxScore)
            {
                MessageText.text = @"
You found me!
EVERY TIME!
Looks like you win for now...
But I've still got enough gold to come back!
";
                MessageText.color = new Color(4911683f, 0.6698113f, 0.36966f);
            }
            else
            {
                MessageText.text = @"
You found me! Every time!!
AND YOU FOUND ALL MY GOLD?!!
You win.
With the secret ending!
";
                MessageText.color = new Color(1, 0, 0.4102478f);
            }

            _win = true;
        }
        else
        {
            MessageText.text = @"
You found me!
No Fair!
I Want a do over.
";
        }
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
            if (!_win)
            {
                Reset();
                _showMessage = false;
                Fader.alpha = 0;
                _fader = 0;
            }
        }
    }

    private void Reset()
    {
        transform.position = _reset;
        _velocity = new Vector3();

        //int level = 0;
        //foreach (Transform child in Evils.transform)
        //{
        //    if (level != _level)
        //    {
        //        child.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        child.gameObject.SetActive(true);
        //    }
        //    level += 1;
        //}
    }
}