Shader "Astralum/Starfield01"
{
    Properties
    {
        _MainTex ("Skyfield Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _Intensity ("Intensity", Range(0,2)) = 1
        _Rotation ("Rotation", Range(0,1)) = 0
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Background"
            "RenderType" = "Background"
            "PreviewType" = "Skybox"
        }
        
        Cull Back
        ZWrite Off
        ZTest LEqual
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _Tint;
            float _Intensity;
            float _Rotation;
            
            struct vertInput
            {
                float4 vertex : POSITION;
            };
            
            struct vertOutput
            {
                float4 pos : SV_POSITION;
                float3 worldDir : TEXCOORD0;
            };
            
            vertOutput vert (vertInput input)
            {
                vertOutput output;

                output.pos = UnityObjectToClipPos(input.vertex);
                float3 worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
                output.worldDir = normalize(worldPos - _WorldSpaceCameraPos);

                return output;
            }
            
            float2 DirectionToEquirectUv(float3 dir)
            {
                dir = normalize(dir);
                
                float u = 1.0 - ( atan2(dir.z, dir.x) / (2.0 * UNITY_PI) + 0.5);
                float v = asin(dir.y) / UNITY_PI + 0.5;
                
                u = u + _Rotation;
                
                return float2(u, v);
            }
            
            fixed4 frag (vertOutput input) : SV_Target
            {
                float2 uv = DirectionToEquirectUv(input.worldDir);
                
                float2 dx = ddx(uv);
                float2 dy = ddy(uv);
                
                dx.x = abs(dx.x) > 0.5 ? dx.x - sign(dx.x) : dx.x;
                dy.x = abs(dy.x) > 0.5 ? dy.x - sign(dy.x) : dy.x;
                
                fixed4 col = tex2Dgrad(_MainTex, uv, dx, dy);
                
                col.rgb *= _Tint.rgb * _Intensity;
                col.a = 1.0;
                
                return col;
            }
            ENDHLSL
        }
    }
}