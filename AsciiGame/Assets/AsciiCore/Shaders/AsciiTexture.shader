Shader "Krakjam/AsciiTexture"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
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
			float4 _MainTex_TexelSize;
			float _Exposure;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			UNITY_DECLARE_TEX2DARRAY(_CharacterTextureArray);

			float _ChunkSizeX;
			float _ChunkSizeY;

			float4 frag(v2f i) : SV_Target
			{
				const float4 data = tex2D(_MainTex, i.uv);

				const float id = data.r;
				const float4 sourceColor = float4(data.yzw, 1.0f);

				float4 col = UNITY_SAMPLE_TEX2DARRAY(_CharacterTextureArray, float3((i.uv * _MainTex_TexelSize.zw) / float2(_ChunkSizeX, _ChunkSizeY), id));
				return lerp(sourceColor * float4(_Exposure, _Exposure, _Exposure, 1.0), col * sourceColor, saturate(length(col.xyz)));
			}
		ENDCG
	}
	}
}