Shader "Custom/ARPlaneDots"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,0.5)
        _DotColor ("Dot Color", Color) = (1,1,1,1)
        _Spacing ("Dots per Unit", Float) = 4.0
        _DotRadius ("Dot Radius (0..0.5)", Range(0.01,0.5)) = 0.05
        _Softness ("Dot Softness", Range(0.001,0.2)) = 0.03
        _MaxDistance ("Max Dot Distance", Float) = 2.0
        _Alpha ("Alpha", Range(0,1)) = 0.9
        _FlickerSpeed ("Flicker Speed", Float) = 5.0
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
                float3 normal : NORMAL; // Tambahkan normal
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1; // Tambahkan world normal
            };

            fixed4 _Color;
            fixed4 _DotColor;
            float _Spacing;
            float _DotRadius;
            float _Softness;
            float _MaxDistance;
            float _Alpha;
            float _FlickerSpeed;

            v2f vert (appv v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal); // Ubah normal ke world space
                return o;
            }

            float2 fract2(float2 x) { return x - floor(x); }

            fixed4 frag (v2f i) : SV_Target
            {
                float distFromCamera = length(i.worldPos - _WorldSpaceCameraPos.xyz);
                if (distFromCamera > _MaxDistance)
                {
                    discard;
                }

                // Pilih sumbu berdasarkan orientasi normal plane
                float3 n = abs(i.worldNormal);
                float2 planeUV;

                if (n.y > n.x && n.y > n.z) // Horizontal plane (normal menghadap atas)
                {
                    planeUV = i.worldPos.xz;
                }
                else if (n.x > n.y && n.x > n.z) // Vertical plane (normal menghadap samping X)
                {
                    planeUV = i.worldPos.yz;
                }
                else // Vertical plane (normal menghadap samping Z)
                {
                    planeUV = i.worldPos.xy;
                }

                // Hitung posisi titik
                float2 cell = planeUV * _Spacing;
                float2 f = fract2(cell);
                float2 center = f - 0.5;
                float dist = length(center);

                // Animasi kedip-kedip
                float flicker = sin(_Time.y * _FlickerSpeed + length(i.worldPos.xz)) * 0.5 + 0.5;
                float animatedRadius = _DotRadius * lerp(0.8, 1.2, flicker);

                // Hitung falloff titik
                float dotMask = 1.0 - smoothstep(animatedRadius - _Softness, animatedRadius + _Softness, dist);

                // Warna akhir
                fixed4 col = lerp(_Color, _DotColor, dotMask);
                col.a *= dotMask * _Alpha;

                // Membuang piksel yang sangat transparan
                clip(col.a - 0.001);

                return col;
            }
            ENDCG
        }
    }
}