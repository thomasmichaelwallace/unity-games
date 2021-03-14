using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        var mouse = Input.mousePosition;
        var toward = _camera.ScreenToWorldPoint(mouse);
        var position = transform.position;
        toward.z = position.z;
        transform.localRotation = Quaternion.FromToRotation(position, toward);
    }
}