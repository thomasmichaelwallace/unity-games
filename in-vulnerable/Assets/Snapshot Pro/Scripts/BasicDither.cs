﻿using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(BasicDitherRenderer), PostProcessEvent.AfterStack, "Snapshot Pro/BasicDither")]
public sealed class BasicDither : PostProcessEffectSettings
{
    [Tooltip("Noise texture to use for dither thresholding.")]
    public TextureParameter noiseTex = new TextureParameter();

    [Range(0.1f, 100.0f), Tooltip("Size of the noise texture.")]
    public FloatParameter noiseSize = new FloatParameter { value = 1.0f };

    [Tooltip("Color to use for dark sections of the image.")]
    public ColorParameter darkColor = new ColorParameter { value = Color.black };

    [Tooltip("Color to use for light sections of the image.")]
    public ColorParameter lightColor = new ColorParameter { value = Color.white };
}

public sealed class BasicDitherRenderer : PostProcessEffectRenderer<BasicDither>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/SnapshotPro/BasicDither"));

        if(settings.noiseTex != null)
        {
            sheet.properties.SetTexture("_NoiseTex", settings.noiseTex);
        }
        else
        {
            sheet.properties.SetTexture("_NoiseTex", Texture2D.whiteTexture);
        }

        sheet.properties.SetFloat("_NoiseSize", settings.noiseSize);
        sheet.properties.SetColor("_DarkColor", settings.darkColor);
        sheet.properties.SetColor("_LightColor", settings.lightColor);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
