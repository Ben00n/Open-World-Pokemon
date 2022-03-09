Shader "Hidden/TextureGraph/Levels"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
		_InLuminance("In Luminance", Vector) = (0, 0.5, 1, 0)
		_OutLuminance("Out Luminance", Vector) = (0, 1, 0, 0)

		_InRed("In Red", Vector) = (0, 0.5, 1, 0)
		_OutRed("Out Red", Vector) = (0, 1, 0, 0)

		_InGreen("In Green", Vector) = (0, 0.5, 1, 0)
		_OutGreen("Out Green", Vector) = (0, 1, 0, 0)

		_InBlue("In Blue", Vector) = (0, 0.5, 1, 0)
		_OutBlue("Out Blue", Vector) = (0, 1, 0, 0)

		_InAlpha("In Alpha", Vector) = (0, 0.5, 1, 0)
		_OutAlpha("Out Alpha", Vector) = (0, 1, 0, 0)
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature_local RRR
			#include "./CGIncludes/MathCommon.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _InLuminance;
			float4 _OutLuminance;
			float4 _InRed;
			float4 _OutRed;
			float4 _InGreen;
			float4 _OutGreen;
			float4 _InBlue;
			float4 _OutBlue;
			float4 _InAlpha;
			float4 _OutAlpha;

			void ApplyLevels(inout float v, float4 inLevels, float4 outLevels)
			{
				float inLow = inLevels.x;
				float inMid = inLevels.y;
				float inHigh = inLevels.z;

				if (v <= inMid)
				{
					v = InverseLerpUnclamped(inLow, 2 * inMid - inLow, v);
				}
				else
				{
					v = InverseLerpUnclamped(2 * inMid - inHigh, inHigh, v);
				}

				float outLow = outLevels.x;
				float outHigh = outLevels.y;
				v = lerp(outLow, outHigh, saturate(v));
			}

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
#if RRR
				color = float4(color.rrr, 1);
#endif
				ApplyLevels(color.r, _InRed, _OutRed);
				ApplyLevels(color.g, _InGreen, _OutGreen);
				ApplyLevels(color.b, _InBlue, _OutBlue);
				ApplyLevels(color.a, _InAlpha, _OutAlpha);

				ApplyLevels(color.r, _InLuminance, _OutLuminance);
				ApplyLevels(color.g, _InLuminance, _OutLuminance);
				ApplyLevels(color.b, _InLuminance, _OutLuminance);

				return color;
			}
			ENDCG
		}
	}
}
