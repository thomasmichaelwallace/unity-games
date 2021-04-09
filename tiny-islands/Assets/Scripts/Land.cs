using UnityEngine;

public class Land : MonoBehaviour
{
    private const float SinkPoint = -10;
    private const float Height = 10f;

    private bool _isSunk;

    private void Update()
    {
        if (_isSunk) return;

        var t = transform;
        var position = t.position;
        position.y -= Water.RisingSpeed * Time.deltaTime;
        t.position = position;

        if (!(position.y < SinkPoint - Height)) return;

        _isSunk = true;
        Destroy(gameObject);
    }
}