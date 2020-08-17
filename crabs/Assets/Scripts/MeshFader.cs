using UnityEngine;
using System.Collections;

// thnaks to: https://github.com/unitycoder/SimpleMeshExploder/blob/master/Assets/Scripts/MeshFader.cs

public class MeshFader : MonoBehaviour
{
    private bool fadeOut = false;
    private float timer = 2f;

    private void Update()
    {
        if (fadeOut) return;

        timer -= Time.deltaTime;
        if (timer < 0f || GetComponent<Rigidbody>().IsSleeping())
        {
            fadeOut = true;
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        float fadeTime = 1.0f;
        var rend = GetComponent<Renderer>();

        var startColor = rend.material.color;
        var endColor = new Color(1, 1, 1, 0);

        for (float t = 0.0f; t < fadeTime; t += Time.deltaTime)
        {
            rend.material.color = Color.Lerp(startColor, endColor, t / fadeTime);
            yield return null;
        }
        Destroy(gameObject);
    }
}