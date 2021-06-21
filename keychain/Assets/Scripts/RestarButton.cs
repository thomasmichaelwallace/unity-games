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
        _fader.GotoScene(scene.buildIndex);
    }

    public void GoNext()
    {
        var scene = SceneManager.GetActiveScene();
        _fader.GotoScene(scene.buildIndex + 1); // ok as last is ending and no ui
    }
    
    public void GoPrev()
    {
        var scene = SceneManager.GetActiveScene();
        _fader.GotoScene(scene.buildIndex - 1); // ok as first is title and no ui
    }

    public void RestartGame()
    {
        _fader.GotoScene(0);
    }
}