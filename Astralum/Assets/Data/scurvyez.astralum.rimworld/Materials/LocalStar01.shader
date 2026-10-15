Shader "Astralum/LocalStar01"
{
  Properties
  {
    _Chromaticity ("Chromaticity", Color) = (1,1,1,1)
    _Corona ("Corona", Color) = (1,1,1,1)

    _SurfaceNoiseStrength ("Surface Noise Strength", Range(0,5)) = 0.025
    _CoronaRotationSpeed ("Corona Rotation Speed", Range(-5,5)) = 0

    _ChromaticityIntensity ("Chromaticity Intensity", Range(0,2)) = 1
    _CoronaIntensity ("Corona Intensity", Range(0,2)) = 1
    _OuterCoronaIntensity ("Outer Corona Intensity", Range(0,2)) = 0.25

    _ChromaticityFalloffPower ("Chromaticity Falloff Power", Range(0,10)) = 2
    _CoronaPower ("Corona Power", Range(0,10)) = 5
    _OuterCoronaPower ("Outer Corona Power", Range(0,10)) = 6

    _VariabilityAmount ("Variability Amount", Range(0,0.5)) = 0
    _VariabilitySpeed ("Variability Speed", Range(0,5)) = 1

    _ScalingFactor ("Scaling Factor", Range(0,2)) = 0.65

    _FocusShimmer ("Focus Shimmer", Range(0, 1)) = 0
    _FocusShimmerColor ("Focus Shimmer Color", Color) = (0.65, 0.85, 1, 1)
    _FocusShimmerSpeed ("Focus Shimmer Speed", Range(0, 2)) = 0.3
    _FocusShimmerWidth ("Focus Shimmer Width", Range(0.001, 0.5)) = 0.02
    _FocusShimmerSoftness ("Focus Shimmer Softness", Range(0.001, 0.5)) = 0.4
    _FocusShimmerIntensity ("Focus Shimmer Intensity", Range(0, 10)) = 10
  }

  SubShader
  {
    Tags
    {
      "Queue" = "Background+20"
      "RenderType" = "Transparent"
    }

    Cull Off
    ZWrite Off
    ZClip On

    // ------------------------------------------------------------
    // PASS 1: STAR CORE / CHROMATICITY
    // Normal alpha blending so the core does not additively inflate
    // against bright sky/background colors.
    // ------------------------------------------------------------
    Pass
    {
      Name "Core"
      Blend SrcAlpha OneMinusSrcAlpha

      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragCore
      #include "UnityCG.cginc"

      fixed4 _Chromaticity;
      float _SurfaceNoiseStrength;
      float _ChromaticityIntensity;
      float _ChromaticityFalloffPower;
      float _ScalingFactor;

      struct vertInput
      {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
      };

      struct vertOutput
      {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
      };

      vertOutput vert(vertInput input)
      {
        vertOutput output;

        float4 world_pos = mul(unity_ObjectToWorld, input.vertex);
        world_pos.xyz += _WorldSpaceCameraPos;

        output.pos = mul(UNITY_MATRIX_VP, world_pos);
        output.uv = input.uv;

        return output;
      }

      fixed4 fragCore(vertOutput input) : SV_Target
      {
        float2 centered_uv = input.uv - float2(0.5, 0.5);
        centered_uv /= max(_ScalingFactor, 0.0001);

        float dist = length(centered_uv);
        float base_radial = saturate(1.0 - dist * 2.0);

        float chromaticity_mask = pow(base_radial, _ChromaticityFalloffPower);

        float3 chromaticity = _Chromaticity.rgb * chromaticity_mask * _ChromaticityIntensity;

        float noise = frac(sin(dot(input.uv * 128.0, float2(12.9898, 78.233))) * 43758.5453);
        noise = lerp(1.0 - _SurfaceNoiseStrength, 1.0 + _SurfaceNoiseStrength, noise);

        chromaticity *= noise;

        float alpha = saturate(chromaticity_mask * _Chromaticity.a);

        return fixed4(saturate(chromaticity), alpha);
      }
      ENDHLSL
    }

    // ------------------------------------------------------------
    // PASS 2: CORONA / OUTER GLOW
    // Screen-ish blending. Less explosive than pure additive on bright
    // backgrounds, while still feeling like glow.
    // ------------------------------------------------------------
    Pass
    {
      Name "Corona"
      Blend One OneMinusSrcColor

      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment fragCorona
      #include "UnityCG.cginc"
      #include "CelestialFocusShimmer.hlsl"

      fixed4 _Corona;

      float _CoronaRotationSpeed;
      float _CoronaIntensity;
      float _OuterCoronaIntensity;
      float _CoronaPower;
      float _OuterCoronaPower;
      float _VariabilityAmount;
      float _VariabilitySpeed;
      float _ScalingFactor;

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

      float hash21(float2 p)
      {
        return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
      }

      float value_noise(float2 uv)
      {
        float2 i = floor(uv);
        float2 f = frac(uv);

        float a = hash21(i);
        float b = hash21(i + float2(1.0, 0.0));
        float c = hash21(i + float2(0.0, 1.0));
        float d = hash21(i + float2(1.0, 1.0));

        float2 u = f * f * (3.0 - 2.0 * f);

        return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
      }

      fixed4 fragCorona(vertOutput input) : SV_Target
      {
        float2 centered_uv = input.uv - float2(0.5, 0.5);
        centered_uv /= max(_ScalingFactor, 0.0001);

        float dist = length(centered_uv);
        float base_radial = saturate(1.0 - dist * 2.0);

        float corona_mask = pow(base_radial, _CoronaPower);

        float outer_halo = saturate(1.0 - dist * 1.15);
        outer_halo = pow(outer_halo, _OuterCoronaPower);

        float variability_pulse = 1.0 + sin(_Time.y * _VariabilitySpeed) * _VariabilityAmount;

        float sphere_mask = saturate(1.0 - dist * 2.0);

        float projected_depth = sqrt(saturate(1.0 - dot(centered_uv * 2.0, centered_uv * 2.0)));

        float visual_day_length_seconds = 240.0;
        float visual_rotation_scale = 0.1;

        float rotations_per_second = _CoronaRotationSpeed / visual_day_length_seconds;
        float flow = _Time.y * rotations_per_second * visual_rotation_scale;

        float surface_x = centered_uv.x;
        float surface_y = centered_uv.y / max(projected_depth, 0.15);

        float2 corona_flow_uv = float2(surface_x * 2.0, surface_y + flow);
        float corona_noise_a = value_noise(corona_flow_uv * 5.0);
        float corona_noise_b = value_noise(corona_flow_uv * 11.0 + 17.3);

        float corona_variation = lerp(corona_noise_a, corona_noise_b, 0.35);
        corona_variation = lerp(0.85, 1.15, corona_variation);
        corona_variation = lerp(1.0, corona_variation, sphere_mask);

        float3 corona = _Corona.rgb * corona_mask * _CoronaIntensity * variability_pulse * corona_variation;
        float3 outer_corona = _Corona.rgb * outer_halo * _OuterCoronaIntensity * variability_pulse * corona_variation;
        float visible_corona_mask = saturate(corona_mask * _CoronaIntensity + outer_halo * _OuterCoronaIntensity);

        float3 final_color = corona + outer_corona;

        float2 screen_uv = input.screenPos.xy / input.screenPos.w;

        float shimmer = astralum_focus_shimmer(
          screen_uv,
          _Time.y,
          _FocusShimmerSpeed,
          _FocusShimmerWidth,
          _FocusShimmerSoftness);

        shimmer *= _FocusShimmer;
        shimmer *= visible_corona_mask;

        float shimmer_strength = saturate(shimmer * _FocusShimmerIntensity);
        float3 shimmer_target = max(final_color, _FocusShimmerColor.rgb);

        final_color = lerp(final_color, shimmer_target, shimmer_strength);

        float alpha = visible_corona_mask * _Corona.a;

        return fixed4(saturate(final_color), alpha);
      }
      ENDHLSL
    }
  }
}