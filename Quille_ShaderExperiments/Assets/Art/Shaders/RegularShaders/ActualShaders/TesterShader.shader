Shader"TesterShader"
{
	Properties 
    {
        // MAIN
		[MainColor] _Color ("Colour", Color) = (1,1,1,1)
		[MainTexture] _MainTex ("Albedo", 2D) = "white" {}

        [NoScaleOffset] _AlphaMap ("Alpha", 2D) = "white" {}
        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.5

        [Normal] _BumpMap ("Normal", 2D) = "bump" {}
        _BumpScale ("BumpScale", Range(0, 3)) = 1

        [NoScaleOffset] _OcclusionMap ("Occlusion", 2D) = "white" {}
        _OcclusionStrength ("Occlusion Strength", Range (0, 1)) = 1

        [NoScaleOffset] _EmissionMap ("Emission", 2D) = "black" {}
		_Emission ("Emission", Color) = (0, 0, 0)


        // DETAIL
        [NoScaleOffset] _DetailMask ("Detail Mask", 2D) = "white" {}

        _DetailTex ("Detail Albedo", 2D) = "gray" {}
        [Normal] _DetailBumpMap ("Detail Normal", 2D) = "bump" {}
        _DetailBumpScale ("Detail Bump Scale", Range(0, 3)) = 1

        [Enum(UV0,0,UV1,1)] _DetailUVs ("UV Set used for detail textures.", Float) = 0


        // SURFACE PROPS
        [NoScaleOffset] _SpecGlossMap ("Specular Map", 2D) = "white" {}
        _SpecularColor ("Specular Colour", Color) = (0.25,0.25,0.25)

        [NoScaleOffset] _MetallicGlossMap ("Metallicness Map", 2D) = "white" {}
        [Gamma] _Metallic ("Metallicness", Range(0, 1)) = 0

        [NoScaleOffset] _GlossMap ("Glossiness Map", 2D) = "white" {}
		_Glossiness ("Glossiness", Range(0, 1)) = 0.5

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0


        // OTHER
        _ShadowColourFraction ("Shadow % of Light Colour", Range(0, 0.5)) = 0.1
        _ShadowColour ("Shadow Colour", Color) = (0.1, 0.1, 0.1)


        // HIDE IN INSPECTOR
        [HideInInspector] _SrcBlend ("_SrcBlend", Float) = 1
        [HideInInspector] _DstBlend ("_DstBlend", Float) = 0
        [HideInInspector] _ZWrite ("_ZWrite", Float) = 1
	}

    //CGINCLUDE
    //    #define BINORMAL_PER_FRAGMENT
    //ENDCG

    SubShader 
    {

        Pass  
        {
            Name "FORWARD"
            Tags
            {
                "LightMode" = "ForwardBase"
            }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            CGPROGRAM

            #pragma target 3.0

            #pragma multi_compile _ SHADOWS_SCREEN
            #pragma multi_compile _ VERTEXLIGHT_ON

            #pragma shader_feature_local _ _SPECULAR_SETUP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            
            #pragma shader_feature_local _ALPHA_MAP
            #pragma shader_feature_local _NORMAL_MAP
            #pragma shader_feature_local _OCCLUSION_MAP
            #pragma shader_feature_local _EMISSION_MAP
            
            #pragma shader_feature_local _DETAIL_MASK
            #pragma shader_feature_local _DETAIL_ALBEDO_MAP
			#pragma shader_feature_local _DETAIL_NORMAL_MAP

            #pragma shader_feature_local _SPECULAR_MAP
            #pragma shader_feature_local _METALLIC_MAP
            #pragma shader_feature_local _GLOSS_MAP

            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF

            #pragma shader_feature_local _ _SHADOWS_USE_COLOUR _SHADOWS_USE_LIGHT_COLOUR

            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram

            #define FORWARD_BASE_PASS

            #include "NouniFragBRDF.cginc"

            ENDCG
        }

        Pass  
        {
            Name "FORWARD_DELTA"
            Tags
            {
                "LightMode" = "ForwardAdd"
            }

            Blend [_SrcBlend] One
            ZWrite Off

            CGPROGRAM

            #pragma target 3.0

            #pragma multi_compile_fwdadd_fullshadows

            #pragma shader_feature_local _ _SPECULAR_SETUP
            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            
            #pragma shader_feature_local _ALPHA_MAP
            #pragma shader_feature_local _NORMAL_MAP
            #pragma shader_feature_local _OCCLUSION_MAP
            #pragma shader_feature_local _EMISSION_MAP
            
            #pragma shader_feature_local _DETAIL_MASK
            #pragma shader_feature_local _DETAIL_ALBEDO_MAP
			#pragma shader_feature_local _DETAIL_NORMAL_MAP

            #pragma shader_feature_local _SPECULAR_MAP
            #pragma shader_feature_local _METALLIC_MAP
            #pragma shader_feature_local _ _GLOSS_MAP

            #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local _GLOSSYREFLECTIONS_OFF

            #pragma shader_feature_local _ _SHADOWS_USE_COLOUR _SHADOWS_USE_LIGHT_COLOUR

            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram

            #include "NouniFragBRDF.cginc"

            ENDCG
        }

        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            CGPROGRAM

            #pragma target 3.0

            #pragma multi_compile_shadowcaster

            #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature_local _ALPHA_MAP
            #pragma shader_feature_local _SEMITRANSPARENT_SHADOWS

            #pragma vertex ShadowVertexProgram
            #pragma fragment ShadowFragmentProgram

            #include "NouniShadows.cginc"

            ENDCG
        }
    }

    CustomEditor "NouniShaderGUI"
}
