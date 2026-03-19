Shader "Custom/ClothMeshShader"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _MainTex ("Albedo (Alpha)", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        _Glossiness ("Smoothness", Range(0,1)) = 0.0
        _Metallic ("Metallic", Range(0,1)) = 0.0

        [Header(Mask)]
        _MaskTex ("Mask Texture (R = keep)", 2D) = "white" {}
        _MaskEnabled ("Mask Enabled", Range(0,1)) = 1.0
        _MaskOffset ("Mask Offset (XY)", Vector) = (0,0,0,0)
        _MaskScale ("Mask Scale (XY)", Vector) = (1,1,0,0)
        _MaskRotation ("Mask Rotation (degrees)", Range(0, 360)) = 0.0
        _MaskCutoff ("Mask Cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 200
        Cull Off
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alphatest:_Cutoff addshadow
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MaskTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _MaskEnabled;
        float4 _MaskOffset;
        float4 _MaskScale;
        float _MaskRotation;
        float _MaskCutoff;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

            float maskAlpha = 1.0;
            if (_MaskEnabled > 0.5)
            {
                float2 centered = IN.uv_MainTex - float2(0.5, 0.5);

                centered /= max(_MaskScale.xy, 0.0001);

                float rad = _MaskRotation * (3.14159265 / 180.0);
                float cosA = cos(rad);
                float sinA = sin(rad);
                float2 rotated = float2(
                    centered.x * cosA - centered.y * sinA,
                    centered.x * sinA + centered.y * cosA
                );

                float2 maskUV = rotated + float2(0.5, 0.5) - _MaskOffset.xy;

                if (maskUV.x < 0.0 || maskUV.x > 1.0 ||
                    maskUV.y < 0.0 || maskUV.y > 1.0)
                {
                    maskAlpha = 1.0;
                }
                else
                {
                    fixed4 mask = tex2D(_MaskTex, clamp(maskUV, 0.0, 1.0));
                    maskAlpha = step(_MaskCutoff, mask.r);
                }
            }

            o.Albedo = _Color.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness = _Glossiness;
            o.Alpha = c.a * _Color.a * maskAlpha;
        }
        ENDCG
    }
    FallBack "Transparent/Cutout/Diffuse"
}
