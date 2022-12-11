Shader "Custom Standard"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _EmissionColor ("Emission Color", Color) = (0, 0, 0, 0)
        _MainTex ("Albedo", 2D) = "white" {}
        _Metallic ("Metallic", Range(0, 1)) = 0.0
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5

        _Brightness("Brightness", Range(0, 2)) = 1.0

        _RimColor("Rim Color", Color) = (1, 1, 1, 0.2)
        _RimIntensity("Rim Intensity", Range(0, 10)) = 0
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200
        
        CGPROGRAM
        
        #pragma target 3.0
        #pragma surface surf Standard fullforwardshadows addshadow

        fixed4 _Color;
        fixed4 _EmissionColor;
        sampler2D _MainTex;
        half _Metallic;
        half _Glossiness;

        half _Brightness;

        float4 _RimColor;
        float _RimIntensity;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
            float3 viewDir;
            float4 screenPos;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float BAYER8x8[64] =
            {
                0.000000, 0.500000, 0.125000, 0.625000, 0.031250, 0.500000, 0.156250, 0.656250,
                0.750000, 0.250000, 0.875000, 0.375000, 0.781250, 0.281250, 0.906250, 0.406250,
                0.187500, 0.687500, 0.062500, 0.562500, 0.218750, 0.718750, 0.093750, 0.593750,
                0.937500, 0.437500, 0.812500, 0.312500, 0.968750, 0.468750, 0.843750, 0.343750,
                0.046875, 0.546875, 0.171875, 0.671875, 0.015625, 0.515625, 0.140625, 0.640625,
                0.796875, 0.296875, 0.921875, 0.421875, 0.765625, 0.265625, 0.890625, 0.390625,
                0.234375, 0.734375, 0.109375, 0.609375, 0.203125, 0.703125, 0.078125, 0.578125,
                0.984375, 0.484375, 0.859375, 0.359375, 0.953125, 0.453125, 0.828125, 0.328125
            };

            float2 pos = (IN.screenPos.xy / IN.screenPos.w) * _ScreenParams.xy;

            int x = int(pos.x) % 8;
            int y = int(pos.y) % 8;
            int index = x * 8 + y;

            if (_Color.a - BAYER8x8[index] <= 0)
                discard;

            float rim = _RimIntensity > 0 ? pow(1 - dot(normalize(IN.viewDir), normalize(IN.worldNormal)), _RimIntensity) : 0;
            o.Albedo = lerp(tex2D(_MainTex, IN.uv_MainTex).rgb * _Color.rgb + (_Brightness - 1), _RimColor.rgb, rim * _RimColor.a);
            o.Emission = lerp(_EmissionColor.rgb * _EmissionColor.a, _RimColor.rgb, rim * _RimColor.a);
            o.Alpha = _Color.a;
            o.Metallic = _Metallic * _Metallic;
            o.Smoothness = _Glossiness;
        }
        
        ENDCG
    }

    FallBack "Diffuse"
}