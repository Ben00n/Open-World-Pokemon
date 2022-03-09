#ifndef BUILTIN_RP_SURFACE_FUNCTION_INCLUDED
	#define BUILTIN_RP_SURFACE_FUNCTION_INCLUDED

	struct SurfaceInput
	{
		float3 positionWS;
		float4 positionSS;
		float3 normalWS;
		float crestMask;
	};

	#if defined(POSEIDON_WATER_ADVANCED)
		#if defined(POSEIDON_BACK_FACE)
			#define SURFACE_FUNCTION(i, o) surfBackFace(i, o);
		#else
			#define SURFACE_FUNCTION(i, o) surfAdvanced(i, o);
		#endif
	#elif defined(POSEIDON_RIVER)
		#define SURFACE_FUNCTION(i, o) surfRiver(i, o);
	#else
		#define SURFACE_FUNCTION(i, o) surfBasic(i, o);
	#endif

	#ifndef POSEIDON_WATER_ADVANCED
		#if LIGHTING_LAMBERT || LIGHTING_BLINN_PHONG
			void surfBasic(SurfaceInput i, inout SurfaceOutput o)
		#else
			void surfBasic(SurfaceInput i, inout SurfaceOutputStandardSpecular o)
		#endif
		{
			float fresnel;
			CalculateFresnelFactor(i.positionWS, i.normalWS, fresnel);

			float4 waterColor = _Color;
			float4 tintColor = _Color;

			#if LIGHT_ABSORPTION || FOAM
				float sceneDepth = GetSceneDepth(i.positionSS);
				float surfaceDepth = GetSurfaceDepth(float4(i.positionWS, 1));
			#endif

			#if LIGHT_ABSORPTION
				CalculateDeepWaterColor(sceneDepth, surfaceDepth, tintColor);
			#endif
			waterColor = lerp(waterColor, tintColor, fresnel);

			half4 foamColor = half4(0, 0, 0, 0);
			#if FOAM
				#if FOAM_HQ
					CalculateFoamColorHQ(sceneDepth, surfaceDepth, float4(i.positionWS, 1), i.normalWS, i.crestMask, foamColor);
				#else
					CalculateFoamColor(sceneDepth, surfaceDepth, float4(i.positionWS, 1), i.normalWS, i.crestMask, foamColor);
				#endif
			#endif

			waterColor = saturate(waterColor);
			o.Albedo = lerp(waterColor.rgb, foamColor.rgb, foamColor.a);
			o.Alpha = lerp(waterColor.a, foamColor.a, foamColor.a);

			#if LIGHTING_LAMBERT || LIGHTING_BLINN_PHONG
				o.Specular = saturate(_Smoothness - foamColor.a);
			#else
				o.Specular = _Specular;
				o.Smoothness = saturate(_Smoothness - foamColor.a);
			#endif
		}
	#endif //!POSEIDON_WATER_ADVANCED

	#if defined(POSEIDON_WATER_ADVANCED)
		#if LIGHTING_LAMBERT || LIGHTING_BLINN_PHONG
			void surfAdvanced(SurfaceInput i, inout SurfaceOutput o)
		#else
			void surfAdvanced(SurfaceInput i, inout SurfaceOutputStandardSpecular o)
		#endif
		{
			#if LIGHT_ABSORPTION || CAUSTIC || FOAM
				float sceneDepth = GetSceneDepth(i.positionSS);
				float surfaceDepth = GetSurfaceDepth(float4(i.positionWS, 1));
				#if LIGHT_ABSORPTION
					float depthFade = GetDepthFade(sceneDepth, surfaceDepth, _MaxDepth);
				#endif
			#endif

			float4 waterColor;
			float4 tintColor = _Color;
			#if LIGHT_ABSORPTION
				CalculateDeepWaterColor(sceneDepth, surfaceDepth, tintColor);
			#endif

			float fresnel;
			CalculateFresnelFactor(i.positionWS, i.normalWS, fresnel);

			float4 reflColor = _Color;
			#if REFLECTION && !UNITY_SINGLE_PASS_STEREO && !STEREO_INSTANCING_ON && !UNITY_STEREO_MULTIVIEW_ENABLED
				SampleReflectionTexture(i.positionSS, i.normalWS, reflColor);
			#endif

			float4 refrColor = _DepthColor;
			#if REFRACTION
				SampleRefractionTexture(i.positionSS, i.normalWS, refrColor);
				#if LIGHT_ABSORPTION
					refrColor = lerp(_DepthColor, refrColor, depthFade);
				#endif
			#endif

			half4 causticColor = half4(0, 0, 0, 0);
			#if CAUSTIC
				SampleCausticTexture(sceneDepth, surfaceDepth, float4(i.positionWS, 1), i.normalWS, causticColor);
				#if LIGHT_ABSORPTION
					causticColor *= depthFade;
				#endif
			#endif
			refrColor += causticColor;

			waterColor = tintColor * lerp(refrColor, reflColor, fresnel);
			waterColor = waterColor * tintColor.a + (1 - tintColor.a) * refrColor;
			waterColor = saturate(waterColor);
			half4 foamColor = half4(0, 0, 0, 0);
			#if FOAM
				#if FOAM_HQ
					CalculateFoamColorHQ(sceneDepth, surfaceDepth, float4(i.positionWS, 1), i.normalWS, i.crestMask, foamColor);
				#else
					CalculateFoamColor(sceneDepth, surfaceDepth, float4(i.positionWS, 1), i.normalWS, i.crestMask, foamColor);
				#endif
			#endif

			o.Albedo = lerp(waterColor.rgb, foamColor.rgb, foamColor.a);
			o.Alpha = lerp(tintColor.a, foamColor.a, foamColor.a);
			#if LIGHTING_LAMBERT || LIGHTING_BLINN_PHONG
				o.Specular = saturate(_Smoothness - foamColor.a);
			#else
				o.Specular = _Specular;
				o.Smoothness = saturate(_Smoothness - foamColor.a);
			#endif
		}

		#if LIGHTING_LAMBERT || LIGHTING_BLINN_PHONG
			void surfBackFace(SurfaceInput i, inout SurfaceOutput o)
		#else
			void surfBackFace(SurfaceInput i, inout SurfaceOutputStandardSpecular o)
		#endif
		{
			float sceneDepth = GetSceneDepth(i.positionSS);
			float surfaceDepth = GetSurfaceDepth(float4(i.positionWS, 1));

			float4 waterColor;
			float4 tintColor = _Color;

			float fresnel;
			CalculateFresnelFactor(i.positionWS, -i.normalWS, fresnel);

			float4 reflColor = _Color;
			float4 refrColor = _Color;
			SampleRefractionTexture(i.positionSS, i.normalWS, refrColor);

			waterColor = tintColor * lerp(refrColor, reflColor, fresnel);
			waterColor = waterColor * tintColor.a + (1 - tintColor.a) * refrColor;
			waterColor = saturate(waterColor);
			half4 foamColor = half4(0, 0, 0, 0);
			#if FOAM
				#if FOAM_HQ
					CalculateFoamColorHQ(sceneDepth, surfaceDepth, float4(i.positionWS, 1), i.normalWS, i.crestMask, foamColor);
				#else
					CalculateFoamColor(sceneDepth, surfaceDepth, float4(i.positionWS, 1), i.normalWS, i.crestMask, foamColor);
				#endif
				foamColor.a *= 0.5;
			#endif

			o.Albedo = lerp(waterColor.rgb, foamColor.rgb, foamColor.a);
			o.Alpha = lerp(tintColor.a, foamColor.a, foamColor.a);
			#if LIGHTING_LAMBERT || LIGHTING_BLINN_PHONG
				o.Specular = saturate(_Smoothness - foamColor.a);
			#else
				o.Specular = _Specular;
				o.Smoothness = saturate(_Smoothness - foamColor.a);
			#endif
		}
	#endif //POSEIDON_WATER_ADVANCED

	#if defined(POSEIDON_RIVER)
		#if LIGHTING_LAMBERT || LIGHTING_BLINN_PHONG
			void surfRiver(SurfaceInput i, inout SurfaceOutput o)
		#else
			void surfRiver(SurfaceInput i, inout SurfaceOutputStandardSpecular o)
		#endif
		{
			float sceneDepth = GetSceneDepth(i.positionSS);
			float surfaceDepth = GetSurfaceDepth(float4(i.positionWS, 1));
			#if LIGHT_ABSORPTION
				float depthFade = GetDepthFade(sceneDepth, surfaceDepth, _MaxDepth);
			#endif

			float4 waterColor;
			float4 tintColor = _Color;
			#if LIGHT_ABSORPTION
				CalculateDeepWaterColor(sceneDepth, surfaceDepth, tintColor);
			#endif

			float fresnel;
			CalculateFresnelFactor(i.positionWS, i.normalWS, fresnel);

			float4 refrColor = _DepthColor;
			SampleRefractionTexture(i.positionSS, i.normalWS, refrColor);
			#if LIGHT_ABSORPTION
				refrColor = lerp(_DepthColor, refrColor, depthFade);
			#endif

			half4 causticColor = half4(0, 0, 0, 0);
			#if CAUSTIC
				SampleCausticTexture(sceneDepth, surfaceDepth, float4(i.positionWS, 1), i.normalWS, causticColor);
				#if LIGHT_ABSORPTION
					causticColor *= depthFade;
				#endif
			#endif
			refrColor += causticColor;

			waterColor = lerp(refrColor, tintColor, tintColor.a * fresnel);

			half4 foamColor = half4(0, 0, 0, 0);
			#if FOAM
				#if FOAM_HQ
					CalculateFoamColorHQ(sceneDepth, surfaceDepth, float4(i.positionWS, 1), i.normalWS, i.crestMask, foamColor);
				#else
					CalculateFoamColor(sceneDepth, surfaceDepth, float4(i.positionWS, 1), i.normalWS, i.crestMask, foamColor);
				#endif
			#endif

			o.Albedo = lerp(waterColor.rgb, foamColor.rgb, foamColor.a);
			o.Alpha = 1;

			#if LIGHTING_LAMBERT || LIGHTING_BLINN_PHONG
				o.Specular = saturate(_Smoothness - foamColor.a);
			#else
				o.Specular = _Specular;
				o.Smoothness = saturate(_Smoothness - foamColor.a);
			#endif
		}
	#endif //POSEIDON_RIVER
#endif //BUILTIN_RP_SURFACE_FUNCTION_INCLUDED
