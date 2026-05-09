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
        
        Cull Off
        ZWrite Off
        ZTest Always
        
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
                float3 dir : TEXCOORD0;
            };
            
            vertOutput vert (vertInput input)
            {
                vertOutput output;
                
                output.pos = UnityObjectToClipPos(input.vertex);
                output.dir = normalize(input.vertex.xyz);
                
                return output;
            }
            
            float2 DirectionToEquirectUv(float3 dir)
            {
                dir = normalize(dir);
                
                float u = atan2(dir.z, dir.x) / (2.0 * UNITY_PI) + 0.5;
                float v = asin(dir.y) / UNITY_PI + 0.5;
                
                u = frac(u + _Rotation);
                
                return float2(u, v);
            }
            
            fixed4 SamplePanorama(float2 uv)
            {
                float seamWidth = 0.006;
                fixed4 col = tex2Dlod(_MainTex, float4(uv, 0, 0));
                
                if (uv.x < seamWidth)
                {
                    float t = uv.x / seamWidth;
                    
                    fixed4 leftEdge = tex2Dlod(_MainTex, float4(uv.x, uv.y, 0, 0));
                    fixed4 rightEdge = tex2Dlod(_MainTex, float4(1.0 - seamWidth + uv.x, uv.y, 0, 0));
                    
                    col = lerp(rightEdge, leftEdge, t);
                }
                else if (uv.x > 1.0 - seamWidth)
                {
                    float t = (uv.x - (1.0 - seamWidth)) / seamWidth;
                    
                    fixed4 rightEdge = tex2Dlod(_MainTex, float4(uv.x, uv.y, 0, 0));
                    fixed4 leftEdge = tex2Dlod(_MainTex, float4(uv.x - (1.0 - seamWidth), uv.y, 0, 0));
                    
                    col = lerp(rightEdge, leftEdge, t);
                }
                
                return col;
            }
            
            fixed4 frag (vertOutput input) : SV_Target
            {
                float2 uv = DirectionToEquirectUv(input.dir);
                fixed4 col = SamplePanorama(uv);
                
                col.rgb *= _Tint.rgb * _Intensity;
                col.a = 1.0;
                
                return col;
            }
            ENDHLSL
        }
    }
}