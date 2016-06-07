Shader "Hidden/FlipWebApps/BeautifulTransitions/WipeCamera"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_OverlayTex("Overlay Texture", 2D) = "white" {}
		_Color("Tint Color", COLOR) = (0,0,0,0)
		_MaskTex("Mask Texture", 2D) = "white" {}
		_Amount("Amount", Range(0, 1)) = 0
		[Toggle(INVERT_MASK)] _INVERT_MASK("Mask Invert", Float) = 0
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
			sampler2D _MaskTex;
			sampler2D _OverlayTex;
			float _Amount;
			fixed4 _Color;

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

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 mask = tex2D(_MaskTex, i.uv2);
				fixed4 overlayColor = tex2D(_OverlayTex, i.uv2);

				// adjust range 0-255 -> 0-254 so alpha is never 1 (otherwise mask weight would not return 0 when _Amount = 1)
				fixed alpha = mask.a * (1 - 1 / 255.0);

				// showBackground == 1 when mask image alpha is >= current amount, 0 otherwise.
				fixed showBackground = step(_Amount, alpha);

// invert if keyword is set
#if INVERT_MASK
				showBackground = 1 - showBackground;
#endif

				// Colour the wipe texture and then select wipe or main texture pixels
				col.rgb = lerp(_Color.rgb * overlayColor.rgb, col.rgb, showBackground);
				return col;
			}
			ENDCG
		}
	}
}
