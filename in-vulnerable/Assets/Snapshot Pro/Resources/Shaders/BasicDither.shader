Shader "Hidden/SnapshotPro/BasicDither"
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
			TEXTURE2D_SAMPLER2D(_NoiseTex, sampler_NoiseTex);
			float4 _NoiseTex_TexelSize;
			float _NoiseSize;

			float4 _LightColor;
			float4 _DarkColor;

			float4 Frag(VaryingsDefault i) : SV_Target
			{
				float3 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
				float lum = dot(col, float3(0.3f, 0.59f, 0.11f));

				float2 noiseUV = i.texcoord * _NoiseTex_TexelSize.xy * _ScreenParams.xy / _NoiseSize;
				float3 noiseColor = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV);
				float threshold = dot(noiseColor, float3(0.3f, 0.59f, 0.11f));

				col = lum < threshold ? _DarkColor : _LightColor;

				return float4(col, 1.0f);
			}

			ENDHLSL
		}
	}
}
