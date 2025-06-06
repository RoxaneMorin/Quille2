﻿Shader"Lit/BabbysFirstShader - Transparency"
{
	Properties 
    {
		[MainColor] _Tint ("Tint", Color) = (1,1,1,1)
		[MainTexture] _MainTex ("Albedo", 2D) = "white" {}
        [NoScaleOffset] _NormalMap ("Normal", 2D) = "bump" {}
        _BumpScale ("BumpScale", Float) = 1

        [NoScaleOffset] _MetallicMap ("Metallicness", 2D) = "white" {}
        [Gamma] _Metallic ("Metallicness", Range(0, 1)) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5

        [NoScaleOffset] _EmissionMap ("Emission", 2D) = "black" {}
		_Emission ("Emission", Color) = (0, 0, 0)

        [NoScaleOffset] _OcclusionMap ("Occlusion", 2D) = "white" {}
        _OcclusionStrength ("Occlusion Strength", Range (0, 1)) = 1
        
        [NoScaleOffset] _DetailMask ("Detail Mask", 2D) = "white" {}

        _DetailTex ("Detail Albedo", 2D) = "gray" {}
        [NoScaleOffset] _DetailNormalMap ("Detail Normal", 2D) = "bump" {}
        _DetailBumpScale ("Detail Bump Scale", Float) = 1

        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        [HideInInspector] _SrcBlend ("_SrcBlend", Float) = 1
        [HideInInspector] _DstBlend ("_DstBlend", Float) = 0
        [HideInInspector] _ZWrite ("_ZWrite", Float) = 1
	}

    CGINCLUDE
        #define BINORMAL_PER_FRAGMENT
    ENDCG

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

            #pragma shader_feature _ _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT
            #pragma shader_feature _NORMAL_MAP
            #pragma shader_feature _METALLIC_MAP
            #pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
            #pragma shader_feature _EMISSION_MAP
            #pragma shader_feature _OCCLUSION_MAP
            #pragma shader_feature _DETAIL_MASK
            #pragma shader_feature _DETAIL_ALBEDO_MAP
			#pragma shader_feature _DETAIL_NORMAL_MAP

            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram

            #define FORWARD_BASE_PASS

            #include "TutorialLighting.cginc"

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

            #pragma shader_feature _ _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT
            #pragma shader_feature _NORMAL_MAP
            #pragma shader_feature _METALLIC_MAP
            #pragma shader_feature _ _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC
            #pragma shader_feature _DETAIL_MASK
            #pragma shader_feature _DETAIL_ALBEDO_MAP
			#pragma shader_feature _DETAIL_NORMAL_MAP

            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram

            #include "TutorialLighting.cginc"

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

            #pragma shader_feature _SEMITRANSPARENT_SHADOWS
            #pragma shader_feature _ _RENDERING_CUTOUT _RENDERING_FADE _RENDERING_TRANSPARENT
			#pragma shader_feature _SMOOTHNESS_ALBEDO

            #pragma vertex ShadowVertexProgram
            #pragma fragment ShadowFragmentProgram

            #include "TutorialShadows.cginc"

            ENDCG
        }
    }

    CustomEditor "BabbysFirstShaderGUI"
}
