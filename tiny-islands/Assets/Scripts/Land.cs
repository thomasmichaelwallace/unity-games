using UnityEngine;

public class Land : MonoBehaviour
{
    private const float SinkPoint = -10;
    private const float Height = 10f;
    private const float SinkDistance = 50f;

    private Transform _boat;
    private Guy[] _guys;
    private bool _isSinking;
    private bool _isSunk;
    private float _sqrSinkDistance;

    private void Awake()
    {
        _boat = FindObjectOfType<BoatInput>().transform;
        _sqrSinkDistance = Mathf.Pow(SinkDistance, 2f);
        _guys = transform.parent.GetComponentsInChildren<Guy>();
    }

    private void Update()
    {
        if (_isSunk) return;

        var t = transform;
        var position = t.position;

        if (!_isSinking)
        {
            var distance = (position - _boat.position).sqrMagnitude;
            if (distance < _sqrSinkDistance) StartSinking();
            return;
        }

        position.y -= Water.RisingSpeed * Time.deltaTime;
        t.position = position;

        if (!(position.y < SinkPoint - Height)) return;

        _isSunk = true;
        Destroy(gameObject);
    }

    private void StartSinking()
    {
        if (_isSinking) return;
        _isSinking = true;
        
        foreach (var guy in _guys) guy.StartSinking();
    }
}