using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // TODO: fire pits of hell
    
    public SpawnController spawner;
    public Transform player;
    public Transform ball;
    public TextMeshProUGUI uiText;
    
    private readonly float pitchX = 25;
    private readonly float pitchZ = 30;
    private readonly int count = 20;

    private Vector3 _playerStart;
    private Vector3 _ballStart;

    private int _kills = 0;
    private int _score = 0;
    private int _health = 5;
    private bool _restarting = false;
    private string _template;
    
    void Start()
    {
        _playerStart = player.position;
        _ballStart = ball.position;
        _template = uiText.text;
        DoMatch();
        UpdateUI();
    }

    private void DoMatch()
    {
        _restarting = false;
        spawner.Spawn(pitchX, pitchZ, count);
        player.position = _playerStart;
        ball.position = _ballStart;
    }

    public void Score(int points)
    {
        _score += points;
        UpdateUI();
    }

    public void Kill()
    {
        _kills += 1;
        UpdateUI();
    }

    private void UpdateUI()
    {
        uiText.text = _template
            .Replace("{s}", _score.ToString())
            .Replace("{k}", _kills.ToString())
            .Replace("{h}", _health.ToString());
    }

    private void FixedUpdate()
    {
        if (_restarting) return;
        if (player.position.y < -1 || ball.position.y < -1)
        {
            _restarting = true;
            // TODO: actually restart
            DoMatch();
        }
    }
}
