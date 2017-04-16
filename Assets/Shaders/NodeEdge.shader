Shader "Custom/NodeEdge"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Animate("Animate", Int) = 0
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

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
			fixed _Animate;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				if(_Animate ==1)
				{
					fixed width = abs(sin(_Time.w))*0.2;
					// sample the texture
					fixed4 col1 = tex2D(_MainTex, float2(0, 0));
					fixed4 col2 = tex2D(_MainTex, float2(0.5, 0.5));
					if (all(i.uv > width) && all(i.uv < 1 - width) || all(i.uv.x < width - 0.03) || all(i.uv.y > 1.03 - width) || all(i.uv.y < width - 0.03) || all(i.uv.x > 1.03 - width))
						return col2;
					else
						return col1;
				}
				else
				{
					return tex2D(_MainTex, i.uv);
				}

				
			}
			ENDCG
		}
	}
}
