Shader "URP/StylizedWater"
{
    Properties
    {
        _DepthGradient1 ("Depth Gradient 1", Color) = (0.1098039,0.5960785,0.6196079,1)
        _DepthGradient2 ("Depth Gradient 2", Color) = (0.05882353,0.1960784,0.4627451,1)
        _DepthGradient3 ("Depth Gradient 3", Color) = (0,0.0625,0.25,1)
        _GradientPosition1 ("Gradient Position 1", Float) = 1.6
        _GradientPosition2 ("Gradient Position 2", Float) = 2
        _FresnelColor ("Fresnel Color", Color) = (0.5764706,0.6980392,0.8000001,1)
        _FresnelExp ("Fresnel Exponent", Range(0, 10)) = 10
        _Roughness ("Roughness", Range(0.01, 1)) = 0.6357628
        _LightColorIntensity ("Light Color Intensity", Range(0, 1)) = 0.7759457
        _SpecularIntensity ("Specular Intensity", Range(0, 1)) = 1
        _FoamColor ("Foam Color", Color) = (0.854902,0.9921569,1,1)
        _MainFoamScale ("Main Foam Scale", Float) = 40
        _MainFoamIntensity ("Main Foam Intensity", Range(0, 10)) = 3.84466
        _MainFoamSpeed ("Main Foam Speed", Float) = 0.1
        _MainFoamOpacity ("Main Foam Opacity", Range(0, 1)) = 0.8737864
        _SecondaryFoamScale ("Secondary Foam Scale", Float) = 40
        _SecondaryFoamIntensity ("Secondary Foam Intensity", Range(0, 10)) = 2.330097
        _SecondaryFoamOpacity ("Secondary Foam Opacity", Range(0, 1)) = 0.6310679
        [Toggle] _SecondaryFoamAlwaysVisible ("Secondary Foam Always Visible", Float) = 1
        _TurbulenceDistortionIntensity ("Turbulence Distortion Intensity", Range(0, 6)) = 0.8155341
        _TurbulenceScale ("Turbulence Scale", Float) = 10
        _WaveDistortionIntensity ("Wave Distortion Intensity", Range(0, 4)) = 0.592233
        _WavesDirection ("Waves Direction", Range(0, 360)) = 0
        _WavesAmplitude ("Waves Amplitude", Range(0, 10)) = 4.980582
        _WavesSpeed ("Waves Speed", Float) = 1
        _WavesIntensity ("Waves Intensity", Float) = 2
        [Toggle] _VertexOffset ("Vertex Offset", Float) = 0
        [Toggle] _RealTimeReflection ("Real Time Reflection", Float) = 0
        _ReflectionsIntensity ("Reflections Intensity", Range(0, 3)) = 1
        _OpacityDepth ("Opacity Depth", Float) = 5
        _Opacity ("Opacity", Range(0, 1)) = 0.7378641
        _RefractionIntensity ("Refraction Intensity", Float) = 1
        _DistortionTexture ("Distortion Texture", 2D) = "white" {}
        _FoamTexture ("Foam Texture", 2D) = "white" {}
        _ReflectionTex ("Reflection Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "Transparent"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _DepthGradient1;
                float4 _DepthGradient2;
                float4 _DepthGradient3;
                float4 _FresnelColor;
                float4 _FoamColor;
                float _GradientPosition1;
                float _GradientPosition2;
                float _FresnelExp;
                float _Roughness;
                float _LightColorIntensity;
                float _SpecularIntensity;
                float _MainFoamScale;
                float _MainFoamIntensity;
                float _MainFoamSpeed;
                float _MainFoamOpacity;
                float _SecondaryFoamScale;
                float _SecondaryFoamIntensity;
                float _SecondaryFoamOpacity;
                float _SecondaryFoamAlwaysVisible;
                float _TurbulenceDistortionIntensity;
                float _TurbulenceScale;
                float _WaveDistortionIntensity;
                float _WavesDirection;
                float _WavesAmplitude;
                float _WavesSpeed;
                float _WavesIntensity;
                float _VertexOffset;
                float _RealTimeReflection;
                float _ReflectionsIntensity;
                float _OpacityDepth;
                float _Opacity;
                float _RefractionIntensity;
                float4 _DistortionTexture_ST;
                float4 _FoamTexture_ST;
                float4 _ReflectionTex_ST;
            CBUFFER_END

            TEXTURE2D(_DistortionTexture);
            SAMPLER(sampler_DistortionTexture);
            TEXTURE2D(_FoamTexture);
            SAMPLER(sampler_FoamTexture);
            TEXTURE2D(_ReflectionTex);
            SAMPLER(sampler_ReflectionTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float fogCoord : TEXCOORD4;
            };

            float CorrectDepth(float rawDepth)
            {
                #if UNITY_REVERSED_Z
                    rawDepth = 1.0 - rawDepth;
                #endif
                
                float persp = LinearEyeDepth(rawDepth, _ZBufferParams);
                float ortho = (_ProjectionParams.z - _ProjectionParams.y) * (1 - rawDepth) + _ProjectionParams.y;
                return lerp(persp, ortho, unity_OrthoParams.w);
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                output.uv = input.uv;
                output.normalWS = normalInput.normalWS;
                output.positionWS = vertexInput.positionWS;
                
                // Wave calculations for vertex displacement
                float node_9794_ang = (_WavesDirection / 57.0);
                float node_9794_cos = cos(node_9794_ang);
                float node_9794_sin = sin(node_9794_ang);
                float2 node_9794_piv = float2(0.5, 0.5);
                float2 node_9794 = mul(output.uv - node_9794_piv, float2x2(node_9794_cos, -node_9794_sin, node_9794_sin, node_9794_cos)) + node_9794_piv;
                
                float4 gradientSample = SAMPLE_TEXTURE2D_LOD(_DistortionTexture, sampler_DistortionTexture, TRANSFORM_TEX(node_9794, _DistortionTexture), 0);
                float node_5335 = sin((_Time.y * _WavesSpeed) - (gradientSample.b * (_WavesAmplitude * 30.0)));
                float waves = node_5335 * _WavesIntensity * 10.0;
                
                // Apply vertex displacement
                float3 vertexOffset = lerp(0.0, normalInput.normalWS * (waves * 0.04), _VertexOffset);
                output.positionWS += vertexOffset;
                output.positionCS = TransformWorldToHClip(output.positionWS);
                
                output.screenPos = ComputeScreenPos(output.positionCS);
                output.fogCoord = ComputeFogFactor(output.positionCS.z);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Sample depth and calculate scene depth
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                float rawDepth = SampleSceneDepth(screenUV);
                float sceneDepth = CorrectDepth(rawDepth);
                float eyeDepth = LinearEyeDepth(input.positionCS.z, _ZBufferParams);
                float depthDifference = sceneDepth - eyeDepth;
                
                // Direction vectors
                float3 viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                float3 normalWS = normalize(input.normalWS);
                
                // Wave calculations
                float node_9794_ang = (_WavesDirection / 57.0);
                float node_9794_cos = cos(node_9794_ang);
                float node_9794_sin = sin(node_9794_ang);
                float2 node_9794_piv = float2(0.5, 0.5);
                float2 node_9794 = mul(input.uv - node_9794_piv, float2x2(node_9794_cos, -node_9794_sin, node_9794_sin, node_9794_cos)) + node_9794_piv;
                
                float4 gradientSample = SAMPLE_TEXTURE2D(_DistortionTexture, sampler_DistortionTexture, TRANSFORM_TEX(node_9794, _DistortionTexture));
                float node_5335 = sin((_Time.y * _WavesSpeed) - (gradientSample.b * (_WavesAmplitude * 30.0)));
                float waves = node_5335 * _WavesIntensity * 10.0;
                
                // Turbulence
                float2 turbulenceUV = (input.uv * _TurbulenceScale) + _Time.y * float2(0.01, 0.01);
                float4 distortionExtra = SAMPLE_TEXTURE2D(_DistortionTexture, sampler_DistortionTexture, TRANSFORM_TEX(turbulenceUV, _DistortionTexture));
                float turbulence = (0.05 * waves * _WaveDistortionIntensity) + ((distortionExtra.g * _TurbulenceDistortionIntensity) * 2.0);
                
                // Refraction
                float2 refractionOffset = (0.01 * _RefractionIntensity) * (turbulence * (input.uv * 2.0));
                float2 refractionUV = screenUV + refractionOffset;
                float3 sceneColor = SampleSceneColor(refractionUV);
                
                // Reflections
                float2 reflectionUV = lerp(screenUV, screenUV + 0.01, turbulence);
                float4 reflectionSample = SAMPLE_TEXTURE2D(_ReflectionTex, sampler_ReflectionTex, TRANSFORM_TEX(reflectionUV, _ReflectionTex));
                float3 reflections = lerp(0.0, (reflectionSample.rgb * 0.3) * _ReflectionsIntensity, _RealTimeReflection);
                
                // Depth gradient
                float depthGradient1 = saturate(depthDifference / _GradientPosition1);
                float depthGradient2 = saturate(pow(saturate(depthDifference / (_GradientPosition1 + _GradientPosition2)), 3.0));
                float3 baseColor = lerp(_DepthGradient1.rgb, lerp(_DepthGradient2.rgb, _DepthGradient3.rgb, depthGradient2), depthGradient1);
                
                // Fresnel
                float fresnel = pow(1.0 - max(0, dot(normalWS, viewDirectionWS)), _FresnelExp);
                float3 waterColor = lerp(baseColor, _FresnelColor.rgb, saturate(fresnel));
                
                // Main foam
                float foamSpeed = _MainFoamSpeed * 0.15;
                float2 mainFoamUV = (float2(foamSpeed, foamSpeed) * _Time.y) + (input.uv * _MainFoamScale);
                float4 foamNoise = SAMPLE_TEXTURE2D(_FoamTexture, sampler_FoamTexture, TRANSFORM_TEX(mainFoamUV, _FoamTexture));
                float foamDepth = (node_5335 * 0.1 + 0.2) * (foamNoise.r * _MainFoamIntensity);
                float mainFoam = (1.0 - saturate(pow(saturate(saturate(depthDifference / foamDepth)), 15.0) / 0.1)) * _MainFoamOpacity;
                
                // Secondary foam
                float2 secondaryFoamUV = (input.uv * _SecondaryFoamScale) + _Time.y * float2(0.01, 0.01);
                float4 secondaryFoamNoise = SAMPLE_TEXTURE2D(_FoamTexture, sampler_FoamTexture, TRANSFORM_TEX(secondaryFoamUV, _FoamTexture));
                float secondaryFoamMask = lerp((1.0 - saturate(pow(saturate(saturate(depthDifference / _SecondaryFoamIntensity)), 1.0) / 0.8)), 1.0, _SecondaryFoamAlwaysVisible);
                float secondaryFoam = (secondaryFoamMask * (pow(saturate((secondaryFoamNoise.r * 4.0 - 1.0)), 0.5) * 0.3)) * _SecondaryFoamOpacity;
                
                // Lighting
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float3 viewReflect = reflect(-viewDirectionWS, normalWS);
                float specular = pow(max(0, dot(lightDir, viewReflect)), exp2(((1.0 - _Roughness) * 5.0 + 5.0)));
                float3 customLight = saturate((((turbulence / _Roughness) * 0.8 + 0.2) * ((2.0 * specular) * mainLight.color.rgb))) * _SpecularIntensity;
                
                // Final color composition
                float3 finalWaterColor = waterColor + reflections;
                float3 foamContribution = (_FoamColor.rgb * mainFoam) + (_FoamColor.rgb * secondaryFoam);
                float3 lightContribution = lerp(1.0, clamp(mainLight.color.rgb, 0.3, 1.0), _LightColorIntensity);
                
                float3 finalColor = (finalWaterColor + foamContribution) * lightContribution + customLight;
                
                // Alpha calculation
                float foamMask = mainFoam + (secondaryFoam * 0.2);
                float opacityFromDepth = saturate(saturate(depthDifference / _OpacityDepth));
                float finalAlpha = saturate(foamMask + _Opacity + opacityFromDepth + length(customLight));
                
                // Final output
                float3 finalResult = lerp(sceneColor, finalColor, finalAlpha);
                finalResult = MixFog(finalResult, input.fogCoord);
                
                return half4(finalResult, 1.0);
            }
            ENDHLSL
        }
    }
    
    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
