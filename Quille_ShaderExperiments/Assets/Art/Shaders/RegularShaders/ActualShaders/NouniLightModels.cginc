#if !defined(NOUNI_LIGHT_MODELS_INCLUDED)
#define NOUNI_LIGHT_MODELS_INCLUDED

#include "NouniLightLibrary.cginc"

//---------------------------------------

// Based on Unity's BRDF1_Unity_PBS function, used in the standard shader.
half3 LocalUnityBRDF(half3 diffuseAlbedo, half3 specularTint, half oneMinusReflectivity, half smoothness, float3 normalDir, float3 viewDir, ExtendedUnityLight light, UnityIndirect gi)
{
    // Precompute various values.
    float perceptualRoughness = SmoothnessToPerceptualRoughness(smoothness);
    float roughness = max(SmoothnessToRoughness(smoothness), 0.002);
    
    float3 halfDir = Unity_SafeNormalize(float3(light.dir) + viewDir);
    half NdotV = abs(dot(normalDir, viewDir));
    float NdotL = saturate(dot(normalDir, light.dir));
    float NdotH = saturate(dot(normalDir, halfDir));
    half LdotV = saturate(dot(light.dir, viewDir));
    half LdotH = saturate(dot(light.dir, halfDir));
    
  
    // Diffuse term.
    half diffuseTerm = DisneyDiffuse(NdotV, NdotL, LdotH, perceptualRoughness) * NdotL;

    
    // Specular term.
    float specularTerm;
    #if defined(_SPECULARHIGHLIGHTS_OFF)
        specularTerm = 0.0;
    #else
        float V = SmithJointGGXVisibilityTerm(NdotL, NdotV, roughness); // Rimlight and attenuation. By itself, a nice velvety effect.
        float D = GGXTerm(NdotH, roughness); // Highlight.
        specularTerm = V * D * UNITY_PI;

        #ifdef UNITY_COLORSPACE_GAMMA
            specularTerm = sqrt(max(1e-4h, specularTerm));
        #endif

        specularTerm = max(0, specularTerm * NdotL); // Ensure the term always has a value.
        specularTerm *= any(specularTint) ? 1.0 : 0.0; // Kill the term for true Lambert.
    #endif


    // Reflectiveness.
    float grazingTerm = saturate(smoothness + (1 - oneMinusReflectivity));
    float surfaceReduction; // Often seems to return values of one or above.
    #ifdef UNITY_COLORSPACE_GAMMA
        surfaceReduction = 1.0 - 0.28 * roughness * perceptualRoughness;
    #else
        surfaceReduction = 1.0 / (roughness * roughness + 1.0);
    #endif
    

    // Coloured shadows.
    half3 colouredLightAtten;
    #if defined(_SHADOWS_USE_COLOUR)
        colouredLightAtten = ProduceColouredShadows(light, _ShadowColour);
    #elif defined(_SHADOWS_USE_LIGHT_COLOUR)
        colouredLightAtten = ProduceColouredShadows(light, _ShadowColourFraction);
    #else
        colouredLightAtten = light.colouredShadowAtten;
    #endif


    // Assemble the results.
    half3 colouredDiffuse = diffuseAlbedo * (diffuseTerm * colouredLightAtten + gi.diffuse);
    half3 colouredSpecular = specularTerm * FresnelTerm(specularTint, LdotH) * colouredLightAtten;
    half3 reflectedSpecular = surfaceReduction * FresnelLerp(specularTint, grazingTerm, NdotV) * gi.specular;
    half3 resultingColour = colouredDiffuse + colouredSpecular + reflectedSpecular;


    return resultingColour;
}

#endif