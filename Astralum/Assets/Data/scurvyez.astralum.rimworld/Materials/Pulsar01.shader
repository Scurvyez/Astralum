Shader "Astralum/Pulsar01"
{
  Properties
  {
    _ShellDarkColor ("Shell Dark Color", Color) = (0.015, 0.03, 0.08, 1)
    _ShellBrightColor ("Shell Bright Color", Color) = (0.18, 0.55, 1.0, 1)
    _CoreColor ("Core Color", Color) = (1, 1, 1, 1)
    _JetColor ("Jet Color", Color) = (0.55, 0.85, 1.0, 1)

    _CanvasScale ("Canvas Scale", Range(1, 10)) = 1
    _Intensity ("Intensity", Range(0, 10)) = 1.25
    _Alpha ("Alpha", Range(0, 1)) = 0.85

    _CoreOffset ("Core Offset", Vector) = (0.06, -0.015, 0, 0)
    _CoreRadius ("Core Radius", Range(0.001, 0.15)) = 0.018
    _CoreGlowRadius ("Core Glow Radius", Range(0.01, 0.5)) = 0.12
    _CoreIntensity ("Core Intensity", Range(0, 20)) = 4
    _CorePulseSpeed ("Core Pulse Speed", Range(0, 20)) = 7
    _CorePulseStrength ("Core Pulse Strength", Range(0, 1)) = 0.2

    _ShellRadius ("Shell Radius", Range(0.05, 0.75)) = 0.32
    _ShellThickness ("Shell Thickness", Range(0.01, 0.35)) = 0.12
    _ShellSoftness ("Shell Softness", Range(0.01, 0.35)) = 0.10
    _ShellPower ("Shell Power", Range(0.5, 8)) = 2.25
    _ShellCoverage ("Shell Coverage", Range(0.05, 1)) = 0.75

    _InnerCrescentRadiusOffset ("Inner Crescent Radius Offset", Range(-0.4, 0.1)) = -0.11
    _InnerCrescentThickness ("Inner Crescent Thickness", Range(0.005, 0.25)) = 0.055
    _InnerCrescentSoftness ("Inner Crescent Softness", Range(0.005, 0.25)) = 0.09
    _InnerCrescentIntensity ("Inner Crescent Intensity", Range(0, 5)) = 1.25

    _BandRadiusOffset ("Inner Band Radius Offset", Range(-0.3, 0.3)) = -0.06
    _BandThickness ("Inner Band Thickness", Range(0.005, 0.2)) = 0.035
    _BandIntensity ("Inner Band Intensity", Range(0, 1)) = 0.35
    _BandSoftness ("Inner Band Softness", Range(0.01, 0.25)) = 0.08

    _JetLength ("Jet Length", Range(0.05, 1.5)) = 0.80
    _JetWidth ("Jet Width", Range(0.001, 0.25)) = 0.035
    _JetSpread ("Jet Spread", Range(0, 0.5)) = 0.18
    _JetIntensity ("Jet Intensity", Range(0, 10)) = 2.0
    _JetFalloff ("Jet Falloff", Range(0.2, 8)) = 2.8
    _JetAngle ("Jet Angle", Range(-3.14159, 3.14159)) = 0
    _JetFlicker ("Jet Flicker", Range(0, 1)) = 0.15
    _JetFlickerSpeed ("Jet Flicker Speed", Range(0, 20)) = 3
    _JetSoftness ("Jet Softness", Range(0.001, 0.5)) = 0.08

    _DustIntensity ("Concave Dust Intensity", Range(0, 2)) = 0.35
    _DustAmount ("Concave Dust Amount", Range(0, 1)) = 0.45
    _DustSpread ("Concave Dust Spread", Range(0.01, 0.6)) = 0.32

    _NoiseScale ("Noise Scale", Range(1, 40)) = 10
    _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.45
    _DetailScale ("Detail Scale", Range(1, 80)) = 24
    _DetailStrength ("Detail Strength", Range(0, 1)) = 0.18

    _FocusShimmer ("Focus Shimmer", Range(0, 1)) = 0
    _FocusShimmerColor ("Focus Shimmer Color", Color) = (0.65, 0.85, 1, 1)
    _FocusShimmerSpeed ("Focus Shimmer Speed", Range(0, 2)) = 0.3
    _FocusShimmerWidth ("Focus Shimmer Width", Range(0.001, 0.5)) = 0.02
    _FocusShimmerSoftness ("Focus Shimmer Softness", Range(0.001, 0.5)) = 0.4
    _FocusShimmerIntensity ("Focus Shimmer Intensity", Range(0, 10)) = 2
  }

  SubShader
  {
    Tags
    {
      "Queue" = "Background+7"
      "RenderType" = "Transparent"
    }

    Blend SrcAlpha One
    ZWrite Off
    ZTest LEqual
    Cull Off

    Pass
    {
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"
      #include "CelestialFocusShimmer.hlsl"

      fixed4 _ShellDarkColor;
      fixed4 _ShellBrightColor;
      fixed4 _CoreColor;
      fixed4 _JetColor;

      float _CanvasScale;
      float _Intensity;
      float _Alpha;

      float4 _CoreOffset;
      float _CoreRadius;
      float _CoreGlowRadius;
      float _CoreIntensity;
      float _CorePulseSpeed;
      float _CorePulseStrength;

      float _ShellRadius;
      float _ShellThickness;
      float _ShellSoftness;
      float _ShellPower;
      float _ShellCoverage;
      float _InnerCrescentRadiusOffset;
      float _InnerCrescentThickness;
      float _InnerCrescentSoftness;
      float _InnerCrescentIntensity;

      float _BandRadiusOffset;
      float _BandThickness;
      float _BandIntensity;
      float _BandSoftness;

      float _JetLength;
      float _JetWidth;
      float _JetSpread;
      float _JetIntensity;
      float _JetFalloff;
      float _JetAngle;
      float _JetFlicker;
      float _JetFlickerSpeed;
      float _JetSoftness;

      float _DustIntensity;
      float _DustAmount;
      float _DustSpread;

      float _NoiseScale;
      float _NoiseStrength;
      float _DetailScale;
      float _DetailStrength;

      float _FocusShimmer;
      fixed4 _FocusShimmerColor;
      float _FocusShimmerSpeed;
      float _FocusShimmerWidth;
      float _FocusShimmerSoftness;
      float _FocusShimmerIntensity;

      struct vertInput
      {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
      };

      struct vertOutput
      {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        float4 screenPos : TEXCOORD1;
      };

      float hash21(float2 p)
      {
        p = frac(p * float2(123.34, 456.21));
        p += dot(p, p + 45.32);
        return frac(p.x * p.y);
      }

      float noise(float2 uv)
      {
        float2 i = floor(uv);
        float2 f = frac(uv);

        float a = hash21(i);
        float b = hash21(i + float2(1, 0));
        float c = hash21(i + float2(0, 1));
        float d = hash21(i + float2(1, 1));

        float2 u = f * f * (3.0 - 2.0 * f);

        return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
      }

      float2 rotate_2d(float2 p, float angle)
      {
        float s = sin(angle);
        float c = cos(angle);

        return float2(p.x * c - p.y * s, p.x * s + p.y * c);
      }

      vertOutput vert(vertInput input)
      {
        vertOutput output;

        float4 world_pos = mul(unity_ObjectToWorld, input.vertex);
        world_pos.xyz += _WorldSpaceCameraPos;

        output.pos = mul(UNITY_MATRIX_VP, world_pos);
        output.uv = input.uv;
        output.screenPos = ComputeScreenPos(output.pos);

        return output;
      }

      fixed4 frag(vertOutput input) : SV_Target
      {
        float2 p = (input.uv - 0.5) * _CanvasScale;

        // rotate the whole effect so +X is always the convex/jet side
        float2 rp = rotate_2d(p, -_JetAngle);

        float shell_dist_from_center = length(rp);
        float angle = atan2(rp.y, rp.x);

        float2 core_p = rp - _CoreOffset.xy;
        float core_dist = length(core_p);

        float n1 = noise(input.uv * _NoiseScale + _Time.y * 0.05);
        float n2 = noise(input.uv * _DetailScale - _Time.y * 0.12);

        float texture_noise = lerp(1.0, n1, _NoiseStrength) * lerp(1.0, n2, _DetailStrength);

        // ------------------------------------------------------------
        // CORE: fixed at true disk center
        // ------------------------------------------------------------
        float pulse = 1.0 + sin(_Time.y * _CorePulseSpeed) * _CorePulseStrength;
        float core = 1.0 - smoothstep(_CoreRadius * pulse, _CoreRadius * pulse * 2.0, core_dist);
        float core_glow = 1.0 - smoothstep(_CoreRadius, _CoreGlowRadius * pulse, core_dist);

        // ------------------------------------------------------------
        // CRESCENT SHELL: partial ring around same center as core
        // Convex side is +X. Concave side is -X
        // ------------------------------------------------------------
        float shell_dist = abs(shell_dist_from_center - _ShellRadius);
        float shell = 1.0 - smoothstep(_ShellThickness, _ShellThickness + _ShellSoftness, shell_dist);

        // gap is centered on the concave side, opposite the jet
        // jet/convex side is +X, concave gap is -X
        float concave_angle = 3.14159265;

        // 1.0 coverage = full circle
        // 0.75 coverage = 25% missing gap
        float gap_angle = (1.0 - _ShellCoverage) * 6.2831853;
        float half_gap = gap_angle * 0.5;

        // angular distance from the concave/gap direction
        float angle_delta = abs(atan2(
          sin(angle - concave_angle),
          cos(angle - concave_angle)
        ));

        // hard-coded angular blur around the missing gap edges
        float gap_blur = 0.95;

        // 0 inside gap, softly fades to 1 outside gap
        float crescent_mask = smoothstep(half_gap - gap_blur, half_gap + gap_blur, angle_delta);

        shell *= crescent_mask;

        // ------------------------------------------------------------
        // INNER BRIGHT CRESCENT: extra matter/energy inside the shell,
        // strongest around the core side and curving with the shell.
        // ------------------------------------------------------------
        float inner_radius = _ShellRadius + _InnerCrescentRadiusOffset;
        float inner_dist = abs(shell_dist_from_center - inner_radius);

        float inner_crescent = 1.0 - smoothstep(
          _InnerCrescentThickness,
          _InnerCrescentThickness + _InnerCrescentSoftness,
          inner_dist
        );

        // Use same angular crescent coverage as the main shell.
        inner_crescent *= crescent_mask;

        // Bias brightness toward the convex/core side.
        float inner_convex_bias = smoothstep(-0.35, 0.85, rp.x / max(_ShellRadius, 0.001));

        inner_crescent *= inner_convex_bias;
        inner_crescent *= texture_noise;

        shell = pow(saturate(shell), _ShellPower);
        shell *= texture_noise;

        // ------------------------------------------------------------
        // INNER DIM BAND: faint darker partial band inside convex shell
        // this subtracts light from the convex side of the shell
        // ------------------------------------------------------------
        float band_radius = _ShellRadius + _BandRadiusOffset;
        float band_dist = abs(shell_dist_from_center - band_radius);

        float band = 1.0 - smoothstep(_BandThickness, _BandThickness + _BandSoftness, band_dist);

        band *= smoothstep(-0.15, 0.85, rp.x / max(_ShellRadius, 0.001));
        band *= _BandIntensity;
        shell *= saturate(1.0 - band);

        // ------------------------------------------------------------
        // JET: soft, smoky, cyclic beam from the core toward convex side
        // perpendicular to the direction of travel from the core
        // ------------------------------------------------------------
        float jet_along = saturate(core_p.x / max(_JetLength, 0.001));

        // starts at core, fades out near end.
        float jet_origin_fade = smoothstep(-_JetWidth * 2.0, _JetWidth * 3.0, core_p.x);
        float jet_end_fade = 1.0 - smoothstep(_JetLength * 0.72, _JetLength, core_p.x);

        float jet_active = jet_origin_fade * jet_end_fade;

        // slow perpendicular undulation
        // this fakes the side-view “rotating hose” effect without becoming a harsh squiggle
        float center_wave =
          sin(jet_along * 7.0 + _Time.y * 0.85) * 0.030 +
          sin(jet_along * 13.0 - _Time.y * 0.45) * 0.014;

        // slight skew as it travels outward
        float skew = jet_along * 0.045;

        // curved centerline of the jet
        float jet_center_y = center_wave + skew;

        // keep width mostly controlled, not a strong V
        // it widens slightly, then softens into a smoky tail
        float jet_width = _JetWidth + _JetSpread * 0.35 * smoothstep(0.15, 1.0, jet_along);
        float jet_across = abs(core_p.y - jet_center_y);

        // soft core + broader smoky envelope
        float jet_inner = 1.0 - smoothstep(jet_width * 0.25, jet_width + _JetSoftness, jet_across);
        float jet_outer = 1.0 - smoothstep(jet_width, jet_width * 3.8 + _JetSoftness * 4.0, jet_across);

        // moving noise along the jet axis
        float jet_noise_a = noise(float2(
          jet_along * 18.0 - _Time.y * 0.65,
          core_p.y * 22.0 + _Time.y * 0.18
        ));

        float jet_noise_b = noise(float2(
          jet_along * 42.0 + _Time.y * 0.35,
          core_p.y * 35.0 - _Time.y * 0.28
        ));

        float jet_breakup = lerp(1.0, jet_noise_a, 0.45) * lerp(1.0, jet_noise_b, 0.25);

        // subtle cyclic brightness bands moving outward
        float flow_bands = 0.72 + 0.28 * sin(jet_along * 22.0 - _Time.y * 2.0);

        // fade with distance, but less aggressively than before
        float jet_fade = pow(saturate(1.0 - jet_along), _JetFalloff);

        // combine inner brightness with smoky envelope
        float jet_shape = saturate(jet_inner * 0.42 + jet_outer * 0.58);

        float jet_flicker = lerp(1.0,
                                           0.88 + 0.12 * noise(float2(jet_along * 10.0, _Time.y * _JetFlickerSpeed)),
                                           _JetFlicker);

        float jet = jet_shape * jet_active * jet_fade * jet_breakup * flow_bands * jet_flicker;
        float origin_plume = 1.0 - smoothstep(_JetWidth * 0.5, _JetWidth * 5.0 + _JetSoftness, length(core_p));
        jet += origin_plume * 0.18;

        // ------------------------------------------------------------
        // SECONDARY REVERSE JET:
        // shorter, dimmer jet emitted in the opposite direction
        // ------------------------------------------------------------

        // mirror the jet axis
        float reverse_along = saturate((-core_p.x) / max(_JetLength * 0.5, 0.001));

        float reverse_active = smoothstep(-_JetWidth * 2.0, _JetWidth * 3.0, -core_p.x) *
        (1.0 - smoothstep(
          (_JetLength * 0.5) * 0.72,
          (_JetLength * 0.5),
          -core_p.x));

        // reverse-side undulation
        float reverse_wave =
          sin(reverse_along * 6.5 + _Time.y * 0.75) * 0.020 +
          sin(reverse_along * 11.0 - _Time.y * 0.35) * 0.010;

        // opposite skew direction
        float reverse_skew = -reverse_along * 0.025;
        float reverse_center_y = reverse_wave + reverse_skew;

        float reverse_width =
          (_JetWidth * 0.85) +
          (_JetSpread * 0.18) *
          smoothstep(0.15, 1.0, reverse_along);

        float reverse_across = abs(core_p.y - reverse_center_y);

        float reverse_inner = 1.0 - smoothstep(reverse_width * 0.25, reverse_width + _JetSoftness, reverse_across);
        float reverse_outer = 1.0 - smoothstep(reverse_width, reverse_width * 3.4 + _JetSoftness * 4.0,
             reverse_across);

        float reverse_noise_a = noise(float2(reverse_along * 16.0 + _Time.y * 0.45,
                                                        core_p.y * 20.0 - _Time.y * 0.15));
        float reverse_noise_b = noise(float2(reverse_along * 32.0 - _Time.y * 0.25,
                                                     core_p.y * 28.0 + _Time.y * 0.22));

        float reverse_breakup =
          lerp(1.0, reverse_noise_a, 0.35) *
          lerp(1.0, reverse_noise_b, 0.18);

        float reverse_bands = 0.78 + 0.22 * sin(reverse_along * 18.0 - _Time.y * 1.5);
        float reverse_fade = pow(saturate(1.0 - reverse_along), _JetFalloff * 1.35);
        float reverse_shape = saturate(reverse_inner * 0.38 + reverse_outer * 0.62);

        float reverse_jet = reverse_shape * reverse_active * reverse_fade * reverse_breakup *
          reverse_bands * jet_flicker;

        // brighter than the primary jet
        reverse_jet *= 1.5;

        // ------------------------------------------------------------
        // CONCAVE DUST/WISPS: subtle particles on -X side(?) of shell
        // ------------------------------------------------------------
        float concave_side = smoothstep(0.15, -0.75, rp.x / max(_ShellRadius, 0.001));

        float dust_ring = 1.0 - smoothstep(_DustSpread, _DustSpread + 0.18, shell_dist);
        float dust_breakup = smoothstep(1.0 - _DustAmount, 1.0, n1);
        float dust = concave_side * dust_ring * dust_breakup * _DustIntensity;

        // ------------------------------------------------------------
        // let's tie it all together...
        // ------------------------------------------------------------
        float shell_alpha = shell * 0.75;
        float inner_crescent_alpha = inner_crescent * 0.55 * _InnerCrescentIntensity;
        float dust_alpha = dust * 0.35;
        float jet_alpha = jet * 0.8;
        float reverse_jet_alpha = reverse_jet * 0.65;
        float core_alpha = saturate(core + core_glow * 0.45);

        float3 shell_color = lerp(_ShellDarkColor.rgb, _ShellBrightColor.rgb, saturate(shell + dust));

        float3 color =
          shell_color * shell_alpha +
          _ShellBrightColor.rgb * inner_crescent_alpha +
          shell_color * dust_alpha +
          _JetColor.rgb * jet_alpha * _JetIntensity +
          _JetColor.rgb * reverse_jet_alpha * (_JetIntensity * 0.75) +
          _CoreColor.rgb * core_alpha * _CoreIntensity;

        float alpha = saturate(
          shell_alpha +
          inner_crescent_alpha +
          dust_alpha +
          jet_alpha +
          reverse_jet_alpha +
          core_alpha
        ) * _Alpha;

        float2 screen_uv = input.screenPos.xy / input.screenPos.w;

        float shimmer = astralum_focus_shimmer(
          screen_uv,
          _Time.y,
          _FocusShimmerSpeed,
          _FocusShimmerWidth,
          _FocusShimmerSoftness
        );

        shimmer *= alpha;
        shimmer *= _FocusShimmer;

        float3 final_color = color * _Intensity;
        final_color += _FocusShimmerColor.rgb * shimmer * _FocusShimmerIntensity;

        return fixed4(final_color, alpha);
      }
      ENDHLSL
    }
  }
}