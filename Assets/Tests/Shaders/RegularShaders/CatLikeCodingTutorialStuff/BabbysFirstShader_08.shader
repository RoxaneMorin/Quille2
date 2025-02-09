Shader"Lit/BabbysFirstShader - Added Complexity"
{
	Properties 
    {
		_Tint ("Tint", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}
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
	}

    //CGINCLUDE
    //    #define BINORMAL_PER_FRAGMENT
    //ENDCG

    SubShader 
    {

        Pass  
        {
            Tags
            {
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM

            #pragma target 3.0

            #pragma multi_compile _ SHADOWS_SCREEN
            #pragma multi_compile _ VERTEXLIGHT_ON

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
            Tags
            {
                "LightMode" = "ForwardAdd"
            }

            Blend One One
            ZWrite Off

            CGPROGRAM

            #pragma target 3.0

            #pragma multi_compile_fwdadd_fullshadows

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
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            CGPROGRAM

            #pragma target 3.0

            #pragma multi_compile_shadowcaster

            #pragma vertex ShadowVertexProgram
            #pragma fragment ShadowFragmentProgram

            #include "TutorialShadows.cginc"

            ENDCG
        }
    }

    //CustomEditor "BabbysFirstShaderGUI"
}
