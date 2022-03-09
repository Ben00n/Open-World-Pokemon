Shader "Hidden/TextureGraph/Splatter"
{
	Properties
	{
		_ShapeTex("Shape Tex", 2D) = "white"{}
		_ShapeAlpha("Shape Alpha", 2D) = "white"{}
		_HueShift("Hue Shift", Float) = 0
		_SaturationShift("Saturation Shift", Float) = 0
		_LightnessShift("Lightness Shift", Float) = 0
	}
	SubShader
	{
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature_local RRR
			#include "./CGIncludes/ColorConversion.cginc"

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

			sampler2D _ShapeTex;
			sampler2D _ShapeAlpha;
			float _HueShift;
			float _SaturationShift;
			float _LightnessShift;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_ShapeTex, i.uv); 
#if RRR
				color = float4(color.rrr, 1);
#endif
				float alphaMask = tex2D(_ShapeAlpha, i.uv);
				color.a *= alphaMask;

				float3 hsl = RGBtoHSL(saturate(color.rgb));
				hsl.x = frac(hsl.x + _HueShift);
				hsl.y = saturate(hsl.y + _SaturationShift);
				hsl.z = saturate(hsl.z + _LightnessShift);
				color.rgb = HSLtoRGB(hsl);

				return saturate(color);
			}
			ENDCG
		}
	}
}
