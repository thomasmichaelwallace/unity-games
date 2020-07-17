using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(Anaglyph3DRenderer), PostProcessEvent.AfterStack, "Snapshot Pro/Anaglyph 3D")]
public sealed class Anaglyph3D : PostProcessEffectSettings
{
    [Range(0f, 0.1f), Tooltip("3D effect intensity.")]
    public FloatParameter strength = new FloatParameter { value = 0.01f };

    [Range(0f, 250.0f), Tooltip("Focal distance")]
    public FloatParameter distance = new FloatParameter { value = 50.0f };
}

public sealed class Anaglyph3DRenderer : PostProcessEffectRenderer<Anaglyph3D>
{
    private Camera camera = null;

    public override void Render(PostProcessRenderContext context)
    {
        var mainCam = Camera.main;
        if(camera == null)
        {
            camera = new GameObject("Anaglyph3D camera.").AddComponent<Camera>();
            camera.transform.parent = mainCam.transform;
            camera.gameObject.SetActive(false);
        }

        camera.CopyFrom(mainCam);

        RenderTexture lTex = RenderTexture.GetTemporary(Screen.width, Screen.height);
        RenderTexture rTex = RenderTexture.GetTemporary(Screen.width, Screen.height);

        camera.transform.localPosition = Vector3.zero;
        camera.transform.localPosition += new Vector3(settings.distance, 0.0f, 0.0f);
        camera.transform.LookAt(mainCam.transform.position + mainCam.transform.forward * settings.distance);
        camera.targetTexture = rTex;
        camera.Render();

        camera.transform.localPosition = Vector3.zero;
        camera.transform.localPosition -= new Vector3(settings.distance, 0.0f, 0.0f);
        camera.transform.LookAt(mainCam.transform.position + mainCam.transform.forward * settings.distance);
        camera.targetTexture = lTex;
        camera.Render();

        var sheet = context.propertySheets.Get(Shader.Find("Hidden/SnapshotPro/Anaglyph3D"));
        sheet.properties.SetTexture("_LTex", lTex);
        sheet.properties.SetTexture("_RTex", rTex);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
