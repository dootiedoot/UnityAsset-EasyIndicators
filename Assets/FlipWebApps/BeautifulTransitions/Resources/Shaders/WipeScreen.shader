Shader "FlipWebApps/BeautifulTransitions/WipeScreen"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_MaskTex("Mask Texture", 2D) = "white" {}
		_Amount("Wipe Amount", Range(0, 1)) = 0
		_Color("Wipe Color", COLOR) = (0,0,0,0)
		[Toggle(INVERT_MASK)] _INVERT_MASK("Mask Invert", Float) = 0 
	}
	SubShader
	{
		Tags{ 
			"Queue" = "Transparent" 
			"RenderType" = "Transparent" 
			"IgnoreProjector" = "True"
		}

		// No culling or depth
		Cull Off 
		ZWrite Off 
		ZTest Always 
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma shader_feature INVERT_MASK

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _MaskTex;
			fixed _Amount;
			fixed4 _Color;

			fixed4 frag(v2f_img i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 mask = tex2D(_MaskTex, i.uv);

				// adjust range 0-255 -> 0-254 so alpha is never 1 (otherwise mask weight would not return 0 when _Amount = 1)
				fixed alpha = mask.a * (1 - 1 / 255.0);

				// showBackground == 1 when mask image alpha is >= current amount, 0 otherwise.
				fixed showBackground = step(_Amount, alpha);

				// Colour the main texture and then set alpha based upon whether to show background or not
				col.rgb = col.rgb * _Color.rgb;

				// invert if keyword is set
#if INVERT_MASK
				//showBackground = 1 - showBackground;
				col.a = showBackground;
#else
				col.a = 1 - showBackground;
#endif
				return col;
			}
			ENDCG
		}
	}
}
