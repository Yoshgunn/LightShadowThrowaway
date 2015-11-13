Shader "Hidden/BWDiffuse" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		//_BGText ("Replacement Tex", 2D) = "white" {}
		//_bwBlend ("Black & White blend", Range (0, 1)) = 0
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _BGTex;
			//uniform float _bwBlend;

			float4 frag(v2f_img i) : COLOR {
				float4 c = tex2D(_MainTex, i.uv);
				float4 c2 = tex2D(_BGTex, i.uv);
				
				float lum = c.r*.3 + c.g*.59 + c.b*.11;
				float3 bw = float3( lum, lum, lum ); 
				
				float4 result = c;
				//result.rgb = lerp(c.rgb, bw, _bwBlend);
				result.rgb = bw;
				
				//float rep = 1-min(c.r+c.g+c.b,1)
				result.rgb = lerp(c2.rgb, c.rgb, min(1000*(c.r+c.g+c.b),1));
				
				return result;
			}
			ENDCG
		}
	}
}