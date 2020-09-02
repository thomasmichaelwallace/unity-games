using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{
    void FixedUpdate()
    {
        if (transform.position.y < -5)
        {
            // fallen off
            SceneManager.LoadScene(0); // restart
        }        
    }
}
