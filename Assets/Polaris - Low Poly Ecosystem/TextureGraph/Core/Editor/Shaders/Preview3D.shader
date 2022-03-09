Shader "Hidden/TextureGraph/Preview3D"
{
    Properties
    {
		_TessellationLevel("Tessellation Level", Int) = 1
		_DisplacementStrength("Displacement Strength", Float) = 0

        _AlbedoMap ("Albedo", 2D) = "white" { }
		_HeightMap ("Height Map", 2D) = "black" {}
		_NormalMap ("Normal Map", 2D) = "bump" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            
            #pragma vertex TessVertexProgram
            #pragma fragment FragmentProgram
            #pragma hull HullProgram
            #pragma domain DomainProgram
            #pragma target 4.6            
            #include "UnityCG.cginc"

            uniform float4 _Light0_Color;
            uniform float _Light0_Intensity;
            uniform float4 _Light0_Direction;

			uniform float4 _Light1_Color;
            uniform float _Light1_Intensity;
			uniform float4 _Light1_Direction;

			uniform float _TessellationLevel;
			uniform float _DisplacementStrength;

            uniform sampler2D _AlbedoMap;
			uniform sampler2D _HeightMap;
			uniform sampler2D _NormalMap;
            
            struct appdata
            {
                float4 positionOS: POSITION;
                float2 uv: TEXCOORD0;
                float3 normalOS: NORMAL;
				float4 tangentOS: TANGENT;
            };
            
            struct v2f
            {
                float4 positionCS: SV_POSITION;
                float2 uv: TEXCOORD0;
                float3 normalWS: NORMAL;
				float3 tangentWS: TANGENT;
				float3 binormalWS : TEXCOORD1;
            };

			struct TessellationFactor
			{
				float edge[3]: SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			struct TessellationControlPoint
			{
				float4 positionOS: INTERNALTESSPOS;
				float2 uv: TEXCOORD0;
				float3 normalOS: NOMRAL;
				float4 tangentOS: TANGENT;
			};

			TessellationControlPoint TessVertexProgram(appdata v)
			{
				TessellationControlPoint o;
				o.positionOS = v.positionOS;
				o.uv = v.uv;
				o.normalOS = v.normalOS;;
				o.tangentOS = v.tangentOS;
				return o;
			}

			[UNITY_domain("tri")]
			[UNITY_outputcontrolpoints(3)]
			[UNITY_outputtopology("triangle_cw")]
			[UNITY_partitioning("integer")]
			[UNITY_patchconstantfunc("PatchConstantFunction")]
			TessellationControlPoint HullProgram(InputPatch<TessellationControlPoint, 3> patch, uint id: SV_OutputControlPointID)
			{
				return patch[id];
			}

			TessellationFactor PatchConstantFunction(InputPatch<TessellationControlPoint, 3> patch)
			{
				TessellationFactor f;
				f.edge[0] = _TessellationLevel;
				f.edge[1] = _TessellationLevel;
				f.edge[2] = _TessellationLevel;
				f.inside = _TessellationLevel;
				return f;
			}

			v2f VertexProgram(appdata v)
			{
				v2f o;
				v.positionOS.xyz += _DisplacementStrength * tex2Dlod(_HeightMap, float4(v.uv, 0, 1)).r * v.normalOS;

				o.positionCS = UnityObjectToClipPos(v.positionOS);
				o.uv = v.uv;
				o.normalWS = mul(unity_ObjectToWorld, v.normalOS.xyz);
				o.tangentWS = mul(unity_ObjectToWorld, v.tangentOS.xyz);
				o.binormalWS = normalize(cross(o.normalWS, o.tangentWS) * v.tangentOS.w);
				return o;
			}

			#define DOMAIN_INTEPOLATE(field) o.field=\
									patch[0].field * bary.x + \
									patch[1].field * bary.y + \
									patch[2].field * bary.z;

			[UNITY_domain("tri")]
			v2f DomainProgram(TessellationFactor f, OutputPatch<TessellationControlPoint, 3> patch, float3 bary: SV_DomainLocation)
			{
				appdata o;
				DOMAIN_INTEPOLATE(positionOS);
				DOMAIN_INTEPOLATE(uv);
				DOMAIN_INTEPOLATE(normalOS);
				DOMAIN_INTEPOLATE(tangentOS);

				return VertexProgram(o);
			}

			float3 CalculateDiffuseLighting(float3 normalWS, float3 lightDir, float3 lightColor, float lightIntensity)
			{
				float nDotL = dot(normalWS, -lightDir);
				float atten = max(0.15, nDotL);
				float3 light = atten*lightColor*lightIntensity;
				return light;
			}

			float4 FragmentProgram(v2f i) : SV_TARGET
			{				
				float4 nColor = tex2D(_NormalMap, i.uv);
				float3 tNormal = float3(2 * nColor.r - 1, 2 * nColor.g - 1, 0);
				tNormal.z = sqrt(1.0 - dot(tNormal, tNormal));

				float3x3 tangentToWorld = transpose(float3x3(i.tangentWS, i.binormalWS, i.normalWS));
				float3 normalWS = mul(tangentToWorld, tNormal);

				float3 diffuseLighting = CalculateDiffuseLighting(normalWS, _Light0_Direction, _Light0_Color, _Light0_Intensity)+CalculateDiffuseLighting(normalWS, _Light1_Direction, _Light1_Color, _Light1_Intensity);


				float4 albedo = tex2D(_AlbedoMap, i.uv);
                float3 color = albedo * diffuseLighting;

                //color = float3(1, 1, 1);
				return float4(color, 1);
			}

			ENDCG	
		}
	}
}
