Shader "Krakjam/ScreenSpaceChunks"
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

			float _ChunkSizeX;
			float _ChunkSizeY;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				const float2 chunkSize = float2(_ChunkSizeX, _ChunkSizeY);
				float4 color = tex2D(_MainTex, floor((i.uv * _MainTex_TexelSize.zw) / chunkSize) * chunkSize * _MainTex_TexelSize.xy);
				return color;
			}
			ENDCG
		}
	}
}