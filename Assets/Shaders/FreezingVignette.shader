Shader "Custom/FreezingVignette"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (0.7, 0.85, 0.95, 1)
        _TintStrength ("Tint Strength", Range(0, 1)) = 0.8
        _VignetteColor ("Vignette Color", Color) = (0.2, 0.4, 0.9, 1)
        _VignetteIntensity ("Vignette Intensity", Range(0, 2)) = 1.0
        _VignetteRadius ("Vignette Radius", Range(0, 2)) = 0.8
        _PulsePhase ("Pulse Phase", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZTest Always Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _TintColor;
            float _TintStrength;
            fixed4 _VignetteColor;
            float _VignetteIntensity;
            float _VignetteRadius;
            float _PulsePhase;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                col.rgb = lerp(col.rgb, col.rgb * _TintColor.rgb, _TintStrength);

                float2 center = i.uv - 0.5;
                float dist = length(center);

                float pulse = sin(_PulsePhase) * 0.5 + 0.5;
                float radius = _VignetteRadius - pulse * 0.25;

                float vignette = smoothstep(radius, radius + 0.45, dist);
                vignette *= _VignetteIntensity;

                col.rgb = lerp(col.rgb, _VignetteColor.rgb, vignette * 0.6);

                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}
