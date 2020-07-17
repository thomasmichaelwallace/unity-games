Shader "Hidden/SnapshotPro/Anaglyph3D"
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

			TEXTURE2D_SAMPLER2D(_LTex, sampler_LTex);
			TEXTURE2D_SAMPLER2D(_RTex, sampler_RTex);

			float4 Frag(VaryingsDefault i) : SV_Target
			{
				float4 col;
				float4 lCol = SAMPLE_TEXTURE2D(_LTex, sampler_LTex, i.texcoord);
				float4 rCol = SAMPLE_TEXTURE2D(_RTex, sampler_RTex, i.texcoord);

				col.r = rCol.r;
				col.gb = lCol.gb;
				col.a = 1.0f;

				return col;
			}

			ENDHLSL
		}
	}
}
