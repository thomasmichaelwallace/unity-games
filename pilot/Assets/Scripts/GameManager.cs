using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool ShouldRestart = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ShouldRestart)
            {
                int index = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(index);
            } else
            {
                gameObject.SetActive(false);
                ShouldRestart = true;
            }
        }
    }
}