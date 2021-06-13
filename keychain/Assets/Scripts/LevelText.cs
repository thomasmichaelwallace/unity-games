using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LevelText : MonoBehaviour
{
    private void Awake()
    {
        var scene = SceneManager.GetActiveScene().buildIndex;
        var text = $"level {scene}";
        var tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = text;
    }
}