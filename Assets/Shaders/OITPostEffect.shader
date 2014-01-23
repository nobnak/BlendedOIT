Shader "Custom/OITPostEffect" {
	Properties {
		_MainTex ("Opaque", 2D) = "black" {}
		_AccumTex ("Accum", 2D) = "black" {}
		_RevealageTex ("Revealage", 2D) = "white" {}
	}
	SubShader {
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
		//Blend OneMinusSrcAlpha SrcAlpha
		Blend One Zero
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _AccumTex;
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
				float4 c0 = tex2D(_MainTex, i.uv);
				float4 accum = tex2D(_AccumTex, i.uv);
				float4 revealage = tex2D(_RevealageTex, i.uv);
				float4 c1 = float4(accum.rgb / clamp(accum.a, 1e-4, 5e4), revealage.r);
				return (1.0 - c1.a) * c1 + c1.a * c0;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
