Shader "Custom/MyCharacter" {
     Properties {
         _MainTex ("Base (RGB)", 2D) = "white" {}
         _BGTex ("BGTex(RGB)", 2D) = "white" {}
     }
     SubShader {
         Tags { "RenderType"="Opaque" }
         LOD 200
         
         CGPROGRAM
         #pragma surface surf Lambert
 
         sampler2D _MainTex;
         sampler2D _BGTex;
 
         struct Input {
             float2 uv_MainTex;
         };
 
         void surf (Input IN, inout SurfaceOutput o) {
             half4 c1 = tex2D (_MainTex, IN.uv_MainTex);
             half4 c2 = tex2D (_BGTex, IN.uv_MainTex);
             o.Albedo = lerp(c2.rgb, c1.rgb, c1.r);
             o.Albedo = lerp(o.Albedo, c1.rgb, c1.b);
             o.Albedo = lerp(o.Albedo, c1.rgb, c1.g);
             //o.Albedo = c1.rgb;
             o.Alpha = c1.a;
         }
         ENDCG
     } 
     FallBack "Diffuse"
 }