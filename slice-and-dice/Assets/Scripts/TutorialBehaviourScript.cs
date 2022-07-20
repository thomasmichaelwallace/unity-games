using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialBehaviourScript : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetButtonDown("Jump")) SceneManager.LoadScene("Main");
    }
}
