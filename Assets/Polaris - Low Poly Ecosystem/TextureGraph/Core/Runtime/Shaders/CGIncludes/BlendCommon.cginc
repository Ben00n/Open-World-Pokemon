#ifndef BLEND_COMMON_INCLUDED
#define BLEND_COMMON_INCLUDED

#ifndef BLEND
#define BLEND(bg, fg) 0;    
#endif

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

sampler2D _Background;
sampler2D _Foreground;
sampler2D _Mask;
float _Opacity;
int _AlphaMode;

float4 blendNormal(float4 bg, float4 fg)
{
	return fg;
}

float4 blendAdd(float4 bg, float4 fg)
{
	return bg + fg * fg.a;
}

float4 blendSub(float4 bg, float4 fg)
{
	return bg - fg * fg.a;
}

float4 blendMul(float4 bg, float4 fg)
{
	return bg * (1 - fg.a) + bg * fg * fg.a;
}

float4 blendDiv(float4 bg, float4 fg)
{
	float4 div = bg / (fg+0.000001);
	return lerp(bg, div, fg.a);
}

float4 blendScreen(float4 bg, float4 fg)
{
	float4 f = 1 - (1 - bg) * (1 - fg);
	return lerp(bg, f, fg.a);
}

float4 blendOverlay(float4 bg, float4 fg)
{
	float4 f0 = 2 * bg * fg;
	float4 f1 = 1 - 2 * (1 - bg) * (1 - fg);
	float4 f = float4(
		f0.r * (bg.r > 0.5) + f1.r * (bg.r <= 0.5),
		f0.g * (bg.g > 0.5) + f1.g * (bg.g <= 0.5),
		f0.b * (bg.b > 0.5) + f1.b * (bg.b <= 0.5),
		f0.a * (bg.a > 0.5) + f1.a * (bg.a <= 0.5));
	return lerp(bg, f, fg.a);
}

float4 blendHardLight(float4 bg, float4 fg)
{
	float4 f0 = 2 * bg * fg;
	float4 f1 = 1 - 2 * (1 - bg) * (1 - fg);
	float4 f = float4(
		f0.r * (bg.r <= 0.5) + f1.r * (bg.r > 0.5),
		f0.g * (bg.g <= 0.5) + f1.g * (bg.g > 0.5),
		f0.b * (bg.b <= 0.5) + f1.b * (bg.b > 0.5),
		f0.a * (bg.a <= 0.5) + f1.a * (bg.a > 0.5));
	return lerp(bg, f, fg.a);
}

float4 blendSoftLight(float4 bg, float4 fg)
{
	float g0 = ((16 * bg - 14) * bg + 4) * bg;
	float g1 = sqrt(bg);
	float4 g = float4(
		g0 * (bg.r <= 0.25) + g1 * (bg.r > 0.25),
		g0 * (bg.g <= 0.25) + g1 * (bg.g > 0.25),
		g0 * (bg.b <= 0.25) + g1 * (bg.b > 0.25),
		g0 * (bg.a <= 0.25) + g1 * (bg.a > 0.25));

	float4 f0 = bg - (1 - 2 * fg) * bg * (1 - bg);
	float4 f1 = bg + (2 * fg - 1) * (g - bg);
	float4 f = float4(
		f0.r * (fg.r <= 0.5) + f1.r * (fg.r > 0.5),
		f0.g * (fg.g <= 0.5) + f1.g * (fg.g > 0.5),
		f0.b * (fg.b <= 0.5) + f1.b * (fg.b > 0.5),
		f0.a * (fg.a <= 0.5) + f1.a * (fg.a > 0.5));
	return lerp(bg, f, fg.a);
}

float4 blendMax(float4 bg, float4 fg)
{
	float4 f = max(bg, fg);
	return lerp(bg, f, fg.a);
}

float4 blendMin(float4 bg, float4 fg)
{
	float4 f = min(bg, fg);
	return lerp(bg, f, fg.a);
}

float blendAlpha(float bg, float fg)
{
	float a =
		1 * (_AlphaMode == 0) +
		bg * (_AlphaMode == 1) +
		fg * (_AlphaMode == 2) +
		(fg * fg + bg * (1 - fg)) * (_AlphaMode == 3) +
		(fg + bg * (1 - fg)) * (_AlphaMode == 4);
	return a;
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
	float4 bg = tex2D(_Background, i.uv);
#if BG_RRR
	bg = float4(bg.rrr, 1);
#endif

	float4 fg = tex2D(_Foreground, i.uv);
#if FG_RRR
	fg = float4(fg.rrr, 1);
#endif

	float4 blended = BLEND(bg, fg);

	float mask = tex2D(_Mask, i.uv).r;
	mask = mask * _Opacity;

	float4 result = lerp(bg, blended, mask);
	result.a = blendAlpha(bg.a, fg.a);

	return saturate(result);
}
#endif