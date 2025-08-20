#if !defined(NOUNI_COMMON_INPUTS_INCLUDED)
#define NOUNI_COMMON_INPUTS_INCLUDED

#include "NouniAutoLight.cginc"
#include "UnityPBSLighting.cginc"

//---------------------------------------

    // PARAMETERS

    // MAIN
    half4 _Color;
    sampler2D _MainTex;
    float4 _MainTex_ST;

    sampler2D _AlphaMap;
    half _AlphaCutoff;

    sampler2D _BumpMap;
    half _BumpScale;

    sampler2D _OcclusionMap;
    half _OcclusionStrength;

    sampler2D _EmissionMap;
    half3 _Emission;


    // DETAIL
    sampler2D _DetailMask;
    half _DetailUVs;

    sampler2D _DetailTex;
    float4 _DetailTex_ST;

    sampler2D _DetailBumpMap;
    half _DetailBumpScale;


    // SURFACE PROPS
    sampler2D _SpecGlossMap;
    half3 _SpecularColor;

    sampler2D _MetallicGlossMap;
    half _Metallic;

    sampler2D _GlossMap;
    half _Glossiness;


//---------------------------------------

    // STRUCTURES

    // VERTICES
    struct VertexData
    {
        float4 vertex : POSITION;

        float3 normal : NORMAL;
        float4 tangent : TANGENT;

        float2 uv0 : TEXCOORD0;
        float2 uv1 : TEXCOORD1;
        #if defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META)
            float2 uv2      : TEXCOORD2;
        #endif
    };

    // INTERPOLATORS/FRAGMENTS
    struct Interpolators
    {
        float4 pos : SV_POSITION;

        float4 uv : TEXCOORD0;
        // TODO: add lightmap UVs.

        float3 normal : TEXCOORD1;
        #if defined(BINORMAL_PER_FRAGMENT)
            float4 tangent : TEXCOORD2;
        #else
            float3 tangent : TEXCOORD2;
            float3 binormal : TEXCOORD3;
        #endif
    
        float3 worldPos : TEXCOORD4;
    
        SHADOW_COORDS(5)

	    #if defined(VERTEXLIGHT_ON)
		    float3 vertexLightColor : TEXCOORD6;
	    #endif
    };


//---------------------------------------

    // UTILITIES

    // GETTERS
    half GetAlpha(Interpolators i)
    {
        half alpha = _Color.a;
        // The alpha map is stored in the diffuse's 'a' channel.
        #if defined(_ALPHA_MAP)
            alpha *= tex2D(_AlphaMap, i.uv.xy).r;
        #else
            alpha *= tex2D(_MainTex, i.uv.xy).a;
        #endif
        return alpha;
    }

    half GetDetailMask(Interpolators i)
    {
        #if defined (_DETAIL_MASK)
            return tex2D(_DetailMask, i.uv.xy).a;
        #else
            return 1;
        #endif
    }

    half3 GetAlbedo(Interpolators i)
    {
        float3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color.rgb;
        #if defined (_DETAIL_ALBEDO_MAP)
            float3 details = tex2D(_DetailTex, i.uv.zw) * unity_ColorSpaceDouble;
            albedo = lerp(albedo, albedo * details, GetDetailMask(i));
        #endif
        return albedo;
    }

    float3 GetTangentSpaceNormal(Interpolators i)
    {
        float3 normal = float3(0, 0, 1);
        #if defined(_NORMAL_MAP)
            normal = UnpackScaleNormal(tex2D(_BumpMap, i.uv.xy), _BumpScale);
        #endif
        #if defined(_DETAIL_NORMAL_MAP)
            float3 detailNormal = UnpackScaleNormal(tex2D(_DetailBumpMap, i.uv.zw), _DetailBumpScale);
            detailNormal = lerp(float3(0, 0, 1), detailNormal, GetDetailMask(i));
            normal = BlendNormals(normal, detailNormal);
        #endif
        return normal;
    }

    half GetOcclusion(Interpolators i)
    {
        #if defined(_OCCLUSION_MAP)
            return lerp(1, tex2D(_OcclusionMap, i.uv.xy).g, _OcclusionStrength);
        #else
            return 1;
        #endif
    }

    half3 GetEmission(Interpolators i)
    {
        #if defined(FORWARD_BASE_PASS)
            #if defined(_EMISSION_MAP)
			    return tex2D(_EmissionMap, i.uv.xy) * _Emission;
		    #else
			    return _Emission;
		    #endif
	    #else
		    return 0;
	    #endif
    }

    half3 GetSpecular(Interpolators i)
    {
        #if defined(_SPECULAR_MAP)
            return tex2D(_SpecGlossMap, i.uv.xy).rgb * _SpecularColor;
        #else
            return _SpecularColor;
        #endif
    }

    half GetMetallic(Interpolators i)
    {
        #if defined(_METALLIC_MAP)
            return tex2D(_MetallicGlossMap, i.uv.xy).r * _Metallic;
        #else
            return _Metallic;
        #endif
    }

    half GetGlossiness(Interpolators i)
    {
        half glossiness = _Glossiness;
        
        #if defined (_GLOSS_MAP)
            glossiness *= tex2D(_GlossMap, i.uv.xy).r;
        #else
            #if defined(_SPECULAR_MAP) 
                glossiness *= tex2D(_SpecGlossMap, i.uv.xy).a;
            #elif defined(_METALLIC_MAP)
                glossiness *= tex2D(_MetallicGlossMap, i.uv.xy).a;
            #endif
        #endif
        
        return glossiness;
    }

#endif