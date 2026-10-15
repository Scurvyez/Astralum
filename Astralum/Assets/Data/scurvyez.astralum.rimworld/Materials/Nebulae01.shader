Shader "Astralum/Nebulae01"
{
  Properties
  {
    _ColorA ("Color A / Outer Dust", Color) = (0.12, 0.18, 0.35, 1)
    _ColorB ("Color B / Main Cloud", Color) = (0.25, 0.45, 1.0, 1)
    _ColorC ("Color C / Hot Knots", Color) = (0.85, 0.25, 0.95, 1)
    _ColorD ("Color D / Bright Core", Color) = (1.0, 0.75, 0.45, 1)

    _ColorStopB ("Color Stop B", Range(0.01, 0.99)) = 0.33
    _ColorStopC ("Color Stop C", Range(0.01, 0.99)) = 0.66
    _ColorBandSharpness ("Color Band Sharpness", Range(0.25, 8)) = 1

    _SeedOffset ("Seed Offset", Vector) = (0,0,0,0)
    _Seed ("Seed", Float) = 0

    _Intensity ("Intensity", Range(0, 5)) = 1
    _Alpha ("Alpha", Range(0, 1)) = 0.35

    _NoiseScale ("Noise Scale", Range(0.5, 20)) = 5
    _NoiseStrength ("Noise Strength", Range(0, 1)) = 1
    _CloudThreshold ("Cloud Threshold", Range(0, 1)) = 0.35
    _EdgeSoftness ("Edge Softness", Range(0.01, 1)) = 0.35

    _WarpScale ("Warp Scale", Range(0.1, 20)) = 3
    _WarpStrength ("Warp Strength", Range(0, 10)) = 0.35
    _ShapePower ("Shape Power", Range(0.25, 6)) = 1.5
    _CoreOffset ("Core Offset", Vector) = (0, 0, 0, 0)

    _StretchX ("Stretch X", Range(0.25, 4)) = 1.8
    _StretchY ("Stretch Y", Range(0.25, 4)) = 0.85
    _Rotation ("Rotation", Range(0, 6.28318)) = 0

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
      "Queue" = "Background+3"
      "RenderType" = "Transparent"
    }

    Pass
    {
      Blend SrcAlpha One
      ZWrite Off
      ZTest LEqual
      Cull Off

      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"
      #include "CelestialFocusShimmer.hlsl"

      fixed4 _ColorA;
      fixed4 _ColorB;
      fixed4 _ColorC;
      fixed4 _ColorD;

      float _ColorStopB;
      float _ColorStopC;
      float _ColorBandSharpness;

      float4 _SeedOffset;
      float _Seed;

      float _Intensity;
      float _Alpha;

      float _NoiseScale;
      float _NoiseStrength;
      float _CloudThreshold;
      float _EdgeSoftness;

      float _WarpScale;
      float _WarpStrength;
      float _ShapePower;
      float4 _CoreOffset;

      float _StretchX;
      float _StretchY;
      float _Rotation;

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

      uint hash_uint(uint x)
      {
        x ^= x >> 16;
        x *= 0x7FEB352Du;
        x ^= x >> 15;
        x *= 0x846CA68Bu;
        x ^= x >> 16;

        return x;
      }

      float hash_lattice(int2 p)
      {
        uint x = asuint(p.x);
        uint y = asuint(p.y);

        uint h = x * 0x8DA6B343u ^y * 0xD8163841u;
        h = hash_uint(h);

        return (h & 0x00FFFFFFu) * (1.0 / 16777216.0);
      }

      float value_noise(float2 uv)
      {
        float2 cell = floor(uv);
        float2 f = uv - cell;

        int2 i = int2(cell);

        float a = hash_lattice(i);
        float b = hash_lattice(i + int2(1, 0));
        float c = hash_lattice(i + int2(0, 1));
        float d = hash_lattice(i + int2(1, 1));

        float2 u = f * f * (3.0 - 2.0 * f);

        return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
      }

      float fbm(float2 uv)
      {
        float value = 0.0;
        float amplitude = 0.5;
        float frequency = 1.0;

        for (int i = 0; i < 5; i++)
        {
          value += value_noise(uv * frequency) * amplitude;
          frequency *= 2.0;
          amplitude *= 0.5;
        }

        return value;
      }

      float ridged_fbm(float2 uv)
      {
        float value = 0.0;
        float amplitude = 0.5;
        float frequency = 1.0;

        for (int i = 0; i < 5; i++)
        {
          float n = value_noise(uv * frequency);
          n = 1.0 - abs(n * 2.0 - 1.0);
          value += n * n * amplitude;

          frequency *= 2.05;
          amplitude *= 0.52;
        }

        return value;
      }

      float seeded_signed_noise(float seed_offset)
      {
        return hash_lattice(float2(_Seed + seed_offset, _Seed * 1.371 + seed_offset)) * 2.0 - 1.0;
      }

      float edge_fade(float2 uv)
      {
        float2 edge_distance = min(uv, 1.0 - uv);
        float nearest_edge = min(edge_distance.x, edge_distance.y);

        return smoothstep(0.0, 0.085, nearest_edge);
      }

      float2 rotate_2d(float2 p, float angle)
      {
        float s = sin(angle);
        float c = cos(angle);

        return float2(p.x * c - p.y * s, p.x * s + p.y * c);
      }

      float2 warp_uv(float2 uv)
      {
        float warp_a = fbm(uv * _WarpScale + _SeedOffset.xy + _Seed * 0.411);
        float warp_b = fbm(uv * _WarpScale + _SeedOffset.zw - _Seed * 0.719);

        float2 warp = float2(warp_a, warp_b) * 2.0 - 1.0;
        float2 secondary_uv = rotate_2d(uv, _Seed * 0.173 + 1.917);

        float warp_c = ridged_fbm(secondary_uv * (_WarpScale * 0.73 + 0.35) + _SeedOffset.yx + _Seed * 0.263);
        float warp_d = fbm(secondary_uv * (_WarpScale * 1.61 + 0.15) - _SeedOffset.wz - _Seed * 0.347);

        float2 secondary_warp = float2(warp_c, warp_d) * 2.0 - 1.0;

        return uv + (warp * 0.72 + secondary_warp * 0.28) * _WarpStrength;
      }

      float3 nebula_gradient(float t)
      {
        t = saturate(t);

        float stop_b = min(_ColorStopB, _ColorStopC - 0.01);
        float stop_c = max(_ColorStopC, stop_b + 0.01);

        float in_bc = step(stop_b, t);
        float in_cd = step(stop_c, t);

        float band_start = lerp(0.0, stop_b, in_bc);
        band_start = lerp(band_start, stop_c, in_cd);

        float band_end = lerp(stop_b, stop_c, in_bc);
        band_end = lerp(band_end, 1.0, in_cd);

        float3 color_start = lerp(_ColorA.rgb, _ColorB.rgb, in_bc);
        color_start = lerp(color_start, _ColorC.rgb, in_cd);

        float3 color_end = lerp(_ColorB.rgb, _ColorC.rgb, in_bc);
        color_end = lerp(color_end, _ColorD.rgb, in_cd);

        float u = saturate((t - band_start) / max(band_end - band_start, 0.0001));
        u = pow(u, _ColorBandSharpness);

        return lerp(color_start, color_end, u);
      }

      fixed4 frag(vertOutput input) : SV_Target
      {
        float2 screen_uv = input.screenPos.xy / input.screenPos.w;

        float2 uv = input.uv - float2(0.5, 0.5);
        uv = rotate_2d(uv, _Rotation);
        uv.x *= _StretchX;
        uv.y *= _StretchY;

        float2 shape_uv = uv - _CoreOffset.xy;
        float shape_angle = atan2(shape_uv.y, shape_uv.x);
        float shape_radius = length(shape_uv);

        float lobe_a = fbm(float2(
          shape_angle * 0.85 + _Seed * 0.071,
          shape_radius * 2.15 - _Seed * 0.119) + _SeedOffset.xy);

        float lobe_b = ridged_fbm(float2(
          shape_angle * 1.73 - _Seed * 0.053,
          shape_radius * 3.35 + _Seed * 0.097) + _SeedOffset.zw);

        float asymmetry = 1.0;
        asymmetry += (lobe_a - 0.5) * 0.42;
        asymmetry += (lobe_b - 0.5) * 0.26;
        asymmetry += dot(normalize(shape_uv + 0.0001), normalize(float2(
          seeded_signed_noise(11.7),
          seeded_signed_noise(29.3)))) * 0.16;
        asymmetry = saturate(asymmetry);

        float radial = length(shape_uv) * 2.0;
        radial /= lerp(0.72, 1.28, asymmetry);

        float soft_shape = 1.0 - smoothstep(1.0 - _EdgeSoftness, 1.0, radial);
        soft_shape = pow(saturate(soft_shape), _ShapePower);

        float uv_edge_fade = edge_fade(input.uv);

        float2 warped_uv = warp_uv(uv);

        float2 deep_warp = float2(
          fbm(warped_uv * (_NoiseScale * 0.31 + 0.15) + _SeedOffset.xy + _Seed * 0.811),
          ridged_fbm(warped_uv * (_NoiseScale * 0.27 + 0.21) - _SeedOffset.zw - _Seed * 0.647)) * 2.0 - 1.0;

        warped_uv += deep_warp * (_WarpStrength * 0.18 + 0.035);

        float noise = fbm(warped_uv * _NoiseScale + _SeedOffset.xy + _Seed * 0.137);
        float detail = fbm(warped_uv * _NoiseScale * 2.35 + _SeedOffset.zw - _Seed * 0.271);
        float fine_detail = fbm(warped_uv * _NoiseScale * 5.0 + 8.91 + _Seed * 0.019);

        float ridges = ridged_fbm(
          rotate_2d(warped_uv, _Seed * 0.113 + 0.61) * _NoiseScale * 1.42
          + _SeedOffset.yw
          + _Seed * 0.389);

        float cloud_blend = hash_lattice(float2(_Seed * 0.173 + 4.1, _SeedOffset.x + 8.7));
        float ridge_blend = hash_lattice(float2(_Seed * 0.419 + 1.9, _SeedOffset.y - 2.4));

        float cloud = lerp(noise, detail, lerp(0.22, 0.52, cloud_blend));
        cloud = lerp(cloud, fine_detail, 0.12);
        cloud = lerp(cloud, ridges, lerp(0.10, 0.34, ridge_blend));

        float large_voids = fbm(
          warped_uv * (_NoiseScale * 0.38 + 0.2)
          - _SeedOffset.xy
          + _Seed * 0.223);

        cloud *= smoothstep(0.12, 0.88, large_voids + asymmetry * 0.22);

        cloud = saturate((cloud - _CloudThreshold) / max(0.0001, 1.0 - _CloudThreshold));
        cloud = pow(cloud, 1.35);

        float mask = cloud * soft_shape * uv_edge_fade * _NoiseStrength;

        float color_noise = fbm(warped_uv * _NoiseScale * 0.65 + _SeedOffset.zw + _Seed * 0.593);;

        float color_t = saturate((color_noise - 0.5) * 1.75 + 0.5);
        float3 color = nebula_gradient(color_t);

        float alpha = saturate(mask * _Alpha);
        float3 final_color = color * mask * _Intensity;

        float shimmer = astralum_focus_shimmer(
          screen_uv,
          _Time.y,
          _FocusShimmerSpeed,
          _FocusShimmerWidth,
          _FocusShimmerSoftness);

        shimmer *= mask;
        shimmer *= _FocusShimmer;

        final_color += _FocusShimmerColor.rgb * shimmer * _FocusShimmerIntensity;

        return fixed4(final_color, alpha);
      }
      ENDHLSL
    }
  }
}