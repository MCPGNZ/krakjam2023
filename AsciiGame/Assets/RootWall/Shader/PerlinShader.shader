Shader "Unlit/PerlinShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _PerlinFrequency("Frequency", Float) = 1.0
        _PerlinAmplitude("Amplitude", Float) = 1.0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

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

                sampler2D _MainTex;
                float4 _MainTex_ST;

                float _PerlinFrequency;
                float _PerlinAmplitude;

                float random(in float3 p)
                {
                    return frac(sin(dot(p, float3(19.5423f, 33.32353, 44.5345f))) * 43567.4534f);
                }

                float sampleSeamlessNoise(in float3 p, in float freq)
                {
                    return random(float3(p.x % freq, p.y % freq, p.z % freq));
                }

                float noise(in float3 p, in float freq)
                {
                    float3 iuv = floor(p);
                    float3 fuv = frac(p);
                    float a = sampleSeamlessNoise(iuv, freq);
                    float b = sampleSeamlessNoise(iuv + float3(1.0f, 0.0f, 0.0f), freq);
                    float c = sampleSeamlessNoise(iuv + float3(0.0f, 1.0f, 0.0f), freq);
                    float d = sampleSeamlessNoise(iuv + float3(1.0f, 1.0f, 0.0f), freq);

                    float a2 = sampleSeamlessNoise(iuv + float3(0.0f, 0.0f, 1.0f), freq);
                    float b2 = sampleSeamlessNoise(iuv + float3(1.0f, 0.0f, 1.0f), freq);
                    float c2 = sampleSeamlessNoise(iuv + float3(0.0f, 1.0f, 1.0f), freq);
                    float d2 = sampleSeamlessNoise(iuv + float3(1.0f, 1.0f, 1.0f), freq);

                    float3 u = fuv * fuv * fuv * (fuv * (fuv * 6.0f - 15.0f) + 10.0f); // w*w*w*(w*(w*6.0-15.0)+10.0);

                    float ab = lerp(a, b, u.x);
                    float cd = lerp(c, d, u.x);

                    float a2b2 = lerp(a2, b2, u.x);
                    float c2d2 = lerp(c2, d2, u.x);

                    float abcd = lerp(ab, cd, u.y);
                    float a2b2c2d2 = lerp(a2b2, c2d2, u.y);

                    return lerp(abcd, a2b2c2d2, u.z);
                }

                //lacunarity should be 2.0f in order to get proper values for seamless Perlin Noise
                float fbm(in float3 uv)
                {
                    float amp = 0.5f;
                    float gain = 0.5f;
                    float lacunarity = 2.0f;
                    float result = 0.0f;
                    float f = _PerlinFrequency;
                    for (int i = 0; i < 8; ++i)
                    {
                        result += amp * noise(uv, _PerlinFrequency);
                        amp *= gain;
                        uv += uv * pow(lacunarity, float(i));
                        f += f * pow(lacunarity, float(i));
                    }

                    return result;
                }

                v2f vert(appdata v)
                {
                    v2f o;
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    v.vertex.y += fbm(float3(o.uv, _Time.w)) * _PerlinAmplitude;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    float4 col = tex2D(_MainTex, i.uv);
                    return col;
                }
                ENDCG
            }
        }
}