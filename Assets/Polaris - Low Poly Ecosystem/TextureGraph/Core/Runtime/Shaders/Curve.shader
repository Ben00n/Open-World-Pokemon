Shader "Hidden/TextureGraph/Curve"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "black" {}
        _CurveRGB ("Curve RGB", 2D) = "black" {}
        _CurveR("Curve R", 2D) = "black" {}
        _CurveG("Curve G", 2D) = "black" {}
        _CurveB("Curve B", 2D) = "black" {}
        _CurveA("Curve A", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local RRR

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
            sampler2D _CurveRGB;
            sampler2D _CurveR;
            sampler2D _CurveG;
            sampler2D _CurveB;
            sampler2D _CurveA;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 c = tex2D(_MainTex, i.uv);
#if RRR
                c = float4(c.rrr, 1);
#endif
                c.r = tex2D(_CurveR, c.rr);
                c.g = tex2D(_CurveG, c.gg);
                c.b = tex2D(_CurveB, c.bb);
                c.a = tex2D(_CurveA, c.aa);

                c.r = tex2D(_CurveRGB, c.rr);
                c.g = tex2D(_CurveRGB, c.gg);
                c.b = tex2D(_CurveRGB, c.bb);

                return c;
            }
            ENDCG
        }
    }
}
