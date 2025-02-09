#if !defined(NOUNI_FRAG_BRDF_INCLUDED)
#define NOUNI_FRAG_BRDF_INCLUDED

#include "NouniCommonFunctions.cginc"
#include "NouniLightModels.cginc"

//---------------------------------------

    // FUNCTIONS
    
    // PER FRAGMENT
    half4 FragmentProgram(Interpolators i) : SV_TARGET
    {
        // Get cutout fragments out of the way.
        half alpha = GetAlpha(i);
        #if defined(_ALPHATEST_ON)
            clip(alpha - _AlphaCutoff);
        #endif
        

        // General preparations.
        InitializeFragmentNormal(i);
        float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);


        // Prepare albedo and specular.
        half3 albedo;
        half3 specularTint;
        half oneMinusReflectivity;
        #if defined(_SPECULAR_SETUP)
            specularTint = GetSpecular(i);
            albedo = EnergyConservationBetweenDiffuseAndSpecular(GetAlbedo(i), specularTint, oneMinusReflectivity);
        #else
            albedo = DiffuseAndSpecularFromMetallic(GetAlbedo(i), GetMetallic(i), specularTint, oneMinusReflectivity);
        #endif


        // Alpha premultiplication.
        #if defined(_ALPHAPREMULTIPLY_ON)
            albedo *= alpha;
            alpha = 1 - oneMinusReflectivity + alpha * oneMinusReflectivity;
        #endif


        // Call the main lighting function.
        half4 colour;
        colour.rgb = LocalUnityBRDF(albedo, specularTint, oneMinusReflectivity, GetGlossiness(i), i.normal, viewDir, CreateLight(i), CreateIndirectLight(i, viewDir));
        colour.rgb += GetEmission(i);
    

        // Do final alpha.
        #if defined(_ALPHABLEND_ON) || defined(_ALPHAPREMULTIPLY_ON)
            colour.a = alpha;
        #endif

        return colour;
    }

#endif