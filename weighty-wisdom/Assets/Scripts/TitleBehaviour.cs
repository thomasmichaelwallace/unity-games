using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleBehaviour : MonoBehaviour
{
    private bool _isEnd;

    private void Start()
    {
        if (LevelManager.LevelNo > 1) _isEnd = true;
        if (!_isEnd) return;
        var text = GetComponent<TextMeshProUGUI>();
        text.text = @"
<size=200%><color=#FFC764>Weighty</color> <color=#FF577F>Wisdom</color></size>
<size=50%>
</size><size=130%>Thanks for playing!</size>
";
    }

    private void Update()
    {
        if (_isEnd) return;
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical")) SceneManager.LoadScene(1);
    }
}