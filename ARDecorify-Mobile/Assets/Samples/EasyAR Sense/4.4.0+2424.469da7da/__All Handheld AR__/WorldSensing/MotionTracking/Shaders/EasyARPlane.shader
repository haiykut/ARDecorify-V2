Shader "Sample/EasyARPlane"
{
	Properties
	{
		_MeshSize("Size", Range(1,10)) = 10
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _MeshSize;


			v2f vert (appdata v)
			{
				v2f o;
				float4 worldPos = mul(UNITY_MATRIX_M, v.vertex);
				float4 viewPos = mul(UNITY_MATRIX_V, worldPos);
				o.vertex = mul(UNITY_MATRIX_P, viewPos);
				o.worldPos = worldPos;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.worldPos.xz * _MeshSize;
				float r = length(i.uv - float2(0.5, 0.5));
				fixed4 col = tex2D(_MainTex, uv);
				col *= 0.5 - r;
				return col;
			}
			ENDCG
		}
	}
}
