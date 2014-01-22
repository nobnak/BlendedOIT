Shader "Custom/OITPostEffect" {
	Properties {
		_MainTex ("Accum", 2D) = "black" {}
		_RevealageTex ("Revealage", 2D) = "white" {}
	}
	SubShader {
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
		Blend OneMinusSrcAlpha SrcAlpha
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _RevealageTex;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			struct vs2ps {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;			
			};
			
			vs2ps vert(appdata i) {
				vs2ps o;
				o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
				o.uv = i.uv;
				return o;
			}
			
			float4 frag(vs2ps i) : COLOR {
				 float4 accum = tex2D(_MainTex, i.uv);
				 float4 revealage = tex2D(_RevealageTex, i.uv);
				 return float4(accum.rgb / clamp(accum.a, 1e-4, 5e4), revealage.r);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
