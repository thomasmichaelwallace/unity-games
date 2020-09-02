using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalBehaviour : MonoBehaviour
{
    public int points;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            Debug.Log($"Score! +{points}");
            StartCoroutine(FadeOut());
        }
    }
    
    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(0);
    }

}
