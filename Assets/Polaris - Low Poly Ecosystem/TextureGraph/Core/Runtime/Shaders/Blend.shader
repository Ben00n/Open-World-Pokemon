Shader "Hidden/TextureGraph/Blend"
{
    Properties
    {
        _Background("Background", 2D) = "black" {}
        _Foreground("Foreground", 2D) = "black" {}
        _Mask("Mask", 2D) = "white" {}
        _Opacity("Opacity", Float) = 1
        _AlphaMode("Alpha Mode", Int) = 0
    }

    CGINCLUDE
        #pragma vertex vert
        #pragma fragment frag
        #pragma shader_feature_local BG_RRR
        #pragma shader_feature_local FG_RRR
    ENDCG

    SubShader
    {
        Pass
        {
            Name "Normal"
            CGPROGRAM
            #define BLEND(bg,fg) blendNormal(bg,fg)
            #include "./CGIncludes/BlendCommon.cginc"
            ENDCG
        }
        Pass
        {
            Name "Add"
            CGPROGRAM
            #define BLEND(bg,fg) blendAdd(bg,fg)
            #include "./CGIncludes/BlendCommon.cginc"
            ENDCG
        }
        Pass
        {
            Name "Sub"
            CGPROGRAM
            #define BLEND(bg,fg) blendSub(bg,fg)
            #include "./CGIncludes/BlendCommon.cginc"
            ENDCG
        }
        Pass
        {
            Name "Mul"
            CGPROGRAM
            #define BLEND(bg,fg) blendMul(bg,fg)
            #include "./CGIncludes/BlendCommon.cginc"
            ENDCG
        }
        Pass
        {
            Name "Div"
            CGPROGRAM
            #define BLEND(bg,fg) blendDiv(bg,fg)
            #include "./CGIncludes/BlendCommon.cginc"
            ENDCG
        }
        Pass
        {
            Name "Screen"
            CGPROGRAM
            #define BLEND(bg,fg) blendScreen(bg,fg)
            #include "./CGIncludes/BlendCommon.cginc"
            ENDCG
        }
        Pass
        {
            Name "Overlay"
            CGPROGRAM
            #define BLEND(bg,fg) blendOverlay(bg,fg)
            #include "./CGIncludes/BlendCommon.cginc"
            ENDCG
        }
        Pass
        {
            Name "Hard Light"
            CGPROGRAM
            #define BLEND(bg,fg) blendHardLight(bg,fg)
            #include "./CGIncludes/BlendCommon.cginc"
            ENDCG
        }
        Pass
        {
            Name "Soft Light"
            CGPROGRAM
            #define BLEND(bg,fg) blendSoftLight(bg,fg)
            #include "./CGIncludes/BlendCommon.cginc"
            ENDCG
        }
        Pass
        {
            Name "Max"
            CGPROGRAM
            #define BLEND(bg,fg) blendMax(bg,fg)
            #include "./CGIncludes/BlendCommon.cginc"
            ENDCG
        }
        Pass
        {
            Name "Min"
            CGPROGRAM
            #define BLEND(bg,fg) blendMin(bg,fg)
            #include "./CGIncludes/BlendCommon.cginc"
            ENDCG
        }
    }
}
