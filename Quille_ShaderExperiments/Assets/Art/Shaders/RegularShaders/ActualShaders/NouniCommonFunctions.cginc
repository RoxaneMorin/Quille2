#if !defined(NOUNI_COMMON_FUNCTIONS_INCLUDED)
#define NOUNI_COMMON_FUNCTIONS_INCLUDED

#include "NouniCommonInputs.cginc"
#include "NouniLightLibrary.cginc"

//---------------------------------------

    // UTILITIES

    // NORMALS
    float3 CreateBinormal(float3 normal, float3 tangent, float binormalSign)
    {
        return cross(normal, tangent.xyz) * (binormalSign * unity_WorldTransformParams.w);
    }

    void InitializeFragmentNormal(inout Interpolators i)
    {
        float3 tangentSpaceNormal = GetTangentSpaceNormal(i);
    
        #if defined(BINORMAL_PER_FRAGMENT)
            float3 binormal = cross(i.normal, i.tangent.xyz) * (i.tangent.w * unity_WorldTransformParams.w);
        #else
            float3 binormal = i.binormal;
        #endif
    
        i.normal = normalize(tangentSpaceNormal.x * i.tangent + tangentSpaceNormal.y * binormal + tangentSpaceNormal.z * i.normal);
    }


    // REFLECTIONS
    half3 BoxProjection (float3 direction, float3 position, float4 cubemapPosition, float3 boxMin, float3 boxMax)
    {
        #if UNITY_SPECCUBE_BOX_PROJECTION
            UNITY_BRANCH
            if (cubemapPosition.w > 0)
            {
                half3 factors = ((direction > 0 ? boxMax : boxMin) - position) / direction;
                half scalar = min(min(factors.x, factors.y), factors.z);
                direction = direction * scalar + (position - cubemapPosition);
            }
        #endif
        return direction;
    }


    // LIGHTING
    void ComputeVertexLightColour(inout Interpolators i)
    {
        #if defined(VERTEXLIGHT_ON)
            i.vertexLightColor = Shade4PointLights
            (
                unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                unity_LightColor[0].rgb, unity_LightColor[1].rgb,
                unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                unity_4LightAtten0, i.worldPos, i.normal   
            );
        #endif
    }

    ExtendedUnityLight CreateLight(Interpolators i)
    {
        ExtendedUnityLight light;
    
        #if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
            light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
        #else
            light.dir = _WorldSpaceLightPos0.xyz;
        #endif

        UNITY_LIGHT_ATTENUATION(shadow, attenuation, i, i.worldPos);
        light.shadow = shadow;
        light.attenuation = attenuation;
        light.combinedShadowAtten = shadow * attenuation;
        light.color = _LightColor0.rgb;
        light.intensity = _LightColor0.a;
        light.colouredShadowAtten = light.combinedShadowAtten * light.color;

        return light;
    }

    UnityIndirect CreateIndirectLight(Interpolators i, float3 viewDir)
    {
        UnityIndirect indirectLight;
        indirectLight.diffuse = 0;
        indirectLight.specular = 0;
    
        #if defined(VERTEXLIGHT_ON)
            indirectLight.diffuse = i.vertexLightColor;
        #endif
    
        #if defined(FORWARD_BASE_PASS)
            indirectLight.diffuse += max(0, ShadeSH9(float4(i.normal, 1)));

            #ifdef _GLOSSYREFLECTIONS_OFF
                indirectLight.specular = unity_IndirectSpecColor.rgb;
            #else
                half3 reflectionDir = reflect(-viewDir, i.normal);
                Unity_GlossyEnvironmentData envData;
                envData.roughness = 1 - GetGlossiness(i);
                envData.reflUVW = BoxProjection(reflectionDir, i.worldPos, unity_SpecCube0_ProbePosition, unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax);
                half3 probe0 = Unity_GlossyEnvironment(UNITY_PASS_TEXCUBE(unity_SpecCube0), unity_SpecCube0_HDR, envData);
            
                #if UNITY_SPECCUBE_BLENDING
                    half interpolator = unity_SpecCube0_BoxMin.w;
                    UNITY_BRANCH
                        if (interpolator < 0.99999)
                        {
                            envData.reflUVW = BoxProjection(reflectionDir, i.worldPos, unity_SpecCube1_ProbePosition, unity_SpecCube1_BoxMin, unity_SpecCube1_BoxMax);
                            half3 probe1 = Unity_GlossyEnvironment(UNITY_PASS_TEXCUBE_SAMPLER(unity_SpecCube1, unity_SpecCube0), unity_SpecCube0_HDR, envData);
                            indirectLight.specular = lerp(probe1, probe0, interpolator);
                        }
                        else
                        {
                            indirectLight.specular = probe0;   
                        }
                #else
                    indirectLight.specular = probe0;
                #endif
            #endif
            
            half occlusion = GetOcclusion(i);
            indirectLight.diffuse *= occlusion;
            indirectLight.specular *= occlusion;
        #endif        

        return indirectLight;
    }


//---------------------------------------

    // FUNCTIONS
    
    // PER VERTEX
    Interpolators VertexProgram(VertexData v)
    {
        Interpolators i;

        i.pos = UnityObjectToClipPos(v.vertex);
        i.worldPos = mul(unity_ObjectToWorld, v.vertex);
        i.normal = UnityObjectToWorldNormal(v.normal);
    
        #if defined(BINORMAL_PER_FRAGMENT)
            i.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
        #else
            i.tangent = UnityObjectToWorldDir(v.tangent.xyz);
            i.binormal = CreateBinormal(i.normal, i.tangent, v.tangent.w);
        #endif
    
        i.uv.xy = TRANSFORM_TEX(v.uv0, _MainTex);
        i.uv.zw = (_DetailUVs == 0) ? TRANSFORM_TEX(v.uv0, _DetailTex) : TRANSFORM_TEX(v.uv1, _DetailTex);
    
        TRANSFER_SHADOW(i);
    
        ComputeVertexLightColour(i);
        
        return i;
    }

#endif