Shader "Custom/StrokeNoiseShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _StrokeTex ("Stroke", 2D) = "white" {}
        _SwirlTex ("Swirl", 2D) = "white" {}
        _NoiseTex ("Noise", 2D) = "white" {}
        _BrushTex ("Brush", 2D) = "white" {}

        _ZoomStroke ("ZoomStroke", Range(0, 1)) = 1
        _ZoomSwirl ("ZoomSwirl", Range(0, 1)) = 1
        _ZoomBrush ("ZoomBrush", Range(0, 1)) = 1

        //_Blur ("Blur", Range(0, 100)) = 1
        _SecondaryBlur ("SecondaryBlur", Range(0, 200)) = 1

        _HueEffect ("Hue", Range(0, 1)) = 0
        _SaturationEffect ("Saturation", Range(0, 1)) = 0

        _Discretize ("Discretize", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            uniform sampler2D _StrokeTex;
            uniform sampler2D _SwirlTex;
            uniform sampler2D _NoiseTex;
            uniform sampler2D _BrushTex;
            
            fixed2 _DevStroke;
            fixed2 _DevSwirl;
            fixed2 _DevBrush0;
            fixed2 _DevBrush1;

            fixed _ZoomStroke;
            fixed _ZoomSwirl;
            fixed _ZoomBrush;

            fixed _HueEffect;
            fixed _SaturationEffect;

            fixed _BrushPrgs = 0;

            //fixed _Blur;
            fixed _SecondaryBlur;

            fixed _Discretize;

            fixed random (fixed2 uv)
            {
                return frac(sin(dot(uv, fixed2(12.9898, 78.233))) * 43758.5453123);
            }

            fixed fade (fixed t){
                return t * t * t * (t * (t * 6 - 15) + 10);
            }

            // All components are in the range [0…1], including hue.
            fixed3 rgb2hsv(fixed3 c)
            {
                fixed4 K = fixed4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                fixed4 p = lerp(fixed4(c.bg, K.wz), fixed4(c.gb, K.xy), step(c.b, c.g));
                fixed4 q = lerp(fixed4(p.xyw, c.r), fixed4(c.r, p.yzx), step(p.x, c.r));

                fixed d = q.x - min(q.w, q.y);
                fixed e = 1.0e-10;
                return fixed3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            // All components are in the range [0…1], including hue.
            fixed3 hsv2rgb(fixed3 c)
            {
                fixed4 K = fixed4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                fixed3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            fixed2 zigzag(fixed2 uv){
                return fixed2(
                    -1*frac(uv.x)*(2 * (floor(uv.x) % 2) - 1) + (floor(uv.x) % 2),
                    -1*frac(uv.y)*(2 * (floor(uv.y) % 2) - 1) + (floor(uv.y) % 2)
                    );
            }

            fixed4 frag (v2f_img i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                fixed2 uv_ = zigzag(i.uv * _ZoomStroke + _DevStroke * _ZoomStroke); //for stroke
                fixed2 uv__ = zigzag(i.uv * _ZoomSwirl + _DevSwirl * _ZoomSwirl); //for swirl
                fixed2 uv___front = zigzag(i.uv * _ZoomBrush  + _DevBrush0);
                fixed2 uv___back = zigzag(i.uv * _ZoomBrush  + _DevBrush1);

                fixed4 stroke = tex2D(_StrokeTex, uv_);
                fixed4 swirl = tex2D(_SwirlTex, uv__);
                fixed4 noise = tex2D(_NoiseTex, i.uv);

                fixed4 brush0 = tex2D(_BrushTex, uv___front);
                fixed4 brush1 = tex2D(_BrushTex, uv___back);
                fixed4 brush = lerp(brush0, brush1, fade(_BrushPrgs));

                fixed4 modifier = stroke * swirl * brush * noise;

                //secondary blur
                fixed offset_ = _SecondaryBlur * (modifier - 0.5) * 1.0 / _ScreenParams.xy;
                fixed4 col_n1 = tex2D(_MainTex, i.uv + fixed2(offset_, 0));
                fixed4 col_n2 = tex2D(_MainTex, i.uv + fixed2(0, offset_));
                fixed4 col_n3 = tex2D(_MainTex, i.uv + fixed2(-1 * offset_, offset_));
                fixed4 col_n4 = tex2D(_MainTex, i.uv + fixed2(offset_, offset_));
                
                col.rgb = lerp(col.rgb, col_n1.rgb, modifier.r);
                col.rgb = lerp(col.rgb, col_n2.rgb, 1 - modifier.r);
                col.rgb = lerp(col.rgb, col_n3.rgb, modifier.r);
                col.rgb = lerp(col.rgb, col_n4.rgb, 1 - modifier.r);

                //hue
                fixed3 hsv = rgb2hsv(col.rgb);

                hsv.x += lerp(
                    hsv.x, 
                    clamp(
                        hsv.x + _HueEffect * floor(0.5 + brush),
                        0,
                        1
                        ),
                    brush); //strokes have slight effect on hue
                hsv.y = lerp(
                    hsv.y,
                    clamp(
                        hsv.x + _SaturationEffect,
                        0,
                        1
                        ),
                    (1 - pow(brush, 0.5))); //strokes have increasing effect on saturation
                col.rgb = hsv2rgb(hsv);

                //use step function to discretize color
                //the smaller the discretize variable, the more saturation levels there could be 
                col.r -= col.r % _Discretize;
                col.g -= col.g % _Discretize;
                col.b -= col.b % _Discretize;
                /*
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
                    ) / 9;*/

                return col;
            }
            ENDCG
        }
    }
}