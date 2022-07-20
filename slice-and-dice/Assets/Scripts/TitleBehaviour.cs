using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleBehaviour : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            var gameManager = FindObjectOfType<GameManager>();
            SceneManager.LoadScene(gameManager ? "Main" : "Tutorial");
        }
    }
}