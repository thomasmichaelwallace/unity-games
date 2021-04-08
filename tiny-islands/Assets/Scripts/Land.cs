using UnityEngine;

public class Land : MonoBehaviour
{
    private const float SinkPoint = -10;
    [SerializeField] private float height = 10f;

    private bool _isSunk;

    private void Update()
    {
        if (_isSunk) return;

        var t = transform;
        var position = t.position;
        position.y -= Water.RisingSpeed * Time.deltaTime;
        t.position = position;

        if (!(position.y < SinkPoint - height)) return;

        _isSunk = true;
        Destroy(gameObject);
    }
}