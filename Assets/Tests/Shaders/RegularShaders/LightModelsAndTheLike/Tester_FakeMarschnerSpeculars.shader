

Shader"Tester/MarschnerSpecular"
{
	Properties {
		_Tint ("Tint", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}

		_ShiftTexture ("Shift", 2D) = "black" {}

        [NoScaleOffset] _NormalMap ("Normal", 2D) = "bump" {}
        _BumpScale ("BumpScale", Float) = 1
        
        _ShiftA ("ShiftA", Range(-1, 1)) = 0
        _ShiftB ("ShiftB", Range(-1, 1)) = 0
        _PowerA ("PowerA", Range(1, 200)) = 45
        _PowerB ("PowerB", Range(1, 200)) = 45
	}

    CGINCLUDE

	#define BINORMAL_PER_FRAGMENT

	ENDCG

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

            #include "QuilleLighting.cginc"

            float4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _ShiftTexture;

            sampler2D _NormalMap;
            float _BumpScale;

            float _ShiftA;
            float _ShiftB;
            float _PowerA;
            float _PowerB;


            // STRUCTURES
            struct VertexData
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
    
                #if defined(BINORMAL_PER_FRAGMENT)
                    float4 tangent : TEXCOORD2;
                #else
                    float3 tangent : TEXCOORD2;
                    float3 binormal : TEXCOORD3;
                #endif
    
                float3 worldPos : TEXCOORD4;
            };


            float3 GetTangentSpaceNormal(Interpolators i)
            {
                float3 normal = UnpackScaleNormal(tex2D(_NormalMap, i.uv.xy), _BumpScale);
                return normal;
            }


            // PER VERTEX
            float3 CreateBinormal(float3 normal, float3 tangent, float binormalSign)
            {
                return cross(normal, tangent.xyz) * (binormalSign * unity_WorldTransformParams.w);
            }

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
    
                i.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                //i.uv.zw = TRANSFORM_TEX(v.uv, _DetailTex);
        
                return i;
            }

            // PER FRAGMENT
            float3 InitializeFragmentNormal(inout Interpolators i)
            {
                float3 tangentSpaceNormal = GetTangentSpaceNormal(i);
    
                #if defined(BINORMAL_PER_FRAGMENT)
                    float3 binormal = cross(i.normal, i.tangent.xyz) * (i.tangent.w * unity_WorldTransformParams.w);
                #else
                    float3 binormal = i.binormal;
                #endif
    
                i.normal = normalize(tangentSpaceNormal.x * i.tangent + tangentSpaceNormal.y * binormal + tangentSpaceNormal.z * i.normal);
                return binormal;
            }

            float4 FragmentProgram(Interpolators i) : SV_TARGET
            {
                float3 binormal = InitializeFragmentNormal(i);
    
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                
                float3 lightColour = _LightColor0.rgb;
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                
                float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;

                float shiftTexture = tex2D(_ShiftTexture, i.uv).r * 0.25;

                float3 specular = FakeMarschnerSpeculars(i.normal, binormal, viewDir, lightDir, lightColour, _ShiftA + shiftTexture, _ShiftB + shiftTexture, _PowerA, _PowerB);
        
                return float4(albedo * specular, 1);
            }

            ENDCG
        }
    }
}