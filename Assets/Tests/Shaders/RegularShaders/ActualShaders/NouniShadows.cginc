#if !defined(NOUNI_SHADOWS_INCLUDED)
#define NOUNI_SHADOWS_INCLUDED

#include "UnityCG.cginc"

#if defined(_ALPHABLEND_ON) || defined(_ALPHAPREMULTIPLY_ON)
    #if defined(_SEMITRANSPARENT_SHADOWS)
        #define SHADOWS_SEMITRANSPARENT 1
    #else
        #define _ALPHATEST_ON
    #endif
#endif

#if SHADOWS_SEMITRANSPARENT || defined(_ALPHATEST_ON)
    #define SHADOWS_NEED_UV 1
#endif


//---------------------------------------

    // PARAMETERS
    half4 _Color;
    sampler2D _AlphaMap;
    sampler2D _MainTex;
    float4 _MainTex_ST;

    half _AlphaCutoff;

    sampler3D _DitherMaskLOD;


//---------------------------------------

    // STRUCTURES

    // VERTICES
    struct VertexData
    {
        float4 position : POSITION;
        float3 normal : NORMAL;
        float2 uv : TEXCOORD0;
    };

    struct InterpolatorsVertex
    {
        float4 position : SV_POSITION;
        #if SHADOWS_NEED_UV
            float2 uv : TEXCOORD0;
        #endif
	    #if defined(SHADOWS_CUBE)
		    float3 lightVec : TEXCOORD1;
	    #endif
    };

    // INTERPOLATORS/FRAGMENTS
    struct Interpolators
    {
        #if SHADOWS_SEMITRANSPARENT
            UNITY_VPOS_TYPE vpos : VPOS;
        #else
            float4 positions : SV_POSITION;
        #endif
    
        #if SHADOWS_NEED_UV
            float2 uv : TEXCOORD0;
        #endif
	    #if defined(SHADOWS_CUBE)
		    float3 lightVec : TEXCOORD1;
	    #endif
    };


//---------------------------------------

    // UTILITIES

    // GETTERS
    half GetAlpha(InterpolatorsVertex i)
    {
        half alpha = _Color.a;
        #if SHADOWS_NEED_UV
            #if !defined(_ALPHA_MAP)
            // The alpha map is stored in the diffuse's 'a' channel.
                alpha *= tex2D(_MainTex, i.uv.xy).a;
            #else
                alpha *= tex2D(_AlphaMap, i.uv.xy).r;
            #endif
        #endif
        return alpha;
    }


//---------------------------------------

    // FUNCTIONS

    // PER VERTEX
    InterpolatorsVertex ShadowVertexProgram(VertexData v)
    {   
        InterpolatorsVertex i;
        #if defined(SHADOWS_CUBE)
            i.position = UnityObjectToClipPos(v.position);
            i.lightVec = mul(unity_ObjectToWorld, v.position).xyz - _LightPositionRange.xyz;
        #else
            i.position = UnityClipSpaceShadowCasterPos(v.position.xyz, v.normal);
            i.position = UnityApplyLinearShadowBias(i.position);
        #endif
        #if SHADOWS_NEED_UV
            i.uv = TRANSFORM_TEX(v.uv, _MainTex);
        #endif
        return i;
    }
    

    // PER FRAGMENT
    half4 ShadowFragmentProgram(Interpolators i) : SV_Target
    {
        float alpha = GetAlpha(i);
        #if defined(_ALPHATEST_ON)
            clip(alpha - _AlphaCutoff);
        #endif
        
        #if SHADOWS_SEMITRANSPARENT
            float dither = tex3D(_DitherMaskLOD, float3(i.vpos.xy * 0.25, alpha * 0.9375)).a;
            clip(dither - 0.01);
        #endif        

        #if defined(SHADOWS_CUBE)
            float depth = length(i.lightVec) + unity_LightShadowBias.x;
            depth *= _LightPositionRange.w;
            return UnityEncodeCubeShadowDepth(depth);
        #else
            return 0;
        #endif
    }

#endif