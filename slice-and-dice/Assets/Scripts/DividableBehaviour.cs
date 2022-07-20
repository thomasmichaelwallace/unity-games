using System;
using System.Collections.Generic;
using UnityEngine;

public class DividableBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float sliceStrength;
    private PolygonCollider2D _collider;
    private DotBehaviour _dot;
    private PolyCollider2DToMesh _mesh;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _collider = GetComponent<PolygonCollider2D>();
        if (_collider.pathCount != 1)
            throw new NotSupportedException("PolygonCollider2D must have precisely one path.");

        // var meshRenderer = GetComponent<MeshRenderer>();
        // meshRenderer.material.color = Random.ColorHSV();

        _mesh = GetComponent<PolyCollider2DToMesh>();

        _dot = GetComponent<DotBehaviour>();
    }

    private static Vector2? Intersection(Vector2 pathPointA, Vector2 pathPointB, Vector2 slicePointA,
        Vector2 slicePointB)
    {
        var isPathPointAToB = pathPointA.x < pathPointB.x;
        var m1 = isPathPointAToB ? pathPointA : pathPointB;
        var m2 = isPathPointAToB ? pathPointB : pathPointA;

        var isSlicePointAToB = slicePointA.x < slicePointB.x;
        var n1 = isSlicePointAToB ? slicePointA : slicePointB;
        var n2 = isSlicePointAToB ? slicePointB : slicePointA;

        // my = ax + b; ny = cx + d; ax + b = cx + d; (a - c)x = (d - b); x = (d - b)/(a - c)
        var md = m2 - m1;
        var a = md.y / md.x;
        var b = m1.y - m1.x * a;

        var nd = n2 - n1;
        var c = nd.y / nd.x;
        var d = n1.y - n1.x * c;

        var pathIsVertical = !float.IsFinite(a);
        var sliceIsVertical = !float.IsFinite(c);

        float x;
        float y;
        if (pathIsVertical && !sliceIsVertical)
        {
            // path is vertical; solve for y along slice
            x = m1.x;
            y = c * x + d;
        }
        else if (sliceIsVertical && !pathIsVertical)
        {
            // slice is vertical
            x = n1.x;
            y = a * x + b;
        }
        else
        {
            // intersect slopes
            x = (d - b) / (a - c);
            y = a * x + b;
        }

        if (x < m1.x || x > m2.x) return null;

        var isPathBottomToTop = pathPointA.y < pathPointB.y;
        var bottom = isPathBottomToTop ? pathPointA.y : pathPointB.y;
        var top = isPathBottomToTop ? pathPointB.y : pathPointA.y;
        if (y < bottom || y > top) return null;

        return new Vector2(x, y);
    }

    public void Divide(Vector2 start, Vector2 end)
    {
        var slope = end - start;
        // assume 5x grow in line ensures points are outside of polygon
        var sA = start - slope * 5;
        var sB = end + slope * 5;

        var path = _collider.GetPath(0);

        List<Vector2> intersections = new();
        List<int> fromPointIndices = new();

        for (var i = 0; i < path.Length; i += 1)
        {
            var a = path[i];
            var b = path[i + 1 == path.Length ? 0 : i + 1];

            var intersection = Intersection(a, b, sA, sB);
            if (intersection == null) continue;

            intersections.Add((Vector2) intersection);
            fromPointIndices.Add(i);
        }

        switch (intersections.Count)
        {
            case 2:
                var shards = GetShards(intersections, fromPointIndices);
                var sliceCenter = Vector2.Lerp(start, end, 0.5f);
                Shard(shards, transform.TransformPoint(sliceCenter));
                break;
            case > 0:
                Debug.LogError("not exactly zero or two intersections");
                break;
        }
    }

    private List<Vector2>[] GetShards(IReadOnlyList<Vector2> intersections, IReadOnlyList<int> fromPointIndices)
    {
        var path = _collider.GetPath(0);

        var fromPointIndex = fromPointIndices[0];
        var fromPoint = intersections[0];
        var untilPointIndex = fromPointIndices[1];
        var untilPoint = intersections[1];

        List<Vector2> first = new() {fromPoint};
        if (untilPointIndex < fromPointIndex) untilPointIndex += path.Length;
        for (var i = fromPointIndex + 1; i <= untilPointIndex; i += 1) first.Add(path[i % path.Length]);
        first.Add(untilPoint);

        List<Vector2> second = new() {untilPoint};
        if (fromPointIndex < untilPointIndex) fromPointIndex += path.Length;
        for (var i = untilPointIndex + 1; i <= fromPointIndex; i += 1) second.Add(path[i % path.Length]);
        second.Add(fromPoint);

        List<Vector2>[] shards = {first, second};
        return shards;
    }

    private void Shard(IReadOnlyList<List<Vector2>> shards, Vector2 sliceCentre)
    {
        var t = transform;
        var p = t.position;
        var r = t.rotation;
        var o = t.parent;

        var first = Instantiate(prefab, p, r, o);
        var firstDivide = first.GetComponent<DividableBehaviour>();
        // var firstDivide = this;
        firstDivide._collider.SetPath(0, shards[0]);
        firstDivide._mesh.Reshape();
        firstDivide._dot.Configure(_dot.WholeDots, false);
        firstDivide._rb.velocity = _rb.velocity;
        firstDivide._rb.angularVelocity = _rb.angularVelocity;

        var second = Instantiate(prefab, p, r, o);
        var secondDivide = second.GetComponent<DividableBehaviour>();
        secondDivide._collider.SetPath(0, shards[1]);
        secondDivide._mesh.Reshape();
        secondDivide._dot.Configure(_dot.WholeDots, false);
        secondDivide._rb.velocity = _rb.velocity;
        secondDivide._rb.angularVelocity = _rb.angularVelocity;

        var firstDivideForce = (firstDivide._rb.worldCenterOfMass - sliceCentre).normalized * sliceStrength;
        firstDivide._rb.AddForceAtPosition(firstDivideForce, sliceCentre, ForceMode2D.Impulse);
        var secondDivideForce = (secondDivide._rb.worldCenterOfMass - sliceCentre).normalized * sliceStrength;
        secondDivide._rb.AddForceAtPosition(secondDivideForce, sliceCentre, ForceMode2D.Impulse);

        Destroy(gameObject);
    }
}