Shader "Custom/FractalNoise"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)

        _f0 ("F0", 2D) = "white" {}
        _f1 ("F1", 2D) = "white" {}
        _f2 ("F2", 2D) = "white" {}
        _f3 ("F3", 2D) = "white" {}

		freq0 ("Fr0", Float) = 100000000
		freq1 ("Fr1", Float) = 100000000
		freq2 ("Fr2", Float) = 100000000
		freq3 ("Fr3", Float) = 100000000
    }
    SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform sampler2D _f0;
            uniform sampler2D _f1;
            uniform sampler2D _f2;
			uniform sampler2D _f3;

			uniform float freq0;
			uniform float freq1;
			uniform float freq2;
			uniform float freq3;

			float4 frag(v2f_img i) : COLOR {
                float4 f0 = tex2D(_f0, i.uv);
				float4 f1 = tex2D(_f1, i.uv);
				float4 f2 = tex2D(_f2, i.uv);
				float4 f3 = tex2D(_f3, i.uv);
				
				float4 result = f0 * 1 / freq0
					+ f1 * 1 / freq1
					+ f2 * 1 / freq2
					+ f3 * 1 / freq3;
				result /= (1/freq0 + 1/freq1 + 1/freq2 + 1/freq3);
				result *= tex2D(_MainTex, i.uv);

				return result;
			}
			ENDCG
		}
	}
}
