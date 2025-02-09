#if !defined(QUILLE_LIGHTING_INCLUDED)
#define QUILLE_LIGHTING_INCLUDED

#include "UnityPBSLighting.cginc"

// TODO: rename variables if needed.

// UTILITY
inline float sqr(float x)
{
    return x * x;
}


float3 ShiftTangent(float3 tangent, float3 normal, float shift) // shift receives a shift value + shift texture
{
    float3 shiftedTangent = tangent + shift * normal;
    return normalize(shiftedTangent);
}




// ALTERNATE DIFFUSE MODELS

// https://www.jordanstevenstechart.com/lighting-models
float3 DiffuseWrap(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour, float wrapValue)
{
    // Light calculations.
    float NdotL = saturate(dot(normalDir, lightDir));
    float diffuse = pow(NdotL * wrapValue + (1 - wrapValue), 2.0);

    // Colour and return.
    return diffuse * lightColour;
}

float3 MinnaertDiffuse(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour, float roughness)
{
    // Light calculations.
    float NdotL = saturate(dot(normalDir, lightDir));
    float NdotV = saturate(dot(normalDir, viewDir));
    float NdotE = saturate(dot(normalDir, -viewDir));

    float minnaert = saturate(NdotL * pow(NdotL * NdotV, roughness));
    //float minnaert = saturate(NdotL * pow(NdotL * NdotV, 1 - roughness) * roughness);
    //float minnaert = saturate(pow(NdotL, roughness + 1) * pow(1 - NdotE, 1 - roughness));
    //float minnaert = saturate(pow(NdotL, roughness + 1) * pow(NdotV, 1 - roughness));

    // Colour and return.
    return minnaert * lightColour;
}

float3 OrenNayarDiffuse(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour, float roughness)
{
    // Light calculations.
    float roughnessSquared = roughness * roughness;
    float3 o_n_fraction = roughnessSquared / (roughnessSquared + float3(0.33, 0.13, 0.09));
    float3 oren_nayar = float3(1, 0, 0) + float3(-0.5, 0.17, 0.45) * o_n_fraction;
    float cos_ndotl = saturate(dot(normalDir, lightDir));
    float cos_ndotv = saturate(dot(normalDir, viewDir));
    float oren_nayar_s = saturate(dot(lightDir, viewDir)) - cos_ndotl * cos_ndotv;
    oren_nayar_s /= lerp(max(cos_ndotl, cos_ndotv), 1, step(oren_nayar_s, 0));
    
    // Colour and return.
    return lightColour * cos_ndotl * (oren_nayar.x + lightColour * oren_nayar.y + oren_nayar.z * oren_nayar_s);
}




// SUBSURFACE SCATTERING

// "Subsurface Scattering" using reversed light directions.
// https://walkingfat.com/simple-subsurface-scatterting-for-mobile/
float3 BacklightSubsurfaceScattering(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour, float subsurfaceFrontDistortion, float subsurfaceBackDistortion, float subsurfaceFrontIntensity, float3 subsurfaceColour, float subsurfaceColourIntensity, float thickness)
{
    // Light calculations.
    float3 frontlightDir = normalDir * subsurfaceFrontDistortion - lightDir;
    float3 backlightDir = normalDir * subsurfaceBackDistortion + lightDir;
    
    float subsurfaceFrontValue = saturate(dot(viewDir, -frontlightDir));
    float subsurfaceBackValue = saturate(dot(viewDir, -backlightDir));
    float subsurfaceValue = saturate(subsurfaceFrontValue * subsurfaceFrontIntensity + subsurfaceBackValue);
    
    // Attenuation via thickness (assumed to be between 0 and 1).
    subsurfaceValue *= saturate(1 - thickness);
    
    // Colour and return.
    return lerp(subsurfaceColour, lightColour, saturate(pow(subsurfaceValue, subsurfaceColourIntensity))).rgb * subsurfaceValue;
}

// https://www.patreon.com/posts/shader-tutorial-77970534
float3 EyeVecSubsurfaceScattering(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour, float subsurfaceThreshold, float scatterPower, float scatterScale, float3 subsurfaceColour, float subsurfaceColourIntensity)
{
    // Light calculations.
    float3 eyeVec =  -viewDir;
    float LdotV = smoothstep(subsurfaceThreshold, 1, saturate(dot(eyeVec, lightDir)));
    //float NdotV = saturate(dot(normalDir, viewDir)); // Concentrate most on front normals.
    float NdotV = 1 - saturate(dot(normalDir, viewDir)); // Use as a 'rimlight'
    float subsurfaceValue = LdotV * NdotV;
    subsurfaceValue = pow(subsurfaceValue, scatterPower) * scatterScale;
    // they also added in an always visible ambient value.
    
    // Colour and return.
    return lerp(subsurfaceColour, lightColour, saturate(pow(subsurfaceValue, subsurfaceColourIntensity))).rgb  * subsurfaceValue;
}

// https://www.ea.com/frostbite/news/approximating-translucency-for-a-fast-cheap-and-convincing-subsurface-scattering-look
float3 FrosbiteSubsurfaceScattering(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour,  float subsurfaceDistortion, float scatterPower, float scatterScale, float3 subsurfaceColour, float subsurfaceColourIntensity, float thickness)
{
    // Light calculations.
    float3 distordedL = lightDir + normalDir * subsurfaceDistortion;
    float LdotV = saturate(dot(viewDir, -distordedL)); // Not sure about the sign for viewDir, will need to test with point lights.
    float subsurfaceValue = pow(LdotV, scatterPower) * scatterScale;
    subsurfaceValue *=  saturate(1 - thickness);
    
    // Colour and return.
    return lerp(subsurfaceColour, lightColour, saturate(pow(subsurfaceValue, subsurfaceColourIntensity))).rgb  * subsurfaceValue;
}

float3 FrosbiteZucconiSubsurfaceScattering(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour,  float subsurfaceDistortion, float scatterPower, float scatterScale, float3 subsurfaceColour, float subsurfaceColourIntensity, float thickness)
{
    // Light calculations.
    float3 distordedL = normalize(lightDir + normalDir * subsurfaceDistortion);
    float LdotV = saturate(dot(viewDir, -distordedL)); // Can used to sample a texture.
    float subsurfaceValue = pow(LdotV, scatterPower) * scatterScale;
    // attenuation * (subsurfaceValue + ambient) * thickness;
    
    // Colour and return.
    return lightColour * subsurfaceValue;
}

// "Subsurface Scattering" using a diffuse wrap.
// https://developer.nvidia.com/gpugems/gpugems/part-iii-materials/chapter-16-real-time-approximations-subsurface-scattering
float3 DiffuseWrapSubsurfaceScattering(float3 normalDir, float3 lightDir, float3 lightColour, float subsurfaceDistortion, float3 subsurfaceColour, float subsurfaceColourIntensity)
{
    // Light calculations.
    float subsurfaceValue = saturate((dot(lightDir, normalDir) + subsurfaceDistortion) / (1.0 + subsurfaceDistortion));

    // Colour and return.
    return lerp(subsurfaceColour, lightColour, saturate(pow(subsurfaceValue, subsurfaceColourIntensity))).rgb * subsurfaceValue;
}

// As above, but the "wrapped" light is isolated.
float3 DiffuseWrapSubsurfaceScatteringOnly(float diffuse, float3 lightColour, float subsurfaceDistortion, float subsurfaceScatterWidth, float3 subsurfaceColour)
{
    // Light calculations.
    float subsurfaceValue = saturate((diffuse + subsurfaceDistortion) / (1.0 + subsurfaceDistortion));
    subsurfaceValue = smoothstep(0.0, subsurfaceScatterWidth, subsurfaceValue) * smoothstep(subsurfaceScatterWidth * 2.0, subsurfaceScatterWidth, subsurfaceValue);
    
    // Colour and return.
    return subsurfaceValue * subsurfaceColour * Luminance(lightColour); // Is Luminance too expensive?
}


// Amplify transmission
// max(0 , -dot(normalDir, lightDir)) * lightColour * subsurfaceColour;

// Amplify translucency/SSS
// Shadows can be attenuated for directional lights: float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
// translucency = lightAtten * (subsurfaceValue * ("Direct" [0, 1]) + (indirect diffuse light) * ("Ambient" [0, 1])) * subsurfaceColour * ("Strength" [0, 50]);


// ALTERNATE SPECULAR MODELS

// https://minionsart.github.io/tutorials/Posts.html?post=u_toon_metal
float3 LightDirSpecular(float3 normalDir, float3 lightDir, float3 lightColour, float specularPower, float specularGloss)
{
    // Light calculations.
    float NdotL = dot(normalDir, lightDir);
    float specular = pow(NdotL, specularGloss) * specularPower;
    specular *= (2 + specularPower) / (2 * UNITY_PI);
    
    // Colour and return.
    return specular * lightColour;
}

// https://www.jordanstevenstechart.com/physically-based-rendering
float3 BlinnPhongSpecular(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour, float specularPower, float specularGloss)
{
    // Light calculations.
    float3 halfDir = normalize(lightDir + viewDir);
    float NdotH = dot(normalDir, halfDir);
    float specular = pow(NdotH, specularGloss) * specularPower;
    specular *= (2 + specularPower) / (2 * UNITY_PI);
    
    // Colour and return.
    return specular * lightColour;
}

float3 PhongSpecular(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour, float specularPower, float specularGloss)
{
    // Light calculations.
    float3 lightReflectionDir = reflect(-lightDir, normalDir);
    float specular = dot(lightReflectionDir, viewDir);
    specular = pow(specular, specularGloss) * specularPower;
    specular *= (2 + specularPower) / (2 * UNITY_PI);
    
    // Colour and return.
    return specular * lightColour;
}

float3 BeckmanSpecular(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour, float roughness)
{
    // Light calculations.
    float3 halfDir = normalize(lightDir + viewDir);
    float NdotH = dot(normalDir, halfDir);
    float NdotHSquared = NdotH * NdotH;
    float roughnessSquared = roughness * roughness;
    float specular = max(0.000001,(1.0 / (UNITY_PI * roughnessSquared * NdotHSquared * NdotHSquared)) * exp((NdotHSquared - 1)/(roughnessSquared * NdotHSquared)));
    
    // Colour and return.
    return specular * lightColour;
}

float3 GaussianSpecular(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour, float roughness)
{
    // Light calculations.
    float3 halfDir = normalize(lightDir + viewDir);
    float NdotH = dot(normalDir, halfDir);
    float thetaH = acos(NdotH);
    float roughnessSquared = roughness * roughness;
    float specular = exp(-thetaH * thetaH / roughnessSquared);
    
    // Colour and return.
    return specular * lightColour;
}

// https://github.com/mebusy/notes/blob/master/dev_notes/unityShaderEffectCookbook.md
float3 CircularSpecular(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour, float specularOffset, float specularPower, float specularGloss)
{
    // Light calculations.
    float3 halfDir = normalize(lightDir + viewDir);
    float NdotL = saturate(dot(normalDir, lightDir));
    float HdotA = dot(normalDir, halfDir);

    float circle = max(sin(radians(180 * (HdotA + specularOffset))), 0);
    circle = pow(circle, specularGloss * 128);
    circle = saturate(circle * specularPower);
    
    // Colour and return.
    return circle * lightColour;
}


// TODO: add maps for anisotropic directions?
float3 TRAnisoSpecular(float3 normalDir, float3 tangent, float3 bitangent, float3 viewDir, float3 lightDir, float3 lightColour, float glossiness, float anisotropy)
{
    // Light calculations.
    float3 halfDir = normalize(lightDir + viewDir);
    float NdotH = dot(normalDir, halfDir);
    float HdotX = dot(halfDir, tangent);
    float HdotY = dot(halfDir, bitangent);
    
    float aspect = sqrt(1.0h - anisotropy * 0.9h);
    float X = max(0.001, sqrt(1.0 - glossiness) / aspect) * 5;
    float Y = max(0.001, sqrt(1.0 - glossiness) * aspect) * 5;
    float specular = 1.0 / (UNITY_PI * X * Y * sqr(sqr(HdotX / X) + sqr(HdotY / Y) + NdotH * NdotH));
    
    // Colour and return.
    return specular * lightColour;
}

float3 WardAnisoSpecular(float3 normalDir, float3 tangent, float3 bitangent, float3 viewDir, float3 lightDir, float3 lightColour, float glossiness, float anisotropy)
{
    // Light calculations.
    float3 halfDir = normalize(lightDir + viewDir);
    float NdotL = abs(dot(normalDir, lightDir));
    float NdotV = dot(normalDir, viewDir);
    float NdotH = dot(normalDir, halfDir);
    float HdotT = dot(halfDir, tangent);
    float HdotB = dot(halfDir, bitangent);
    
    float aspect = sqrt(1.0h - anisotropy * 0.9h);
    float slopeX = max(0.001, sqr(1.0 - glossiness) / aspect) * 5;
    float slopeY = max(0.001, sqr(1.0 - glossiness) * aspect) * 5;
    
    float exponent = -(sqr(HdotT / slopeX) + sqr(HdotB / slopeY)) / sqr(NdotH);
    float denom = 4 * UNITY_PI * slopeX * slopeY * sqrt(NdotL * NdotV);
    float specular = exp(exponent) / denom;

    // Colour and return.
    return specular * lightColour;
}

// https://www.yaldex.com/open-gl/ch14lev1sec3.html
// TODO: implement the physical accuracy checks.
float3 WardAnisoSpecular2(float3 normalDir, float3 tangent, float3 bitangent, float3 viewDir, float3 lightDir, float3 lightColour, float glossiness, float anisotropy)
{
  
    // Light calculations.
    float3 halfDir = normalize(lightDir + viewDir);
    float NdotL = abs(dot(normalDir, lightDir));
    float NdotV = dot(normalDir, viewDir);
    float NdotH = dot(normalDir, halfDir);
    float HdotT = dot(halfDir, tangent);
    float HdotB = dot(halfDir, bitangent);
    
    float aspect = sqrt(1.0h - anisotropy * 0.9h);
    float slopeX = max(0.001, sqr(1.0 - glossiness) / aspect) * 5;
    float slopeY = max(0.001, sqr(1.0 - glossiness) * aspect) * 5;
    
    float exponent = -2.0 * ((sqr(HdotT / slopeX) + sqr(HdotB / slopeY)) / (1 + NdotH));
    float denom = 4 * UNITY_PI * slopeX * slopeY * sqrt(NdotL * NdotV);
    float specular = exp(exponent) / denom;
    
    // Colour and return.
    return specular * lightColour;
}



float3 KajiyaKayDiffuse(float3 bitangent, float3 viewDir, float3 lightDir, float3 lightColour, float power)
{
    // Light calculations.
    float TdotL = dot(lightDir, bitangent);
    float sinTL = sqrt(1 - TdotL * TdotL);
    float diffuse = pow(sinTL, power);
    
    // Colour and return.
    return diffuse * lightColour;
}

float3 KajiyaKaySpecular(float3 bitangent, float3 viewDir, float3 lightDir, float3 lightColour, float power)
{
    // Light calculations.
    float3 halfDir = normalize(lightDir + viewDir);
    float TdotH = dot(halfDir, bitangent);
    float sinTH = sqrt(1 - TdotH * TdotH);
    float specular = pow(sinTH, power);

    // Colour and return.
    return specular * lightColour;
}

// https://web.engr.oregonstate.edu/~mjb/cs557/Projects/Papers/HairRendering.pdf
float KajiyaKaySpecularAttenuated(float3 tangent, float3 halfDir, float power)
{
    // Light calculations.
    float TdotH = dot(tangent, halfDir);
    float sinTH = sqrt(1 - sqr(TdotH));
    float dirAtten = smoothstep(-1, 0, TdotH);
    return dirAtten * pow(sinTH, power);
}

float3 FakeMarschnerSpeculars(float3 normalDir, float3 tangent, float3 viewDir, float3 lightDir, float3 lightColour, float shiftA, float shiftB, float powerA, float powerB)
{
    float3 shiftedTangentA = ShiftTangent(tangent, normalDir, shiftA);
    float3 shiftedTangentB = ShiftTangent(tangent, normalDir, shiftB);

    float3 halfDir = normalize(lightDir + viewDir);
    float3 specularA = KajiyaKaySpecularAttenuated(shiftedTangentA, halfDir, powerA);
    float3 specularB = KajiyaKaySpecularAttenuated(shiftedTangentB, halfDir, powerB);
    
    return (specularA * float3(1, 0, 0) + specularB * float3(0, 0, 1)) * lightColour;
}



// https://knarkowicz.wordpress.com/2018/01/04/cloth-shading/ & https://google.github.io/filament/Filament.html#materialsystem/anisotropicmodel
// Sheens and Vs are meant to be combined/multiplied with one another.
float3 AshikhminSheen(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour, float roughness)
{
    // Light calculations.
    float rSqr = roughness * roughness;
    float3 halfDir = normalize(lightDir + viewDir);
    float NdotH = saturate(dot(normalDir, halfDir));
    
    float cos2H = NdotH * NdotH;
    float sin2H = max(0.0078125, 1.0 - cos2H);
    float sin4H = sin2H * sin2H;
    float cot2 = -cos2H / (sin2H * rSqr);
    float sheen = (sin4H + 4.0 * exp(cot2)) / (UNITY_PI * (1.0 + 4.0 * rSqr) * sin4H);

    // Colour and return.
    return sheen * lightColour;
}

float3 AshikhminV(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour)
{
    // Light calculations.
    float NdotL = saturate(dot(normalDir, lightDir));
    float NdotV = saturate(dot(normalDir, viewDir));
    float sheen = 1.0 / (4.0 * (NdotL + NdotV - NdotL * NdotV)) * NdotL * UNITY_PI;
    
    // Colour and return.
    return sheen * lightColour;
}

float3 CharlieSheen(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour, float roughness)
{
    // Light calculations.
    float3 halfDir = normalize(lightDir + viewDir);
    float NdotH = saturate(dot(normalDir, halfDir));

    float invR = 1.0 / roughness;
    float cos2H = NdotH * NdotH;
    float sin2H = 1.0 - cos2H;
    float sheen = (2.0 * invR) * pow(sin2H, invR * 0.5) / (2.0 * UNITY_PI);
    
    // Colour and return.
    return sheen * lightColour;
}

//https://www.shadertoy.com/view/4tfBzn
float L(float x, float r)
{
    r = saturate(r);
    r = 1.0 - (1.0 - r) * (1.0 - r);
    
    float a = lerp( 25.3245,  21.5473, r);
	float b = lerp( 3.32435,  3.82987, r);
	float c = lerp( 0.16801,  0.19823, r);
	float d = lerp(-1.27393, -1.97760, r);
	float e = lerp(-4.85967, -4.32054, r);
    
    return a / (1. + b * pow(x, c)) + d * x + e;
}

float3 CharlieV(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour, float roughness)
{
    // Light calculations.
    float NdotL = saturate(dot(normalDir, lightDir));
    float NdotV = saturate(dot(normalDir, viewDir));
    
    float visV = NdotV < 0.5 ? exp(L(NdotV, roughness)) : exp(2.0 * L(0.5, roughness) - L(1.0 - NdotV, roughness));
    float visL = NdotL < 0.5 ? exp(L(NdotL, roughness)) : exp(2.0 * L(0.5, roughness) - L(1.0 - NdotL, roughness));

    float sTermDenom = max(0.1, 1.0 + visV + visL);
    float sTerm = 1.0 / sTermDenom; // 1/(1 + Λ(ωo) + Λ(ωi))
    float fDenom = max(0.01, 4.0 * NdotV); // 1/(4 |ωo·N| |ωi·N|)
    float sheen = (sTerm / fDenom) * UNITY_PI; // I removed the NdotLs as they cancelled each other out.

    // Colour and return.
    return sheen * lightColour;
}



float Fresnel(float NdotV, float fresnelBias, float fresnelScale, float fresnelPower)
{
    return fresnelBias + fresnelScale * pow(1 - NdotV, fresnelPower);
}

float FresnelSchlick(float NdotV, float reflectionCoef)
{
    return reflectionCoef + (1.0 - reflectionCoef) * pow(1.0 - NdotV, 5);
}

float FresnelSchlickIor(float NdotV, float ior)
{
    float reflectionCoef = pow(ior - 1.0, 2.0) / pow(ior + 1.0, 2.0);
    return FresnelSchlick(NdotV, reflectionCoef);

}




float3 LightDirRimlight(float3 normalDir, float3 viewDir, float3 lightDir, float3 lightColour, float viewDirBias, float viewDirScale, float viewDirPower, float lightDirBias, float lightDirScale, float lightDirPower)
{
    float NdotV = dot(normalDir, viewDir);
    float NdotL = dot(normalDir, lightDir);
   
    float viewDirFresnel = Fresnel(NdotV, viewDirBias, viewDirScale, viewDirPower);
    float lightDirFresnel = Fresnel(1.0 - NdotL, lightDirBias, lightDirScale, lightDirPower);
    float rimlight = viewDirFresnel * lightDirFresnel;
    
    return rimlight * lightColour;
}













#endif