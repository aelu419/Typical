Shader "Unlit/BlurShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Blur ("Blur", Range(0,20)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass{
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            
            fixed _Blur;

            fixed4 frag (v2f_img i) : SV_Target
            {
                
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                //blur effect
                //       magnitude of blurring
                fixed offset = _Blur / _ScreenParams.xy;
                fixed4 n1 = tex2D(_MainTex, i.uv + fixed2(offset, 0));
                fixed4 n2 = tex2D(_MainTex, i.uv + fixed2(-1 * offset, 0));
                fixed4 n3 = tex2D(_MainTex, i.uv + fixed2(offset, offset));
                fixed4 n4 = tex2D(_MainTex, i.uv + fixed2(0, offset));
                fixed4 n5 = tex2D(_MainTex, i.uv + fixed2(-1 * offset, offset));
                fixed4 n6 = tex2D(_MainTex, i.uv + fixed2(offset, -1 * offset));
                fixed4 n7 = tex2D(_MainTex, i.uv + fixed2(0, -1 * offset));
                fixed4 n8 = tex2D(_MainTex, i.uv + fixed2(-1 * offset, -1 * offset));

                col.rgb = (
                        col.rgb + n1.rgb + n2.rgb + n3.rgb + n4.rgb + n5.rgb + n6.rgb + n7.rgb + n8.rgb
                    ) / 9;
                
                return col;
            }
            ENDCG
        }
    }
}
