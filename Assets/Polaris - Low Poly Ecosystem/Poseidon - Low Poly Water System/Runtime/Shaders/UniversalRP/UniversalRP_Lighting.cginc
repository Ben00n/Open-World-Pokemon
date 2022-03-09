#ifndef LIGHTING_SRP_INCLUDED
#define LIGHTING_SRP_INCLUDED

half4 PoseidonFragmentPBR(InputData inputData, half3 albedo, half3 specular,
	half smoothness, half alpha)
{
	BRDFData brdfData;
	InitializeBRDFData(albedo, /*metallic*/0, specular, smoothness, alpha, brdfData);

	Light mainLight = GetMainLight(inputData.shadowCoord);
	half3 color = GlobalIllumination(brdfData, inputData.bakedGI, /*occlusion*/1, inputData.normalWS, inputData.viewDirectionWS);
	color += LightingPhysicallyBased(brdfData, mainLight, inputData.normalWS, inputData.viewDirectionWS);

#ifdef _ADDITIONAL_LIGHTS
	int pixelLightCount = GetAdditionalLightsCount();
	for (int i = 0; i < pixelLightCount; ++i)
	{
		Light light = GetAdditionalLight(i, inputData.positionWS);
		color += LightingPhysicallyBased(brdfData, light, inputData.normalWS, inputData.viewDirectionWS);
	}
#endif

#ifdef _ADDITIONAL_LIGHTS_VERTEX
	color += inputData.vertexLighting * brdfData.diffuse;
#endif

	return half4(color, alpha);
}

half4 PoseidonFragmentBlinnPhong(InputData inputData, half3 diffuse, half4 specular, half smoothness, half alpha)
{
	smoothness *= 1000;

    Light mainLight = GetMainLight(inputData.shadowCoord);
    half3 attenuatedLightColor = mainLight.color * (mainLight.distanceAttenuation * mainLight.shadowAttenuation);
    half3 diffuseColor = inputData.bakedGI + LightingLambert(attenuatedLightColor, mainLight.direction, inputData.normalWS);
    half3 specularColor = LightingSpecular(attenuatedLightColor, mainLight.direction, inputData.normalWS, inputData.viewDirectionWS, specular, smoothness);

#ifdef _ADDITIONAL_LIGHTS
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, inputData.positionWS);
        half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
        diffuseColor += LightingLambert(attenuatedLightColor, light.direction, inputData.normalWS);
        specularColor += LightingSpecular(attenuatedLightColor, light.direction, inputData.normalWS, inputData.viewDirectionWS, specular, smoothness);
    }
#endif

#ifdef _ADDITIONAL_LIGHTS_VERTEX
    diffuseColor += inputData.vertexLighting;
#endif

    half3 finalColor = diffuseColor * diffuse;
    finalColor += specularColor;

    return half4(finalColor, alpha);
}

half4 PoseidonFragmentLambert(InputData inputData, half3 diffuse, half alpha)
{
    Light mainLight = GetMainLight(inputData.shadowCoord);
    half3 attenuatedLightColor = mainLight.color * (mainLight.distanceAttenuation * mainLight.shadowAttenuation);
    half3 diffuseColor = inputData.bakedGI + LightingLambert(attenuatedLightColor, mainLight.direction, inputData.normalWS);

#ifdef _ADDITIONAL_LIGHTS
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, inputData.positionWS);
        half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
        diffuseColor += LightingLambert(attenuatedLightColor, light.direction, inputData.normalWS);
    }
#endif

#ifdef _ADDITIONAL_LIGHTS_VERTEX
    diffuseColor += inputData.vertexLighting;
#endif

    half3 finalColor = diffuseColor * diffuse;

    return half4(finalColor, alpha);
}


#endif
