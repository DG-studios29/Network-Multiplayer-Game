Shader "Custom/WaterShader"
{
    Properties
    {
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveHeight ("Wave Height", Float) = 0.5
        _WaveLength ("Wave Length", Float) = 2.0
        _WaveDirection ("Wave Direction", Vector) = (1, 0, 0, 0)

        _ShallowColor ("Shallow Water Color", Color) = (0.0, 0.8, 1.0, 1)
        _DeepColor ("Deep Water Color", Color) = (0.0, 0.0, 0.4, 1)

        _FoamColor ("Foam Color", Color) = (1,1,1,1)
        _FoamThreshold ("Foam Threshold", Float) = 0.6
        _FoamBlend ("Foam Blend", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        float _WaveSpeed;
        float _WaveHeight;
        float _WaveLength;
        float4 _WaveDirection;

        fixed4 _ShallowColor;
        fixed4 _DeepColor;

        fixed4 _FoamColor;
        float _FoamThreshold;
        float _FoamBlend;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        float wave(float2 dir, float time, float3 worldPos)
        {
            float frequency = 2 * UNITY_PI / _WaveLength;
            float phase = frequency * dot(dir, worldPos.xz) + _WaveSpeed * time;
            return sin(phase) * _WaveHeight;
        }

        void vert(inout appdata_full v)
        {
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

            float time = _Time.y;
            float2 dir1 = normalize(_WaveDirection.xz);
            float2 dir2 = normalize(float2(-_WaveDirection.z, _WaveDirection.x));

            float w1 = wave(dir1, time, worldPos);
            float w2 = wave(dir2, time, worldPos);

            float waveHeight = w1 + w2;

            v.vertex.y += waveHeight;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float depth = saturate((IN.worldPos.y + 5.0) / 10.0);
            fixed4 col = lerp(_DeepColor, _ShallowColor, depth);

            // Simple foam effect based on steepness
            float2 dir1 = normalize(_WaveDirection.xz);
            float2 dir2 = normalize(float2(-_WaveDirection.z, _WaveDirection.x));

            float wave1 = wave(dir1, _Time.y, IN.worldPos);
            float wave2 = wave(dir2, _Time.y, IN.worldPos);

            float steepness = abs(ddx(wave1 + wave2)) + abs(ddy(wave1 + wave2));
            float foam = smoothstep(_FoamThreshold - _FoamBlend, _FoamThreshold + _FoamBlend, steepness);

            col = lerp(col, _FoamColor, foam);

            o.Albedo = col.rgb;
            o.Smoothness = 0.8;
            o.Metallic = 0.0;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
