#if !defined(NOUNI_LIGHT_LIBRARY_INCLUDED)
#define NOUNI_LIGHT_LIBRARY_INCLUDED

#include "UnityStandardBRDF.cginc"

//---------------------------------------

// STRUCTS
struct ExtendedUnityLight
{
    half3 dir;
    
    fixed shadow;
    fixed attenuation;
    fixed combinedShadowAtten;

    half3 color;
    half intensity;
    half3 colouredShadowAtten;
};


//---------------------------------------

// MISC UTILITIES

float SimpleFresnel(float NdotV, float fresnelScale, float fresnelPower)
{
    return fresnelScale * pow(1 - NdotV, fresnelPower);
}

float SimpleFresnel(float NdotV, float fresnelBias, float fresnelScale, float fresnelPower)
{
    return fresnelBias + fresnelScale * pow(1 - NdotV, fresnelPower);
}


float StepAntialiasing(float A, float B)
{
    float BminusA = B - A;
    return saturate(BminusA / fwidth(BminusA));
}


//---------------------------------------

// SHADOW COLOURATION

// INPUTS
half _ShadowColourFraction;
half3 _ShadowColour;

// METHODS
half3 ProduceColouredShadows(ExtendedUnityLight light, half shadowColourFraction)
{
    // Coloured shadows.
    half3 colouredLightAtten = lerp(light.color * shadowColourFraction, light.color, light.shadow);
    #if !defined(FORWARD_BASE_PASS)
        colouredLightAtten *= light.attenuation;
    #endif  

    return colouredLightAtten;
}
half3 ProduceColouredShadows(ExtendedUnityLight light, half3 shadowColour)
{
    // Coloured shadows.
    half3 colouredLightAtten = lerp(shadowColour, light.color, light.shadow);
    #if !defined(FORWARD_BASE_PASS)
        colouredLightAtten *= light.attenuation;
    #endif  

    return colouredLightAtten;
}


//---------------------------------------

// LIGHT MODELS

// SPECULAR MODELS
float LightDirSpecular(float NdotL, float specularGloss, float specularPower)
{
    // Light calculations.
    float specular = pow(NdotL, specularPower) * specularGloss;
    //specular *= (2 + specularGloss) / (2 * UNITY_PI);

    return specular;
}

float BlinnPhongSpecular(float3 NdotH, float specularGloss, float specularPower)
{
    // Light calculations.
    float specular = pow(NdotH, specularPower) * specularGloss;
    specular *= (2 + specularGloss) / (2 * UNITY_PI);

    return specular;
}


// DISTRIBUTION FUNCTIONS (D)
float BeckmanNDF(float3 NdotH, float roughness)
{
    // Light calculations.
    float NdotHSquared = NdotH * NdotH;
    float roughnessSquared = roughness * roughness;
    return max(0.000001,(1.0 / (UNITY_PI * roughnessSquared * NdotHSquared * NdotHSquared)) * exp((NdotHSquared - 1)/(roughnessSquared * NdotHSquared)));
}


// VISIBILITY TERMS (Vis)
float CookTorranceVis(float NdotV, float NdotL, float NdotH, float VdotH)
{
    half G = min(1.0, min(2.0 * NdotH * NdotV / VdotH, 2.0 * NdotH * NdotL / VdotH));
    return G;
}


// FRESNEL TERMS
float SchlickFresnelReflectance(float3 H, float3 V, float F0)
{
    float base = 1.0 - saturate(dot(V, H));
    float exponential = Pow5(base);
    return exponential + F0 * (1.0 - exponential);
}



// OTHER EFFECTS
float3 SubsurfaceScattering(float3 normalDir, float3 viewDir, float3 lightDir, float subsurfaceDistortion, float scatterPower, float scatterScale)
{
    // Light calculations.
    float3 distordedLightDir = normalDir * subsurfaceDistortion + lightDir;
    float LdotV = saturate(dot(viewDir, -distordedLightDir));
    return pow(LdotV, scatterPower) * scatterScale;
}

float3 SubsurfaceScatteringWithFrontLight(float3 normalDir, float3 viewDir, float3 lightDir, float subsurfaceDistortionFront, float subsurfaceDistortionBack, float scatterPower, float scatterScale)
{
    // Light calculations.
    float3 distordedBackLightDir = normalDir * subsurfaceDistortionBack + lightDir;
    float LdotVBack = saturate(dot(viewDir, -distordedBackLightDir));

    float3 distordedFrontLightDir = normalDir * subsurfaceDistortionFront - lightDir;
    float LdotVFront = saturate(dot(viewDir, -distordedFrontLightDir));
    
    return pow((LdotVBack + LdotVFront), scatterPower) * scatterScale;
}





#endif