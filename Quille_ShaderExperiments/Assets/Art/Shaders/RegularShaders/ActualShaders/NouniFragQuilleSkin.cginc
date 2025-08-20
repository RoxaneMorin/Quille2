#if !defined(NOUNI_FRAG_QUILLESKIN_INCLUDED)
#define NOUNI_FRAG_QUILLESKIN_INCLUDED

#include "NouniCommonFunctions.cginc"
#include "NouniLightLibrary.cginc"

//---------------------------------------

    // PARAMETERS
    sampler2D _DiffuseRamp;

    half _RimlightScale;
    half _RimlightPower;
    half3 _RimlightColour;

    half _SubsurfaceDistortionFront;
    half _SubsurfaceDistortionBack;
    half _ScatterPower;
    half _ScatterScale;
    half3 _SubsurfaceColour;
    half _SubsurfaceColourPower;


//---------------------------------------

    // FUNCTIONS

    // LIGHTING FUNCTION
    half3 Lighting_QuilleSkin(half3 albedoColour, half3 specularColour, half reflectivity, half oneMinusReflectivity, half smoothness, float3 normalDir, float3 viewDir, ExtendedUnityLight light, UnityIndirect gi)
    {
        // Precompute various values.
        float perceptualRoughness = SmoothnessToPerceptualRoughness(smoothness);
        float roughness = max(PerceptualRoughnessToRoughness(perceptualRoughness), 0.002);
    
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
            // Alternatives that didn't look as good:
            //float V = CookTorranceVis(NdotV, NdotL, NdotH, saturate(dot(viewDir, halfDir)));
            //float D = BlinnPhongSpecular(NdotH, smoothness, max(1.0, smoothness * 10)); // Highlight.

            float V = SmithJointGGXVisibilityTerm(NdotL, NdotV, roughness); // Geometric attenuation. By itself, a nice velvety effect.
            float D = BeckmanNDF(NdotH, perceptualRoughness); // Not sure whether to use roughness or perceptualRoughness.
            specularTerm = V * D * UNITY_PI; // The multiplication by Pi compensates for a divison not being done in the diffuse term.

            #ifdef UNITY_COLORSPACE_GAMMA
                specularTerm = sqrt(max(1e-4h, specularTerm));
            #endif

            specularTerm = max(0, specularTerm * NdotL); // Ensure the term always has a value.
            specularTerm *= any(specularColour) ? 1.0 : 0.0; // Kill the term for true Lambert.
        #endif


        // Fresnel term.
        float fresnelTerm = SchlickFresnelReflectance(halfDir, viewDir, 0.028);
        half3 colouredFresnelTerm = lerp(specularColour, 1.0, fresnelTerm);


        // Additional stylized highlight.
        float extraHighlight = saturate(LightDirSpecular(NdotL, max(0.01, smoothness), max(1, smoothness * 80)));

        // Additional stylized rimlight.
        float extraRimlight = saturate(SimpleFresnel(NdotV, _RimlightScale, _RimlightPower));

        // Subsurface scattering.
        float subsurfaceLight = SubsurfaceScatteringWithFrontLight(normalDir, viewDir, light.dir, _SubsurfaceDistortionFront, _SubsurfaceDistortionBack, _ScatterPower, _ScatterScale);
        // TODO: add thickness


        // TODO:
       // how to handle reflection probes?
       // colour via the environment?

        // Reflectiveness.
        //float grazingTerm = saturate(smoothness + (1 - oneMinusReflectivity)); // Dims the reflections on rough nonmetals.
        //float surfaceReduction; // Dims the reflections on rough metals.
        //#ifdef UNITY_COLORSPACE_GAMMA
        //    surfaceReduction = 1.0 - 0.28 * roughness * perceptualRoughness;
        //#else
        //    surfaceReduction = 1.0 / (roughness * roughness + 1.0);
        //#endif
        //half3 reflectedSpecular = surfaceReduction * FresnelLerp(specularColour, grazingTerm, NdotV) * gi.specular;


        // Coloured shadows.
        half3 colouredLightAtten;
        #if defined(_SHADOWS_USE_COLOUR)
            colouredLightAtten = ProduceColouredShadows(light, _ShadowColour);
        #elif defined(_SHADOWS_USE_LIGHT_COLOUR)
            colouredLightAtten = ProduceColouredShadows(light, _ShadowColourFraction);
        #else
            colouredLightAtten = light.colouredShadowAtten;
        #endif


        // Colour the various elements.
        half3 colouredDiffuse;
        #if defined(FORWARD_BASE_PASS) 
        // In the base pass, apply the ramp to the diffuse term.
            colouredDiffuse = albedoColour * (tex2D(_DiffuseRamp, float2(diffuseTerm, 0)) * colouredLightAtten + gi.diffuse);
        #else 
            colouredDiffuse = albedoColour * (diffuseTerm * colouredLightAtten + gi.diffuse);
        #endif

        half3 colouredSpecular = specularTerm * colouredFresnelTerm;
        half3 colouredExtraRimlight = extraRimlight * _RimlightColour;
        half3 colouredSubsurfaceLight = subsurfaceLight * lerp(_SubsurfaceColour, 1.0, saturate(pow(subsurfaceLight, _SubsurfaceColourPower)));
        

        // Assemble and return.
        half3 resultingColour = colouredDiffuse + (colouredSpecular + extraHighlight + colouredExtraRimlight + colouredSubsurfaceLight) * colouredLightAtten;
        return resultingColour;
    }


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


        // Prepare albedo and specular colours, specularity, one minus reflectivity.
        half3 albedoColour = GetAlbedo(i);
        half3 specularColour = GetSpecular(i);
        half oneMinusReflectivity;
        EnergyConservationBetweenDiffuseAndSpecular(albedoColour, specularColour, oneMinusReflectivity);
        half reflectivity = 1.0 - oneMinusReflectivity;

        // Alpha premultiplication.
        #if defined(_ALPHAPREMULTIPLY_ON)
            albedo *= alpha;
            alpha = 1 - oneMinusReflectivity + alpha * oneMinusReflectivity;
        #endif


        // Call the main lighting function.
        half4 finalColour;
        finalColour.rgb = Lighting_QuilleSkin(albedoColour, specularColour, reflectivity, oneMinusReflectivity, GetGlossiness(i), i.normal, viewDir, CreateLight(i), CreateIndirectLight(i, viewDir));
        finalColour.rgb += GetEmission(i);
    

        // Do final alpha.
        #if defined(_ALPHABLEND_ON) || defined(_ALPHAPREMULTIPLY_ON)
            finalColour.a = alpha;
        #endif

        return finalColour;
    }

#endif