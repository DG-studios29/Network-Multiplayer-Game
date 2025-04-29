Shader "Custom/FoamShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _FoamTex ("Foam Texture", 2D) = "white" { }
        _FoamStrength ("Foam Strength", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _FoamTex;
            float _FoamStrength;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float foam = tex2D(_FoamTex, uv).r * _FoamStrength;
                half4 baseColor = tex2D(_MainTex, uv);
                return baseColor + foam;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
