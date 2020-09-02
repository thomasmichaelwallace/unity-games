using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // TODO: fire pits of hell
    
    public SpawnController spawner;
    public Transform player;
    public Transform ball;
    
    private readonly float pitchX = 25;
    private readonly float pitchZ = 30;
    private readonly int count = 20;

    private Vector3 _playerStart;
    private Vector3 _ballStart;

    private int _score = 0;
    private bool _restarting = false;
    
    void Start()
    {
        _playerStart = player.position;
        _ballStart = ball.position;
        DoMatch();
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
        // TODO: actually show score
        Debug.Log($"current score: {_score}");
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
