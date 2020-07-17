Shader "Hidden/SnapshotPro/Cutout"
{
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag

			#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

			TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
			TEXTURE2D_SAMPLER2D(_CutoutTex, sampler_CutoutTex);
			float4 _BorderColor;
			int _Stretch;

			float4 Frag(VaryingsDefault i) : SV_Target
			{
				float aspect = _ScreenParams.x / _ScreenParams.y;
				float2 nonStretchedUVs = float2(aspect * (i.texcoord.x - 0.5f) + 0.5f, i.texcoord.y);

				float2 cutoutUVs = (_Stretch == 0) ? nonStretchedUVs : i.texcoord;
				float cutoutAlpha = SAMPLE_TEXTURE2D(_CutoutTex, sampler_CutoutTex, cutoutUVs).a;
				float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
				return lerp(col, _BorderColor, cutoutAlpha);
			}

			ENDHLSL
		}
	}
}
