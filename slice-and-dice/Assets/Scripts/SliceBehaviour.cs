using UnityEngine;

public class SliceBehaviour : MonoBehaviour
{
    private AudioBehaviour _audio;
    private Camera _camera;
    private DividableBehaviour _dividable;
    private Vector2? _sliceEnd;
    private Vector2? _sliceStart;

    private void Awake()
    {
        _camera = Camera.main;
        _dividable = GetComponent<DividableBehaviour>();
        _audio = FindObjectOfType<AudioBehaviour>();
    }

    private void OnMouseEnter()
    {
        if (!Input.GetMouseButton(0))
        {
            _sliceStart = null;
            return;
        }

        _sliceStart = GetMousePosition();
        _audio.PlayStart();
        _sliceEnd = null;
    }

    private void OnMouseExit()
    {
        if (!Input.GetMouseButton(0)) return;

        if (_sliceStart is null || _sliceEnd is null) return;

        _audio.PlayEnd();
        _dividable.Divide((Vector2) _sliceStart, (Vector2) _sliceEnd);
    }

    private void OnMouseOver()
    {
        if (!Input.GetMouseButton(0)) return;
        _sliceStart ??= GetMousePosition();
        _sliceEnd = GetMousePosition();
    }

    private Vector2? GetMousePosition()
    {
        var worldMouse = _camera.ScreenToWorldPoint(Input.mousePosition);
        var hitInfo = Physics2D.Raycast(worldMouse, Vector2.zero);
        if (!hitInfo)
        {
            Debug.LogWarning("could not get slice position!");
            return null;
        }

        var p = transform.position;
        return hitInfo.point - new Vector2(p.x, p.y);
    }
}