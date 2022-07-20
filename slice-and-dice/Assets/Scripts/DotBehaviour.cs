using System;
using System.Collections.Generic;
using UnityEngine;

public class DotBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject dot;
    private AudioBehaviour _audio;
    private bool _isShard;

    public int ShardDots { get; private set; }
    public int WholeDots { get; private set; }

    private void Awake()
    {
        _audio = FindObjectOfType<AudioBehaviour>();
    }

    private void OnMouseOver()
    {
        if (_isShard) return;
        if (!Input.GetMouseButtonDown(0)) return;

        if (WholeDots < 6)
            WholeDots += 1;
        else
            WholeDots = 1;
        _audio.PlayRoll();
        DrawDots();
    }

    private static List<Vector2> GetDotPositions(int count)
    {
        const float d = 0.55f;

        return count switch
        {
            1 => new List<Vector2> {new(0, 0)},
            2 => new List<Vector2> {new(-d, d), new(d, -d)},
            3 => new List<Vector2> {new(-d, d), new(0, 0), new(d, -d)},
            4 => new List<Vector2> {new(-d, d), new(d, d), new(-d, -d), new(d, -d)},
            5 => new List<Vector2>
            {
                new(-d, d),
                new(d, d),
                new(0, 0),
                new(-d, -d),
                new(d, -d)
            },
            6 => new List<Vector2>
            {
                new(-d, d),
                new(d, d),
                new(-d, 0),
                new(d, 0),
                new(-d, -d),
                new(d, -d)
            },
            _ => throw new ArgumentException("dot count must be 1 - 6")
        };
    }

    private void DestroyDots()
    {
        for (var i = transform.childCount - 1; i >= 0; i--) Destroy(transform.GetChild(i).gameObject);
    }

    public void Configure(int diceDots, bool isWhole)
    {
        WholeDots = diceDots;
        _isShard = !isWhole;
        DrawDots();
    }

    private void DrawDots()
    {
        DestroyDots();

        var target = GetComponent<PolygonCollider2D>();
        if (target is null)
        {
            Debug.LogWarning("Expected collider!");
            return;
        }

        var t = transform;
        var points = GetDotPositions(WholeDots);

        var c = 0;
        foreach (var p in points)
        {
            var w = t.TransformPoint(p);

            if (!target.OverlapPoint(w)) continue;

            Instantiate(dot, w, Quaternion.identity, t);
            c += 1;
        }

        ShardDots = c;
    }
}