Shader "Unlit/GodraysShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ProxFade ("Proximity Fade Smoothness", Float) = 16
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Cull Off
            ZWrite Off
            Blend One One        // Additive
            ColorMask RGBA
            Fog { Mode Exp2 }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
			float _ProxFade;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;      // Clip-space position for rasterization
                fixed4 color : COLOR0;         // Vertex color from ParticleSystem
                float2 uv : TEXCOORD0;         // Texture coordinates
                float3 camPos : TEXCOORD1;     // Camera-space position for distance-based alpha
                float3 viewNormal : TEXCOORD2; // Camera-space normal
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // World-space position
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.camPos = mul(UNITY_MATRIX_V, float4(worldPos,1)).xyz;

                // World-space normal
                float3 worldNorm = UnityObjectToWorldNormal(v.normal);
                float3 viewNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);
                o.viewNormal = viewNorm;

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 tex = tex2D(_MainTex, uv);

                // Multiply by tint and vertex color
                fixed3 rgb = tex.rgb * _Color.rgb * i.color.rgb * _Color.a * i.color.a;
                fixed alpha = tex.a * _Color.a * i.color.a;
                fixed4 outCol = fixed4(rgb, alpha);

                // Distance from camera
                float distance = abs(i.camPos.z); // camera-space forward distance
                float dVis = 1 - saturate(distance / 70);
                outCol.rgb *= dVis;

                // Proximity fade
                outCol.rgb *= saturate(distance * distance / _ProxFade);

                // Camera facing factor
                // facing = 1 when normal faces camera directly, 0 when perpendicular
                float3 viewDir = normalize(-i.camPos); // from vertex to camera
                float facing = abs(dot(i.viewNormal, viewDir)); 
                outCol.rgb *= saturate(facing);

                return outCol;
            }
            ENDCG
        }
    }

    FallBack "Particles/Alpha Blended"
}