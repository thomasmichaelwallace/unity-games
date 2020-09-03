using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public CanvasGroup fader;
    public TextMeshProUGUI uiGoal;
    public SpawnController spawner;
    public Transform player;
    public Transform ball;
    public TextMeshProUGUI uiText;
    
    private readonly float pitchX = 25;
    private readonly float pitchZ = 30;
    private readonly int count = 20;

    private Vector3 _playerStart;
    private Vector3 _ballStart;
    
    private int _score = 0;
    
    private readonly int _killWeight = 1;
    private readonly int _goalWeight = 10;
    private readonly int _outWeight = 10;
    private readonly float _gameLength = 60;

    private bool _ending;
    private bool _restarting = true;
    private string _template;
    private readonly float _fadeSpeed = 2;
    private readonly float _fadeHold = 1f;
    private readonly float _scoreHold = 5f;

    private float _faderTimer;
    private float _timer;
    
    void Start()
    {
        _playerStart = player.position;
        _ballStart = ball.position;
        _template = uiText.text;
        _timer = _gameLength;
        DoMatch();
        _restarting = true; // show title
        fader.alpha = 1;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0 && !_ending)
        {
            _timer = 0;
            _ending = true;
            DoScreen($"final score\n{_score}", new Color(1, 0.8871336f, 0.3254717f));
            _faderTimer = -_scoreHold;
        }

        uiText.text = _template
            .Replace("{s}", _score.ToString())
            .Replace("{t}", _timer.ToString("0.00"));


        if (!_restarting) return;
        
        if (fader.alpha < 1)
        {
            fader.alpha += Time.deltaTime * _fadeSpeed;
        }
        else
        {
            _faderTimer += Time.deltaTime;
            if (_faderTimer > _fadeHold)
            {
                if (_ending) SceneManager.LoadScene(0);
                DoMatch();
            }
        }
        
    }

    private void DoMatch()
    {
        _restarting = false;
        fader.alpha = 0;

        spawner.Spawn(pitchX, pitchZ, count);
        player.position = _playerStart;
        ball.position = _ballStart;
        ball.GetComponent<Rigidbody>().velocity = Vector3.up * 5;
    }

    private void DoScreen(string text, Color color)
    {
        uiGoal.text = text;
        uiGoal.color = color;
        _restarting = true;
        _faderTimer = 0;
    }

    public void Score(int points)
    {
        if (_restarting) return;
        _score += (points * _goalWeight);
        DoScreen("GOAL!", new Color(0.4431373f, 0.9882354f, 0.6745098f));
    }

    public void Kill()
    {
        _score += _killWeight;
    }
    
    private void FixedUpdate()
    {
        if (_restarting) return;
        if (player.position.y < -1 || ball.position.y < -1)
        {
            _score -= _outWeight;
            DoScreen("OUT!", new Color(0.945098f, 0.4509804f, 0.5556951f));
        }
    }
}
