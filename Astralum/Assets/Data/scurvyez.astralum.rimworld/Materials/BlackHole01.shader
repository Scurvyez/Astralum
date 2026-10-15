Shader "Astralum/BlackHole01"
{
  Properties
  {
    _EffectActive ("Effect Active", Range(0,1)) = 0
    _CanvasScale ("Canvas Scale", Range(1,10)) = 2
    _ScreenEdgeFadeStart ("Screen Edge Fade Start", Range(0,0.5)) = 0.035
    _ScreenEdgeFadeEnd ("Screen Edge Fade End", Range(0,0.5)) = 0.16

    _Radius ("Event Horizon Radius", Range(0,1)) = 0.18
    _DistortionRadius ("Distortion Radius", Range(0,5)) = 1
    _DistortionStrength ("Distortion Strength", Range(0,0.2)) = 0.04
    _Darkness ("Center Darkness", Range(0,1)) = 1

    _HorizonFeather ("Event Horizon Feather", Range(0.001,0.2)) = 0.025
    _DistortionFeather ("Distortion Edge Feather", Range(0.001,0.5)) = 0.12
    _RingFeather ("Einstein Ring Feather", Range(0.001,0.3)) = 0.08

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
      "Queue" = "Background+6"
      "RenderType" = "Transparent"
    }

    GrabPass
    {
      "_AstralumBlackHoleGrabTexture"
    }

    Pass
    {
      Blend SrcAlpha OneMinusSrcAlpha
      ZWrite Off
      ZTest LEqual
      Cull Off

      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"
      #include "CelestialFocusShimmer.hlsl"

      sampler2D _AstralumBlackHoleGrabTexture;

      float _EffectActive;
      float _CanvasScale;
      float _ScreenEdgeFadeStart;
      float _ScreenEdgeFadeEnd;

      float _Radius;
      float _DistortionRadius;
      float _DistortionStrength;
      float _Darkness;

      float _HorizonFeather;
      float _DistortionFeather;
      float _RingFeather;

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
        fixed4 color : COLOR;
      };

      struct vertOutput
      {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
        float4 color : COLOR;
        float4 grabPos : TEXCOORD1;
      };

      vertOutput vert(vertInput input)
      {
        vertOutput output;

        float4 world_pos = mul(unity_ObjectToWorld, input.vertex);
        world_pos.xyz += _WorldSpaceCameraPos;

        output.vertex = mul(UNITY_MATRIX_VP, world_pos);
        output.uv = input.uv;
        output.grabPos = ComputeGrabScreenPos(output.vertex);
        output.color = input.color;

        return output;
      }

      fixed4 frag(vertOutput input) : SV_Target
      {
        float active = saturate(_EffectActive);

        float2 screen_uv = input.grabPos.xy / input.grabPos.w;

        float edge_distance = min(
          min(screen_uv.x, 1.0 - screen_uv.x),
          min(screen_uv.y, 1.0 - screen_uv.y));

        float screen_edge_fade = smoothstep(_ScreenEdgeFadeStart, _ScreenEdgeFadeEnd, edge_distance);

        float2 p = (input.uv - 0.5) * _CanvasScale;
        float2 local_offset = p;

        float dist = length(local_offset);

        float distortion_mask = 1.0 - smoothstep(
          _DistortionRadius,
          _DistortionRadius + _DistortionFeather,
          dist
        );

        distortion_mask *= active;

        float2 radial_dir = local_offset / max(dist, 0.0001);

        // distance from event horizon
        float safe_dist = max(dist - _Radius, 0.0001);

        // strongest bending near the horizon, fading outward
        float bend = _DistortionStrength / safe_dist;

        // keep it controlled
        bend = min(bend, _DistortionStrength * 8.0);

        // fade out at outer distortion boundary.
        float outer_fade = 1.0 - smoothstep(_Radius, _DistortionRadius, dist);
        bend *= outer_fade * distortion_mask * active;

        // horizon fade prevents unstable smear inside the black center.
        float horizon_fade = smoothstep(_Radius, _Radius + _HorizonFeather, dist);
        bend *= horizon_fade;
        bend *= screen_edge_fade;

        // Einstein-ring
        float ring_center = _Radius + _RingFeather * 0.65;
        float ring_mask = 1.0 - smoothstep(_RingFeather * 0.75, _RingFeather * 1.75, abs(dist - ring_center));
        ring_mask *= active;
        ring_mask *= screen_edge_fade;

        // near the ring, sample from opposite side of the ring...
        // inverted sky effect on either side
        float inversion_strength = ring_mask * 0.65;

        float2 normal_lens_offset = local_offset - radial_dir * bend;
        float2 inverted_lens_offset = -local_offset * 0.85;

        // main gravitational lensing:
        // sample farther/closer along the radial line.
        // this makes background appear bent around the black hole.
        float2 distorted_local_offset = lerp(normal_lens_offset, inverted_lens_offset, inversion_strength);

        float2 distorted_uv = 0.5 + distorted_local_offset / _CanvasScale;
        float2 uv_delta = distorted_uv - input.uv;

        uv_delta *= screen_edge_fade;

        float4 grab_uv = input.grabPos;
        grab_uv.xy += uv_delta * grab_uv.w;

        fixed4 color = tex2Dproj(_AstralumBlackHoleGrabTexture, UNITY_PROJ_COORD(grab_uv));

        // DEBUGGING: shows the distortion mask
        //float2 debugUV = distortedUV;
        //fixed4 color = fixed4(frac(debugUV.x), frac(debugUV.y), 0, 1 );

        float hole_mask = 1.0 - smoothstep(_Radius, _Radius + _HorizonFeather, dist);
        hole_mask *= active;

        color.rgb = lerp(color.rgb, 0.0, hole_mask * _Darkness);

        float alpha_mask = 1.0 - smoothstep(_DistortionRadius, _DistortionRadius + _DistortionFeather, dist);

        float shimmer = astralum_focus_shimmer(
          screen_uv,
          _Time.y,
          _FocusShimmerSpeed,
          _FocusShimmerWidth,
          _FocusShimmerSoftness
        );

        shimmer *= alpha_mask;
        shimmer *= 1.0 - hole_mask;

        float center_falloff = 1.0 - smoothstep(_Radius, _DistortionRadius, dist);

        shimmer *= center_falloff;
        shimmer *= _FocusShimmer;

        color.rgb += _FocusShimmerColor.rgb * shimmer * _FocusShimmerIntensity;
        color.a = alpha_mask * active * input.color.a;

        return color;
      }
      ENDHLSL
    }
  }
}