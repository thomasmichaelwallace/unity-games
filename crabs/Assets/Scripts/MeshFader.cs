// adapted from: https://github.com/unitycoder/SimpleMeshExploder/blob/master/Assets/Scripts/MeshFader.cs
using System.Collections;
using UnityEngine;

public class MeshFader : MonoBehaviour
{
    private readonly float maximumLifeTime = 2f;
    private readonly float fadeTime = 1f;

    private float timer;

    private bool fadingOut = false;

    private void Start()
    {
        timer = maximumLifeTime;
    }

    private void Update()
    {
        if (fadingOut) return;

        timer -= Time.deltaTime;
        if (timer < 0f || GetComponent<Rigidbody>().IsSleeping())
        {
            fadingOut = true;
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        var render = GetComponent<Renderer>();

        var startColor = render.material.color;
        var endColor = new Color(1, 1, 1, 0); // fade to white

        for (float t = 0.0f; t < fadeTime; t += Time.deltaTime)
        {
            render.material.color = Color.Lerp(startColor, endColor, t / fadeTime);
            yield return null;
        }
        Destroy(gameObject);
    }
}