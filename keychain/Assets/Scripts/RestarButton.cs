using UnityEngine;
using UnityEngine.SceneManagement;

public class RestarButton : MonoBehaviour
{
    private FaderManager _fader;

    private void Start()
    {
        _fader = FindObjectOfType<FaderManager>();
    }

    public void Restart()
    {
        var scene = SceneManager.GetActiveScene();
        // SceneManager.LoadScene(scene.buildIndex);
        _fader.GotoScene(scene.buildIndex);
    }
}