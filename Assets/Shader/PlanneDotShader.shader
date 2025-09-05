Shader "Custom/ARPlaneDots"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,0.5)
        _DotColor ("Dot Color", Color) = (1,1,1,1)
        _Spacing ("Dots per Unit", Float) = 4.0
        _DotRadius ("Dot Radius (0..0.5)", Range(0.01,0.5)) = 0.12
        _Softness ("Dot Softness", Range(0.001,0.2)) = 0.03
        _EdgeFade ("Edge Fade (0..0.5)", Range(0,0.5)) = 0.08
        _Noise ("Noise Intensity", Range(0,1)) = 0.08
        _Alpha ("Alpha", Range(0,1)) = 0.9
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appv {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            fixed4 _Color;
            fixed4 _DotColor;
            float _Spacing;
            float _DotRadius;
            float _Softness;
            float _EdgeFade;
            float _Noise;
            float _Alpha;

            v2f vert (appv v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            // simple fract helper
            float2 fract2(float2 x) { return x - floor(x); }

            fixed4 frag (v2f i) : SV_Target
            {
                // world-space xz used for stable repeating if needed:
                float2 worldXZ = i.worldPos.xz;

                // cell coords
                float2 cell = worldXZ * _Spacing;
                float2 f = fract2(cell);

                // distance from cell center
                float2 center = f - 0.5;
                float dist = length(center);

                // dot falloff
                float dotMask = 1.0 - smoothstep(_DotRadius - _Softness, _DotRadius + _Softness, dist);

                // slight procedural variation / noise
                float n = sin(worldXZ.x * 12.9898 + worldXZ.y * 78.233) * 43758.5453;
                n = frac(n);
                dotMask *= lerp(1.0 - _Noise, 1.0 + _Noise, n);

                // edge fade based on mesh UV (assumes plane UV ~ [0..1])
                float2 centeredUV = i.uv - 0.5;
                float edgeDist = length(centeredUV);
                float edgeMask = saturate(1.0 - smoothstep(0.5 - _EdgeFade, 0.5, edgeDist));

                // final color
                fixed4 col = lerp(_Color, _DotColor, dotMask);
                col.a *= dotMask * edgeMask * _Alpha;

                // premult alpha-ish look (optional subtle darkening)
                col.rgb *= col.a;

                // discard fully transparent pixels to avoid sorting artifacts sometimes
                clip(col.a - 0.001);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
