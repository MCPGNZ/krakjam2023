// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SubtractivePlane"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _PlaneNormal("Plane Normal", Vector) = (0, 0, 1, 0)
        _PlanePosition("Plane Position", Vector) = (0, 0, 1, 0)
    }

        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            LOD 200

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows

            #pragma target 5.0

            sampler2D _MainTex;

            struct Input
            {
                float2 uv_MainTex;
                float3 worldPos;
            };

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            float3 _PlaneNormal;
            float3 _PlanePosition;

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata_full v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(UNITY_MATRIX_M, v.vertex).xyz;

                return o;
            }

            float rayCastPlane(float3 wPos, float3 camDir, float3 normal, float zPos)
            {
                float dn = dot(camDir, normal);

                if (dn >= 0.0f)
                {
                    return -1.0f;
                }

                float t = (zPos + dot(wPos, normal)) / dn;

                if (t >= 0.0f)
                {
                    return t;
                }

                return -1.0f;
            }

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                float dist = dot(IN.worldPos, _PlaneNormal) - dot(_PlanePosition, _PlaneNormal);
                if (step(0.0f, dist))
                {
                    discard;
                }
                float4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;

                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}