Shader "Sprites/Dream Fall Lurch Smear"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // Driven from script. You can go high (0.25 - 0.35) for intense lurch.
        _Stretch ("Smear Amount", Range(0, 0.35)) = 0

        // How fast it slams in. Higher = reaches full smear earlier.
        _Strength ("Strength", Range(0, 200)) = 45

        // Controls how far "from below" we pull in UV space.
        _PullSpan ("Pull Span", Range(0, 10)) = 4.0

        // Ghost echoes: discrete copies (like the very first shader taps).
        _GhostCount ("Ghost Count", Range(0, 8)) = 4
        _GhostSpacing ("Ghost Spacing", Range(0, 6)) = 2.5
        _GhostHardness ("Ghost Hardness", Range(0.1, 4)) = 1.7

        // Dream wobble (kept subtle by default). Set to 0 to disable.
        _Jitter ("Jitter", Range(0, 0.02)) = 0.004
        _JitterSpeed ("Jitter Speed", Range(0, 10)) = 2.0

        // Optional edge mask: 0 = full screen, >0 focuses toward center.
        _EdgeFade ("Edge Fade", Range(0, 4)) = 0.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Sprite"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            float _Stretch, _Strength, _PullSpan;
            float _GhostCount, _GhostSpacing, _GhostHardness;
            float _Jitter, _JitterSpeed, _EdgeFade;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            // Premultiplied sample for SpriteRenderer blend mode (One, OneMinusSrcAlpha)
            fixed4 SampleSprite(float2 uv)
            {
                fixed4 c = tex2D(_MainTex, uv);
                c.rgb *= c.a;
                return c;
            }

            // Cheap hash for stable per-pixel jitter
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                fixed4 baseCol = SampleSprite(uv) * i.color;

                // Optional edge focus
                float edgeMask = 1.0;
                if (_EdgeFade > 0.001)
                {
                    float2 d = abs(uv - 0.5) * 2.0;      // 0 center -> 1 edges
                    float m = saturate(1.0 - max(d.x, d.y));
                    edgeMask = pow(m, _EdgeFade);
                }

                // Intensity curve: slam in hard as _Stretch increases
                float a = saturate(_Stretch * _Strength);
                a = pow(a, 0.35);        // aggressive ramp (hits 1 quickly)
                a *= edgeMask;

                // Pull distance (UV space). Larger = more violent length.
                float pull = _Stretch * _PullSpan;

                // Dream jitter (vertical only) — still "pull from below" (never sample above)
                float ttime = _Time.y * _JitterSpeed;
                float n = hash21(uv * 80.0 + ttime);
                float jitter = (n - 0.5) * 2.0 * _Jitter; // [-_Jitter, +_Jitter]

                // ----- 1) Strong "pull from below" smear (keeps color consistent) -----
                // We sample BELOW current uv.y and take MAX to keep it punchy.
                fixed4 smear = baseCol;

                // A few fixed taps, spaced increasingly, to create a strong drag
                float2 sUV1 = float2(uv.x, max(0.0, uv.y - pull * 0.35 + jitter));
                float2 sUV2 = float2(uv.x, max(0.0, uv.y - pull * 0.85 + jitter));
                float2 sUV3 = float2(uv.x, max(0.0, uv.y - pull * 1.60 + jitter));

                fixed4 p1 = SampleSprite(sUV1) * i.color;
                fixed4 p2 = SampleSprite(sUV2) * i.color;
                fixed4 p3 = SampleSprite(sUV3) * i.color;

                smear = max(smear, p1);
                smear = max(smear, p2);
                smear = max(smear, p3);

                // ----- 2) Discrete "ghost taps" (rollercoaster echoes) -----
                // These are stamped copies with larger spacing, for obvious repeats.
                int gcount = clamp((int)_GhostCount, 0, 8);
                if (gcount > 0)
                {
                    // Ghost spacing grows with pull and a separate multiplier
                    float gStep = pull * _GhostSpacing;

                    // We'll accumulate ghosts with MAX (keeps intensity)
                    // Also apply a "hardness" curve so they stay visible.
                    for (int g = 1; g <= gcount; g++)
                    {
                        float gy = max(0.0, uv.y - gStep * g);
                        float2 guv = float2(uv.x, gy);

                        fixed4 gc = SampleSprite(guv) * i.color;

                        // Harden ghost alpha a bit so they read as distinct layers
                        float hardA = saturate(pow(gc.a, _GhostHardness));
                        gc.rgb = gc.rgb * (hardA / max(gc.a, 1e-5));
                        gc.a = hardA;

                        smear = max(smear, gc);
                    }
                }

                // Final blend: base -> smear (violent lurch)
                // Using lerp with a (0..1) but the smear itself is already "maxed" to be punchy.
                return lerp(baseCol, smear, a);
            }
            ENDCG
        }
    }
}
