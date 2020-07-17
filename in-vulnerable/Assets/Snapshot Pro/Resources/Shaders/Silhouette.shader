Shader "Hidden/SnapshotPro/Silhouette"
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
			#include "HLSLSupport.cginc"

			TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);

			float4 _NearColor;
			float4 _FarColor;

			float4 Frag(VaryingsDefault i) : SV_Target
			{
				float4 tex = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord);
				float depth = UNITY_SAMPLE_DEPTH(tex);
				depth = pow(Linear01Depth(depth), 0.75);

				return lerp(_NearColor, _FarColor, depth);
			}

			ENDHLSL
		}
	}
}
