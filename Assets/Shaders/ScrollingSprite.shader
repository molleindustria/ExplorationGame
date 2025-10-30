Shader "ScrollingTexture" {
    Properties{
        _MainTex("Base (RGB)", 2D) = "white" {}
        _TextureScrollX("Texture X Scroll", Range(-10, 10)) = 0
        _TextureScrollY("Texture Y Scroll", Range(-10, 10)) = 0

    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 200
            Cull Off
            CGPROGRAM
            #pragma surface surf Lambert

            sampler2D _MainTex;
            float _TextureScrollX;
            float _TextureScrollY;


            struct Input {
                float2 uv_MainTex;
                float3 uv_NormalTex;
                float3 vertColor;
            };

            void surf(Input IN, inout SurfaceOutput o) {
                float2 scrolledUV = IN.uv_MainTex;
                float scrollSpeedX = _TextureScrollX * _Time;
                float scrollSpeedY = _TextureScrollY * _Time;

                scrolledUV += float2(scrollSpeedX, scrollSpeedY);

                half4 c = tex2D(_MainTex, scrolledUV);
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
