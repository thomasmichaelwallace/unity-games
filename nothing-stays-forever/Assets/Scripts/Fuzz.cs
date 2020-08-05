using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Fuzz : MonoBehaviour
{
    public Feature[] Features;
    public Story TheStory;

    private Bloom bloom;
    private ChromaticAberration chrome;
    private Grain grain;
    private PostProcessVolume volume;

    private void Start()
    {
        volume = GetComponent<PostProcessVolume>();

        volume.profile.TryGetSettings<Bloom>(out bloom);
        volume.profile.TryGetSettings<ChromaticAberration>(out chrome);
        volume.profile.TryGetSettings<Grain>(out grain);

        bloom.intensity.value = 0;
        chrome.intensity.value = 0;
        grain.intensity.value = 0;
    }

    private void Update()
    {
        float fuzz = 0;
        int parts = 0;
        foreach (Feature feature in Features)
        {
            parts += 1;
            fuzz += feature.CorrectMaterial ? 0 : 1;
            fuzz += feature.CorrectShape ? 0 : 1;
            fuzz += Mathf.Min(1f, feature.Distance / (Screen.height * 0.5f));
        }

        fuzz /= (float)parts;
        bloom.intensity.value = fuzz * 10;
        chrome.intensity.value = fuzz;
        grain.intensity.value = fuzz;
    }
}