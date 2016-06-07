Shader "Hidden/FlipWebApps/BeautifulTransitions/FadeCamera"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_OverlayTex("Overlay Texture", 2D) = "white" {}
		_Color("Tint Color", COLOR) = (0,0,0,0)
		_Amount("Amount", Range(0, 1)) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off
		ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature INVERT_MASK

			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _OverlayTex;
			fixed4 _Color;
			fixed _Amount;

			float4 _MainTex_TexelSize;

			v2f vert(appdata_img v) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;
				o.uv2 = v.texcoord.xy;
#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv2.y = 1.0 - o.uv2.y;
#endif
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 overlayCol = tex2D(_OverlayTex, i.uv2);
				overlayCol = overlayCol * _Color;
				col.rgb = lerp(col, overlayCol, _Amount);
				return col;
			}
			ENDCG
		}
	}
}
