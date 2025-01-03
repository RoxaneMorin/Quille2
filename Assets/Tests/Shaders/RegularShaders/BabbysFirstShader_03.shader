// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader"Unlit/BabbysFirstShader - Shaded!"
{
	Properties {
		_Tint ("Tint", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}
        [Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5

	}

    SubShader{

        Pass  {
            Tags
            {
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM

            #pragma target 3.0

            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram

            #include "UnityPBSLighting.cginc"

            float4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Metallic;
            float _Smoothness;


            struct vertexInput // Vertex Data.
            {
                float4 vertexPos : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct vertexOutput // Interpolators. 
            {
                float4 position : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };


            vertexOutput VertexProgram(vertexInput input)
            {
                vertexOutput output;

                output.position = UnityObjectToClipPos(input.vertexPos);
                output.worldPos = mul(unity_ObjectToWorld, input.vertexPos);
    
                output.normal = UnityObjectToWorldNormal(input.normal);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                return output;
            }

            float4 FragmentProgram(vertexOutput input) : SV_TARGET
            {
                input.normal = normalize(input.normal);
                
                float3 lightColour = _LightColor0.rgb;
    
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 viewDir = normalize(_WorldSpaceCameraPos - input.worldPos);
                //float3 reflectionDir = reflect(-lightDir, input.normal);
                //float3 halfVector = normalize(lightDir + viewDir);

                float3 albedo = tex2D(_MainTex, input.uv).rgb * _Tint.rgb;
                float3 specularTint;
                float oneMinusReflectivity;
                albedo = DiffuseAndSpecularFromMetallic(albedo, _Metallic, specularTint, oneMinusReflectivity);
    
                //float3 diffuse = albedo * lightColour * DotClamped(lightDir, input.normal);
                //float3 specular = specularTint * lightColour *  pow(DotClamped(halfVector, input.normal), _Smoothness * 100);
    
                //return pow(DotClamped(viewDir, reflectionDir), _Smoothness * 100);
                //return float4(diffuse + specular, 1);
    
                UnityLight light;
                light.color = lightColour;
                light.dir = lightDir;
                light.ndotl = DotClamped(input.normal, lightDir);
                UnityIndirect indirectLight;
                indirectLight.diffuse = 0;
                indirectLight.specular = 0;
    
                return UNITY_BRDF_PBS(albedo, specularTint, oneMinusReflectivity, _Smoothness, input.normal, viewDir, light, indirectLight);
            }

            ENDCG
        }
    }
}
