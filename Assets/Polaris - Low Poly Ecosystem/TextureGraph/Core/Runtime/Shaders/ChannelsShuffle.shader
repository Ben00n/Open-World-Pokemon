Shader "Hidden/TextureGraph/ChannelsShuffle"
{
    Properties
    {
        _Texture0 ("Texture 0", 2D) = "black" {}
        _Texture1 ("Texture 1", 2D) = "black" {}
        _ChannelSource ("Channel Source", Vector) = (0, 1, 2, 3)
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
            #pragma shader_feature_local RRR_0
            #pragma shader_feature_local RRR_1

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

            sampler2D _Texture0;
            sampler2D _Texture1;
            float4 _ChannelSource;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float shuffle(float4 c0, float4 c1, float src)
            {
                float4 c =
                    c0.r * (src == 0) +
                    c0.g * (src == 1) +
                    c0.b * (src == 2) +
                    c0.a * (src == 3) +
                    c1.r * (src == 4) +
                    c1.g * (src == 5) +
                    c1.b * (src == 6) +
                    c1.a * (src == 7);
                return c;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 c0 = tex2D(_Texture0, i.uv);
#if RRR_0
                c0 = float4(c0.rrr, 1);
#endif

                float4 c1 = tex2D(_Texture1, i.uv);
#if RRR_1
                c1 = float4(c1.rrr, 1);
#endif

                float4 color = float4(
                    shuffle(c0, c1, _ChannelSource.x),
                    shuffle(c0, c1, _ChannelSource.y),
                    shuffle(c0, c1, _ChannelSource.z),
                    shuffle(c0, c1, _ChannelSource.w));

                return color;
            }
            ENDCG
        }
    }
}
