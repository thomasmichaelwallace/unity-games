using System;
using UnityEngine;

public class BouncerBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!LevelManager.Manager.bouncy) return;
        var bike = other.gameObject.GetComponent<BikeBehaviour>();
        if (bike == null) return;
        bike.Bounce();
    }

}
