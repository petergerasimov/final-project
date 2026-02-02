#ifndef SCREEN_SPACE_FLUID_DEPTH_PLAIN_INCLUDED
#define SCREEN_SPACE_FLUID_DEPTH_PLAIN_INCLUDED

#include "UnityCG.cginc"

uint _IndexCount;
StructuredBuffer<uint> _Indices;
StructuredBuffer<float4> _Points;
float4x4 _ViewMatrix;
float4x4 _ProjMatrix;
float _ParticleDiameter;

struct v2g {};

struct g2f
{
    float4 pos : SV_Position;
    float3 viewPos : VIEWPOS;
    float2 uv : TEXCOORD0;
    float scale : ANI_SCALE;
};

struct f2o
{
    float depth : SV_Target;
};

v2g vert() {
    v2g o;
    UNITY_INITIALIZE_OUTPUT(v2g, o)
    return o;
}

[maxvertexcount(4)]
void geom(point v2g i[1], uint id : SV_PrimitiveId, inout TriangleStream<g2f> triStream)
{
    if (id >= _IndexCount) return;
    uint index = _Indices[id];
    float3 particleWorldPos = _Points[index].xyz;

    #ifdef UNITY_PASS_SHADOWCASTER
        #define MATRIX_V UNITY_MATRIX_V
        #define MATRIX_P UNITY_MATRIX_P
    #else
        #define MATRIX_V _ViewMatrix
        #define MATRIX_P _ProjMatrix
    #endif

    float3 particleViewPos = mul(MATRIX_V, float4(particleWorldPos, 1)).xyz;

    //float scale = max(0.01, _ParticleDiameter * 0.5);
    float scale = 0.08;

    float3 up = float3(0, 1, 0), right = float3(1, 0, 0);
    float3 viewPosArr[4] = {
        particleViewPos + (right - up) * scale,
        particleViewPos + (right + up) * scale,
        particleViewPos + (-right - up) * scale,
        particleViewPos + (-right + up) * scale
    };

    float2 uvArr[4] = {
        float2(1, 0),
        float2(1, 1),
        float2(0, 0),
        float2(0, 1)
    };

    g2f o;
    UNITY_INITIALIZE_OUTPUT(g2f, o)
    o.scale = scale;

    for (int idx = 0; idx < 4; ++idx)
    {
        float4 pos = mul(MATRIX_P, float4(viewPosArr[idx], 1));
        #ifdef UNITY_PASS_SHADOWCASTER
            pos = UnityApplyLinearShadowBias(pos);
        #endif
        o.pos = pos;
        o.viewPos = viewPosArr[idx];
        o.uv = uvArr[idx];
        triStream.Append(o);
    }
}

f2o frag(g2f i)
{
    f2o o;
    UNITY_INITIALIZE_OUTPUT(f2o, o)

    float3 quadCoord = float3(i.uv * float2(2.0, 2.0) - float2(1.0, 1.0), 0);
    float3 rayDirection = normalize(i.viewPos);

    // (quadCoord + rayDirection * t)^2 == 1  (unit sphere in quad-space)
    float a = dot(rayDirection, rayDirection);
    float halfB = dot(rayDirection, quadCoord);
    float c = dot(quadCoord, quadCoord) - 1;

    if (a == 0 || halfB == 0) discard;
    float delta = halfB * halfB - a * c;
    if (delta < 0) discard;

    float t = (-halfB - sqrt(delta)) / a;

    // move along the view ray by t, then scale by the particle radius
    float3 surfaceViewPos = i.viewPos + rayDirection * t * i.scale;

    #ifdef UNITY_PASS_SHADOWCASTER
        float4 clipPos = mul(UNITY_MATRIX_P, float4(surfaceViewPos, 1));
        #ifdef SHADER_API_GLCORE
            o.depth = (clipPos.z / clipPos.w + 1.0) * 0.5;
        #else
            o.depth = clipPos.z / clipPos.w;
        #endif
    #else
        o.depth = -surfaceViewPos.z;
    #endif

    return o;
}

#endif
