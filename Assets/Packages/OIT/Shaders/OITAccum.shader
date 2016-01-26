Shader "Custom/OITAccum" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		//_Weight ("Weight Exponent", Range(-5.0, 0.0)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
		LOD 200
		Cull Off Lighting Off ZWrite Off ZTest LEqual Fog { Mode Off }
		Blend One One

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile PREMULTIPLIED_ALPHA_ON PREMULTIPLIED_ALPHA_OFF
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Weight;

			struct appdata {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};
			
			struct vs2ps {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;			
				float z : TEXCOORD1;
			};
			
			vs2ps vert(appdata i) {
				float4 v = mul(UNITY_MATRIX_MV, i.vertex);
			
				vs2ps o;
				o.vertex = mul(UNITY_MATRIX_P, v);
				//o.color = float4(i.color.rgb * i.color.a, i.color.a);
				o.color = i.color * i.color.a;
				o.uv = i.uv;
				o.z = abs(v.z);
				return o;
			}
			
			float w(float z) {
				return pow(z, _Weight);
			}
			
			float4 frag(vs2ps i) : COLOR {
				 float4 c = tex2D(_MainTex, i.uv);
				 #ifdef PREMULTIPLIED_ALPHA_OFF
				 c.rgb *= c.a;
				 #endif
				 return c * i.color * w(i.z);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
