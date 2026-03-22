/*
 * Credits to Yafei Ou, this is mostly the DebugParticleRendering.shader
 * Also to Hugo Elias for the integer hash function
 * I added the second pass and the ball coloring
 */

Shader "PhysX 5 for Unity/Screen Space Particle"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Radius ("Radius", float) = 0.15
        _Glossiness ("Smoothness", Range(0, 1)) = 0.0
        _Metallic ("Metallic", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }
            LOD 200
            Cull Off
            ZWrite On
            ZTest Less
    
            CGPROGRAM
    
            #pragma target 5.0
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
    
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "UnityPBSLighting.cginc"
            #include "AutoLight.cginc"
    
            StructuredBuffer<uint> _Indices;
            StructuredBuffer<float4> _Points;
            float _Radius;
            float4 _Color;
            float4 _Glossiness;
            float4 _Metallic;
    
            struct v2g {};
            
            // Hugo Elias's integer hash function
            float hash11(uint n)
            {
                n = (n << 13U) ^ n;
                n = n * (n * n * 15731U + 789221U) + 1376312589U;
                return float(n & 0x7fffffffU) / float(0x7fffffff);
            }

            struct g2f
            {
                float4 pos : SV_Position;
                float3 viewPos : VIEWPOS;
                float2 uv : TEXCOORD0;
                float4 customColor : COLOR;
                SHADOW_COORDS(1)
            };
            
            struct f2o
            {
                float4 color : SV_Target;
                float depth : SV_Depth;
            };
            
            v2g vert()
            {
                v2g o;
                UNITY_INITIALIZE_OUTPUT(v2g, o)
                return o;
            }
            
            [maxvertexcount(4)]
            void geom(point v2g i[1], uint id : SV_PrimitiveId, inout TriangleStream<g2f> triStream)
            {
                uint index = _Indices[id];
                float3 particleWorldPos = _Points[index].xyz;
                
                float randVal = hash11(index);
                float4 particleColor;
                if (randVal < 0.333) 
                {
                    particleColor = float4(1.0, 1.0, 0.0, 1.0);
                } else if (randVal < 0.666) 
                {
                    particleColor = float4(0.0, 1.0, 0.0, 1.0);
                } else 
                {
                    particleColor = float4(1.0, 0.0, 0.0, 1.0);
                }
            
                float3 particleViewPos = mul(UNITY_MATRIX_V, float4(particleWorldPos, 1));
            
                float3 up = float3(0, 1, 0), right = float3(1, 0, 0);
                float3 viewPos[4] = {
                    particleViewPos + (right - up) * _Radius,
                    particleViewPos + (right + up) * _Radius,
                    particleViewPos + (-right - up) * _Radius,
                    particleViewPos + (-right + up) * _Radius,
                };
            
                float2 uv[4] = {
                    float2(1, 0),
                    float2(1, 1),
                    float2(0, 0),
                    float2(0, 1)
                };
            
                g2f o;
                UNITY_INITIALIZE_OUTPUT(g2f, o)
            
                for (int i = 0; i < 4; ++i)
                {
                    float4 pos = mul(UNITY_MATRIX_P, float4(viewPos[i], 1));
                    o.pos = pos;
                    o.viewPos = viewPos[i];
                    o.uv = uv[i];
                    o.customColor = particleColor;
                    TRANSFER_SHADOW(o)
                    triStream.Append(o);
                }
            }
            
            f2o frag(g2f i)
            {
                f2o o;
                UNITY_INITIALIZE_OUTPUT(f2o, o)

                float3 quadCoord = float3(i.uv * float2(2.0, 2.0) - float2(1.0, 1.0), 0);
                float3 rayDirection = normalize(i.viewPos);

                // quadCoord^2 + (rayDirection * t)^2 == R^2
                float sqrT = 1 - dot(quadCoord, quadCoord);
                if (sqrT < 0) discard;
                float t = -sqrt(sqrT);

                float3 viewPos = i.viewPos + rayDirection * t * _Radius;
                
                // Depth
                float4 pos = mul(UNITY_MATRIX_P, float4(viewPos, 1));
                #ifdef SHADER_API_GLCORE
                    o.depth = (pos.z / pos.w + 1.0) * 0.5;
                #else
                    o.depth = pos.z / pos.w;
                #endif
                
                // Surface
                float3 viewNormal = normalize(quadCoord + rayDirection * t);

                float3 worldPos = mul(UNITY_MATRIX_I_V, float4(viewPos, 1));
                float3 worldNormal = mul(UNITY_MATRIX_I_V, viewNormal);
                float3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

                // Surface data
                SurfaceOutputStandard surfaceData = (SurfaceOutputStandard)0;
                surfaceData.Albedo = i.customColor.rgb;
                surfaceData.Normal = worldNormal;
                surfaceData.Emission = 0;
                surfaceData.Metallic = _Metallic;
                surfaceData.Smoothness = _Glossiness;
                surfaceData.Occlusion = 1.0;
                surfaceData.Alpha = i.customColor.a;

                // UnityGI data
                UnityGI gi = (UnityGI)0;
                gi.indirect.diffuse = 0;
                gi.indirect.specular = 0;
                gi.light.color = _LightColor0.rgb;
                gi.light.dir = lightDir;

                UnityGIInput giInput = (UnityGIInput)0;
                giInput.light = gi.light;
                giInput.worldPos = worldPos;
                giInput.worldViewDir = viewDir;
                UNITY_LIGHT_ATTENUATION(atten, i, worldPos)
                giInput.atten = atten;

                float3 sh = 0;
                #if UNITY_SHOULD_SAMPLE_SH
                    sh = ShadeSHPerPixel(worldNormal, 0, worldPos);
                #endif
                giInput.ambient = sh;

                #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
                    giInput.boxMin[0] = unity_SpecCube0_BoxMin;
                    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                
                #ifdef UNITY_SPECCUBE_BOX_PROJECTION
                    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
                    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
                    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
                    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
                    giInput.probeHDR[0] = unity_SpecCube0_HDR;
                    giInput.probeHDR[1] = unity_SpecCube1_HDR;
                #endif
                LightingStandard_GI(surfaceData, giInput, gi);

                o.color.xyz = LightingStandard(surfaceData, viewDir, gi);
            
                return o;
            }

            ENDCG
        }
        
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend One One
            Cull Off
            ZWrite Off
            ZTest LEqual
            
            CGPROGRAM
            #pragma target 5.0
            #pragma multi_compile_fwdadd fullshadows
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
    
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "UnityPBSLighting.cginc"
            #include "AutoLight.cginc"
    
            StructuredBuffer<uint> _Indices;
            StructuredBuffer<float4> _Points;
            float _Radius;
            float4 _Color;
            float4 _Glossiness;
            float4 _Metallic;
    
            struct v2g {};
            
            // Hugo Elias's integer hash function
            float hash11(uint n)
            {
                n = (n << 13U) ^ n;
                n = n * (n * n * 15731U + 789221U) + 1376312589U;
                return float(n & 0x7fffffffU) / float(0x7fffffff);
            }

            struct g2f
            {
                float4 pos : SV_Position;
                float3 viewPos : VIEWPOS;
                float2 uv : TEXCOORD0;
                float4 customColor : COLOR;
                SHADOW_COORDS(1)
            };
            
            struct f2o
            {
                float4 color : SV_Target;
                float depth : SV_Depth;
            };
            
            v2g vert()
            {
                v2g o;
                UNITY_INITIALIZE_OUTPUT(v2g, o)
                return o;
            }
            
            [maxvertexcount(4)]
            void geom(point v2g i[1], uint id : SV_PrimitiveId, inout TriangleStream<g2f> triStream)
            {
                uint index = _Indices[id];
                float3 particleWorldPos = _Points[index].xyz;
                
                float randVal = hash11(index);
                float4 particleColor;
                if (randVal < 0.333) 
                {
                    particleColor = float4(1.0, 1.0, 0.0, 1.0);
                } else if (randVal < 0.666) 
                {
                    particleColor = float4(0.0, 1.0, 0.0, 1.0);
                } else 
                {
                    particleColor = float4(1.0, 0.0, 0.0, 1.0);
                }
            
                float3 particleViewPos = mul(UNITY_MATRIX_V, float4(particleWorldPos, 1));
            
                float3 up = float3(0, 1, 0), right = float3(1, 0, 0);
                float3 viewPos[4] = {
                    particleViewPos + (right - up) * _Radius,
                    particleViewPos + (right + up) * _Radius,
                    particleViewPos + (-right - up) * _Radius,
                    particleViewPos + (-right + up) * _Radius,
                };
            
                float2 uv[4] = {
                    float2(1, 0),
                    float2(1, 1),
                    float2(0, 0),
                    float2(0, 1)
                };
            
                g2f o;
                UNITY_INITIALIZE_OUTPUT(g2f, o)
            
                for (int i = 0; i < 4; ++i)
                {
                    float4 pos = mul(UNITY_MATRIX_P, float4(viewPos[i], 1));
                    o.pos = pos;
                    o.viewPos = viewPos[i];
                    o.uv = uv[i];
                    o.customColor = particleColor;
                    TRANSFER_SHADOW(o)
                    triStream.Append(o);
                }
            }
            
            f2o frag(g2f i)
            {
                f2o o;
                UNITY_INITIALIZE_OUTPUT(f2o, o)

                float3 quadCoord = float3(i.uv * float2(2.0, 2.0) - float2(1.0, 1.0), 0);
                float3 rayDirection = normalize(i.viewPos);

                // quadCoord^2 + (rayDirection * t)^2 == R^2
                float sqrT = 1 - dot(quadCoord, quadCoord);
                if (sqrT < 0) discard;
                float t = -sqrt(sqrT);

                float3 viewPos = i.viewPos + rayDirection * t * _Radius;
                
                // Depth
                float4 pos = mul(UNITY_MATRIX_P, float4(viewPos, 1));
                #ifdef SHADER_API_GLCORE
                    o.depth = (pos.z / pos.w + 1.0) * 0.5;
                #else
                    o.depth = pos.z / pos.w;
                #endif
                
                // Surface
                float3 viewNormal = normalize(quadCoord + rayDirection * t);

                float3 worldPos = mul(UNITY_MATRIX_I_V, float4(viewPos, 1));
                float3 worldNormal = mul(UNITY_MATRIX_I_V, viewNormal);
                float3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

                // Surface data
                SurfaceOutputStandard surfaceData = (SurfaceOutputStandard)0;
                surfaceData.Albedo = i.customColor.rgb;
                surfaceData.Normal = worldNormal;
                surfaceData.Emission = 0;
                surfaceData.Metallic = _Metallic;
                surfaceData.Smoothness = _Glossiness;
                surfaceData.Occlusion = 1.0;
                surfaceData.Alpha = i.customColor.a;

                UNITY_LIGHT_ATTENUATION(atten, i, worldPos)

                UnityGI gi = (UnityGI)0;
                gi.light.color = _LightColor0.rgb * atten;
                gi.light.dir = lightDir;

                o.color.xyz = LightingStandard(surfaceData, viewDir, gi);
                o.color.w = 0;
            
                return o;
            }

            ENDCG
        }
    }
    FallBack Off
}
