#if !defined(TEST_LIGHTING_INCLUDED)
#define TEST_LIGHTING_INCLUDED

#include "AutoLight.cginc"
#include "UnityPBSLighting.cginc"

    float4 _Tint;
    sampler2D _MainTex;
    float4 _MainTex_ST;
    float _Metallic;
    float _Smoothness;


    struct vertexData // Vertex Data.
    {
        float4 vertexPos : POSITION;
        float3 normal : NORMAL;
        float2 uv : TEXCOORD0;
    };

    struct interpolatedData // Interpolators. 
    {
        float4 position : SV_POSITION;
        float3 normal : TEXCOORD0;
        float2 uv : TEXCOORD1;
        float3 worldPos : TEXCOORD2;
    
        #if defined(VERTEXLIGHT_ON)
            float3 vertexLightColour : TEXCOORD3;
        #endif
    };


    // PER VERTEX
    void ComputeVertexLightColour(inout interpolatedData input)
    {
        #if defined(VERTEXLIGHT_ON)
            input.vertexLightColour = Shade4PointLights
            (
                unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                unity_LightColor[0].rgb, unity_LightColor[1].rgb,
                unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                unity_4LightAtten0, input.worldPos, input.normal   
            );
        #endif
    }

    interpolatedData VertexProgram(vertexData input)
    {
        interpolatedData output;

        output.position = UnityObjectToClipPos(input.vertexPos);
        output.worldPos = mul(unity_ObjectToWorld, input.vertexPos);
    
        output.normal = UnityObjectToWorldNormal(input.normal);
        output.uv = TRANSFORM_TEX(input.uv, _MainTex);
        ComputeVertexLightColour(output);
                
        return output;
    }


    // PER FRAGMENT
    UnityLight CreateLight(interpolatedData input)
    {
        UnityLight light;
    
        #if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
            light.dir = normalize(_WorldSpaceLightPos0.xyz - input.worldPos);
        #else
            light.dir = _WorldSpaceLightPos0.xyz;
        #endif
    
        UNITY_LIGHT_ATTENUATION(attenuation, ObjectRayDirection, input.worldPos);
        light.color = _LightColor0.rgb * attenuation;
        light.ndotl = DotClamped(input.normal, light.dir);
        return light;
    }

    UnityIndirect CreateIndirectLight(interpolatedData input)
    {
        UnityIndirect indirectLight;
        indirectLight.diffuse = 0;
        indirectLight.specular = 0;
    
        #if defined(VERTEXLIGHT_ON)
            indirectLight.diffuse = input.vertexLightColour;
        #endif
    
        #if defined(FORWARD_BASE_PASS)
            indirectLight.diffuse += max(0, ShadeSH9(float4(input.normal, 1)));
        #endif
        
        return indirectLight;
    }

    float4 FragmentProgram(interpolatedData input) : SV_TARGET
    {
        input.normal = normalize(input.normal);
    
        float3 viewDir = normalize(_WorldSpaceCameraPos - input.worldPos);
    
        float3 albedo = tex2D(_MainTex, input.uv).rgb * _Tint.rgb;
        float3 specularTint;
        float oneMinusReflectivity;
        albedo = DiffuseAndSpecularFromMetallic(albedo, _Metallic, specularTint, oneMinusReflectivity);

        return UNITY_BRDF_PBS(albedo, specularTint, oneMinusReflectivity, _Smoothness, input.normal, viewDir, CreateLight(input), CreateIndirectLight(input));
    }

#endif