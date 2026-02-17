Shader "Hidden/DarknessVision"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Darkness ("Darkness", Range(0, 1)) = 1
        _Ambient ("Ambient Visibility", Range(0, 1)) = 0
        _EdgeSoftness ("Edge Softness", Range(0.01, 2)) = 0.35
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Darkness;
            float _Ambient;
            float _EdgeSoftness;
            float _Aspect;
            int _EmitterCount;
            float4 _Emitters[64]; // xy = viewport pos, z = viewport radius, w = intensity

            fixed4 frag(v2f_img i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                float visibility = _Ambient;

                [loop]
                for (int index = 0; index < 64; index++)
                {
                    if (index >= _EmitterCount)
                    {
                        break;
                    }

                    float4 emitter = _Emitters[index];
                    float2 delta = i.uv - emitter.xy;
                    delta.x *= _Aspect;

                    float distanceToEmitter = length(delta);
                    float radius = max(emitter.z, 0.00001);
                    float feather = max(radius * _EdgeSoftness, 0.00001);

                    float emitterVisibility = 1.0 - smoothstep(radius, radius + feather, distanceToEmitter);
                    visibility = max(visibility, saturate(emitterVisibility * emitter.w));
                }

                visibility = saturate(visibility);
                float brightness = lerp(1.0 - _Darkness, 1.0, visibility);
                color.rgb *= brightness;

                return color;
            }
            ENDCG
        }
    }

    Fallback Off
}
