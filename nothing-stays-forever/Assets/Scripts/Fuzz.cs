using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Fuzz : MonoBehaviour
{
    public GameObject Features;
    public Story TheStory;

    public Feature[] features;
    private Bloom bloom;
    private ChromaticAberration chrome;
    private Grain grain;
    private PostProcessVolume volume;
    private ColorGrading color;

    private void Start()
    {
        features = Features.GetComponentsInChildren<Feature>();

        volume = GetComponent<PostProcessVolume>();

        volume.profile.TryGetSettings(out bloom);
        volume.profile.TryGetSettings(out chrome);
        volume.profile.TryGetSettings(out grain);
        volume.profile.TryGetSettings(out color);

        bloom.intensity.value = 0;
        chrome.intensity.value = 0;
        grain.intensity.value = 0;
    }

    private void Update()
    {
        float fuzz = 0;
        foreach (Feature feature in features)
        {
            fuzz += feature.Correctness;
        }
        fuzz += TheStory.Correctness;
        fuzz /= (float)(features.Length + TheStory.Weight);
        fuzz = 1f - fuzz;

        bloom.intensity.value = fuzz * 15;
        chrome.intensity.value = fuzz;
        grain.intensity.value = fuzz;
        color.saturation.value = -85 * fuzz;
        color.contrast.value = 85 * fuzz;
    }
}