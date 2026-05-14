Shader "Astralum/SkyCoordinateGrid01"
{
    Properties
    {
        _Color ("Color", Color) = (0.35, 0.65, 1.0, 0.28)
        _Intensity ("Intensity", Range(0, 5)) = 1
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Background+10"
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
            
            fixed4 _Color;
            float _Intensity;
            
            struct vertInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct vertOutput
            {
                float4 pos : SV_POSITION;
            };
            
            vertOutput vert(vertInput input)
            {
                vertOutput output;
                
                float4 worldPos = mul(unity_ObjectToWorld, input.vertex);
                worldPos.xyz += _WorldSpaceCameraPos;
                
                output.pos = mul(UNITY_MATRIX_VP, worldPos);
                
                return output;
            }
            
            fixed4 frag(vertOutput input) : SV_Target
            {
                return fixed4(_Color.rgb * _Intensity, _Color.a);
            }
            ENDHLSL
        }
    }
}