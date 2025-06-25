Shader "Custom/WoodPanelShader"
{
    Properties
    {
        _Color ("Wood Color", Color) = (1,1,1,1)        
        _MainTex ("Wood Texture", 2D) = "white" {}       
        _BumpMap ("Normal Map", 2D) = "bump" {}          
        _Metallic ("Metallic", Range(0,1)) = 0.0        
        _Glossiness ("Smoothness", Range(0,1)) = 0.5    
        _WoodGrain ("Wood Grain Strength", Range(0, 1)) = 0.5 
        _NormalStrength ("Normal Strength", Range(0, 5)) = 1.0 // Nowa właściwość dla siły mapy normalnej
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        sampler2D _MainTex;  
        sampler2D _BumpMap;  
        half _Metallic;
        half _Glossiness;
        fixed4 _Color;
        half _WoodGrain;  
        half _NormalStrength;  // Nowa zmienna dla siły mapy normalnej

        struct Input
        {
            float2 uv_MainTex;   
            float2 uv_BumpMap;   
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // Zwiększenie wpływu mapy normalnej na każdą stronę
            float3 normalMap = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

            // Wzmocnienie wpływu mapy normalnej
            normalMap = normalize(normalMap) * _NormalStrength;  // Skalowanie mapy normalnej

            // Ustawienie zaktualizowanej normy
            o.Normal = normalMap;

            o.Albedo = c.rgb;

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
