Shader "Astralum/Aurora01"
{
  Properties
  {
    _AuroraColorBottom ("Aurora Color Bottom", Color) = (0.2, 1.0, 0.45, 1.0)
    _AuroraColorMiddle ("Aurora Color Middle", Color) = (0.0, 0.7, 1.0, 1.0)
    _AuroraColorTop ("Aurora Color Top", Color) = (0.7, 0.0, 1.0, 1.0)
    _Intensity ("Intensity", Range(0, 5)) = 1.5
    _RayFrequency ("Ray Frequency", Range(1, 64)) = 24
    _RayStrength ("Ray Strength", Range(0, 1)) = 0.65
    _FilamentStrength ("Filament Strength", Range(0, 1)) = 0.45
    _AnimationSpeed ("Animation Speed", Range(0, 5)) = 0.25
    _DistortionStrength ("Distortion Strength", Range(0, 0.25)) = 0.06
    _WiggleStrength ("Wiggle Strength", Range(0, 20)) = 0.5
    _WiggleSpeed ("Wiggle Speed", Range(0, 20)) = 1.5

    _AuroraActive ("Aurora Active", Range(0, 1)) = 1
  }

  SubShader
  {
    Tags
    {
      "Queue" = "Overlay"
      "RenderType" = "Transparent"
      "IgnoreProjector" = "True"
    }

    Blend SrcAlpha One
    Cull Off
    ZWrite Off
    ZTest LEqual

    Pass
    {
      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      struct vertInput
      {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
      };

      struct vertOutput
      {
        float4 position : SV_POSITION;
        float2 uv : TEXCOORD0;
      };

      fixed4 _AuroraColorBottom;
      fixed4 _AuroraColorMiddle;
      fixed4 _AuroraColorTop;
      float _Intensity;
      float _RayFrequency;
      float _RayStrength;
      float _FilamentStrength;
      float _AnimationSpeed;
      float _DistortionStrength;
      float _WiggleStrength;
      float _WiggleSpeed;

      float _AuroraActive;

      float hash3(float3 p)
      {
        p = frac(p * float3(123.34, 456.21, 789.12));
        p += dot(p, p.yzx + 45.32);

        return frac((p.x + p.y) * p.z);
      }

      float noise_3d(float3 p)
      {
        float3 i = floor(p);
        float3 f = frac(p);

        f = f * f * (3.0 - 2.0 * f);

        float n000 = hash3(i + float3(0.0, 0.0, 0.0));
        float n100 = hash3(i + float3(1.0, 0.0, 0.0));
        float n010 = hash3(i + float3(0.0, 1.0, 0.0));
        float n110 = hash3(i + float3(1.0, 1.0, 0.0));
        float n001 = hash3(i + float3(0.0, 0.0, 1.0));
        float n101 = hash3(i + float3(1.0, 0.0, 1.0));
        float n011 = hash3(i + float3(0.0, 1.0, 1.0));
        float n111 = hash3(i + float3(1.0, 1.0, 1.0));

        float n00 = lerp(n000, n100, f.x);
        float n10 = lerp(n010, n110, f.x);
        float n01 = lerp(n001, n101, f.x);
        float n11 = lerp(n011, n111, f.x);

        float n0 = lerp(n00, n10, f.y);
        float n1 = lerp(n01, n11, f.y);

        return lerp(n0, n1, f.z);
      }

      // circular noise domain:
      // uv.x = 0 and uv.x = 1 produce exactly the same point in
      // this remove the curtain seam
      float aurora_noise(float u, float radius, float offset)
      {
        float angle = u * UNITY_TWO_PI;
        float2 circle = float2(cos(angle), sin(angle));

        return noise_3d(float3(circle * radius, offset));
      }

      float aurora_vertical_noise(float u, float height, float radius, float offset)
      {
        float angle = u * UNITY_TWO_PI;
        float2 circle = float2(cos(angle), sin(angle));

        return noise_3d(float3(circle * radius + offset, height));
      }

      vertOutput vert(vertInput v)
      {
        vertOutput o;

        float time = _Time.y * _WiggleSpeed;

        // slow, broad movement of the curtain
        // then, smaller, quicker motion layered over it
        float broad_wiggle = aurora_noise(v.uv.x, 2.0, time * 0.12);
        float fine_wiggle = aurora_noise(v.uv.x, 5.0, -time * 0.18 + 13.7);
        broad_wiggle = broad_wiggle * 2.0 - 1.0;
        fine_wiggle = fine_wiggle * 2.0 - 1.0;

        float wiggle = broad_wiggle * 0.7 + fine_wiggle * 0.3;

        // anchor the lower edge more strongly while allowing the upper curtain to move
        float height_mask = smoothstep(0.0, 1.0, v.uv.y);
        float displacement = wiggle * _WiggleStrength * height_mask;

        float3 curtain_direction = float3(v.vertex.x, 0.0, v.vertex.z);
        float curtain_length = length(curtain_direction);

        if (curtain_length > 0.0001)
        {
          curtain_direction /= curtain_length;
          v.vertex.xyz += curtain_direction * displacement;
        }

        o.position = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;

        return o;
      }

      fixed4 frag(vertOutput input) : SV_Target
      {
        float time = _Time.y * _AnimationSpeed;

        // slowly move the fine structures laterally through the curtain
        float flow = aurora_noise(input.uv.x, 2.0, time * 0.08);
        float flow_offset = (flow - 0.5) * _DistortionStrength * input.uv.y;
        float flowing_u = input.uv.x + flow_offset;

        // ------------------------------------------------
        // irregular vertical extent
        // ------------------------------------------------
        float edge_noise = aurora_noise(flowing_u, 2.5, time * 0.08);

        float top_start = lerp(0.5, 0.72, edge_noise);
        float irregular_top_fade = 1.0 - smoothstep(top_start, 0.92, input.uv.y);
        float bottom_edge_fade = smoothstep(0.0, 0.25, input.uv.y);
        float top_edge_fade = 1.0 - smoothstep(0.75, 1.0, input.uv.y);
        float edge_fade = bottom_edge_fade * top_edge_fade;

        float vertical_mask = irregular_top_fade * edge_fade;

        // ------------------------------------------------
        // broad luminosity
        // ------------------------------------------------
        float broad_noise = aurora_noise(flowing_u, 3.0, time * 0.12);
        float broad_structure = lerp(0.4, 1.0, broad_noise);

        // ------------------------------------------------
        // rayed structure
        // ------------------------------------------------
        float fine_noise_a = aurora_noise(flowing_u, _RayFrequency * 0.25, time * 0.35);
        float fine_noise_b = aurora_noise(flowing_u, _RayFrequency * 0.43, -time * 0.22 + 31.7);

        float ray_noise = fine_noise_a * 0.65 + fine_noise_b * 0.35;
        float fine_rays = smoothstep(0.4, 0.8, ray_noise);
        float rays = broad_structure * lerp(1.0, fine_rays, _RayStrength);

        // ------------------------------------------------
        // thin field-aligned filaments
        // ------------------------------------------------
        // yeah, so like... let's not turn the entire ribbon into a fucking bar code...
        float filament_noise = aurora_noise(flowing_u, _RayFrequency * 0.8, -time * 0.3 + 57.1);
        float filaments = smoothstep(0.72, 0.9, filament_noise);
        rays *= lerp(1.0, 1.5, filaments * _FilamentStrength);

        // keep the rays vertically coherent, but allow their brightness to vary along their height
        float vertical_variation = aurora_vertical_noise(
          flowing_u, input.uv.y * 3.0, 4.0, -time * 0.15 + 17.3);

        rays *= lerp(0.35, 1.0, vertical_variation);

        // ------------------------------------------------
        // vertical color morphology
        // ------------------------------------------------
        float color_noise = aurora_vertical_noise(flowing_u, input.uv.y * 2.0,
                                                               2.0, time * 0.05 + 41.7);

        // prevent the color boundaries from reading as perfectly horizontal bands
        float color_y = input.uv.y + (color_noise - 0.5) * 0.10;
        float middle_blend = smoothstep(0.55, 0.78, color_y);
        float top_blend = smoothstep(0.78, 1.0, color_y);

        float4 aurora_color = lerp(_AuroraColorBottom, _AuroraColorMiddle, middle_blend);
        aurora_color = lerp(aurora_color, _AuroraColorTop, top_blend);

        float glow = vertical_mask * rays * _Intensity * _AuroraActive;
        float3 color = aurora_color.rgb * glow;
        float alpha = aurora_color.a * vertical_mask * rays * _AuroraActive;

        return fixed4(color, alpha);
      }
      ENDHLSL
    }
  }
}