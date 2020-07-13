using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - this.player.transform.position;        
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;       
    }
}
