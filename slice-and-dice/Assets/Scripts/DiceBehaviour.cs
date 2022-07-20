using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DiceBehaviour : MonoBehaviour
{
    [SerializeField] private float initialPush;
    [SerializeField] private float initialDelay;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject dicePrefab;
    [SerializeField] private TextMeshProUGUI scoreText;
    private AudioBehaviour _audio;
    private float _countdown;
    private bool _end;
    private readonly float _endAtY = -7.25f;
    private float _endCountdown;
    private const float ExcludeAtX = 15f; // as so.
    private const float ExcludeAtY = 8f; // prevent flying objects mucking things up.
    private GameManager _gameManager;
    private bool _pushed;
    private string _target;
    private bool _won;

    private void Start()
    {
        _countdown = initialDelay;

        var count = Random.Range(1, 4 - 1);
        var dx = count switch
        {
            2 => 2.25f * 2,
            3 => 3.5f,
            _ => 0f
        };

        var t = transform;
        const float y = -6.5f;
        var x = count == 2 ? -dx / 2 : -dx;
        for (var i = 0; i < count; i += 1)
        {
            var dz = Random.Range(-30f, 30f);
            var q = Quaternion.Euler(0, 0, 0 + dz);
            var o = Instantiate(dicePrefab, new Vector3(x, y), q, t);
            x += dx;

            var d = o.GetComponent<DotBehaviour>();
            d.Configure(Random.Range(1, 6 + 1), true);
            var r = o.GetComponent<Rigidbody2D>();
            r.Sleep();
        }

        _target = GenerateTarget(count);
        _pushed = false;

        _gameManager = FindObjectOfType<GameManager>();
        UpdateScoreText();

        _audio = FindObjectOfType<AudioBehaviour>();
    }

    private void Update()
    {
        if (!_pushed)
        {
            _countdown -= Time.deltaTime;
            if (_countdown > 0) return;

            _pushed = true;
            var dice = GetComponentsInChildren<Rigidbody2D>();
            foreach (var d in dice)
            {
                var dz = Random.Range(-2.5f, 2.5f);
                var q = Quaternion.Euler(0, 0, 0 + dz);
                var f = q * new Vector3(0, initialPush);

                d.WakeUp();
                d.AddForce(f, ForceMode2D.Impulse);
            }
        }

        if (_end)
        {
            SceneManager.LoadScene(_gameManager.Power < 0 ? "Gameover" : "Main");
        }

        UpdateScoreView();
    }

    private string GenerateTarget(int count)
    {
        var target = "";

        for (var i = 0; i < count; i += 1)
        {
            var s = Random.Range(0, 6) + 1;
            if (s == 1) //  || Random.value < 0.1f)
            {
                // no slice required
                target += s;
                continue;
            }

            var l = Random.Range(0, s - 1) + 1;
            target += l;

            var r = s - l;
            if (r == 1) // || Random.value < 0.8f)
            {
                // only one slice required
                target += r;
                continue;
            }

            var rl = Random.Range(0, r - 1) + 1;
            target += rl;
            var rr = r - rl;
            target += rr;
        }

        return target;
    }

    private void UpdateScoreView()
    {
        var score = GetScore();
        if (_won) return;

        var targets = _target.ToCharArray();
        var scores = score.ToCharArray();

        if (scores.Length > targets.Length) return;

        const string ok = "#D9F8C4";
        var matches = 0;

        var output = "";
        for (var i = 0; i < targets.Length; i += 1)
        {
            var t = targets[i];
            var s = i < scores.Length ? scores[i] : '?';
            if (s == t)
            {
                output += "<color=" + ok + ">" + t + "</color>";
                matches += 1;
            }
            else
            {
                output += t;
            }
        }

        // Debug.Log("m " + _target + " / " + score);
        text.text = output;

        if (matches != targets.Length) return;
        _gameManager.SetScore(_target);
        UpdateScoreText();
        _audio.PlayWin();
        _won = true;
    }

    private void UpdateScoreText()
    {
        scoreText.text = _gameManager.Score + " Points";
    }

    private string GetScore()
    {
        var rigidbody2Ds = GetComponentsInChildren<Rigidbody2D>();
        Array.Sort(rigidbody2Ds,
            Comparer<Rigidbody2D>.Create((a, b) => a.worldCenterOfMass.x.CompareTo(b.worldCenterOfMass.x)));
        var score = "";

        var maxY = Mathf.NegativeInfinity;

        foreach (var rb in rigidbody2Ds)
        {
            if (-ExcludeAtX < rb.position.x && rb.position.x < ExcludeAtX && rb.position.y < ExcludeAtY)
                maxY = Mathf.Max(rb.position.y, maxY);
            if (!rb.gameObject.activeSelf) continue;
            var dot = rb.gameObject.GetComponent<DotBehaviour>();
            var dots = dot.ShardDots;
            if (dots > 0) score += dots;
        }

        if (maxY < _endAtY) _end = true;

        return score;
    }
}