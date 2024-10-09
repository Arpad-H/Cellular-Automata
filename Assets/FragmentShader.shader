Shader "Custom/InstancedColorShader"
{
    Properties
    {
        _Center ("Center Point", Vector) = (16, 16, 16)
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Oapque"
        }
        LOD 200
        // Enable blending for transparency
        //Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
          //  Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Buffers from compute shader
            StructuredBuffer<float4> instanceColors;
            StructuredBuffer<float4> instancePositions;


            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            // Data passed from vertex to fragment shader
            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            float4 _Center;


            // Vertex shader with instancing support
            v2f vert(appdata v, uint instanceID : SV_InstanceID)
            {
                v2f o;

                // Get the instance's position and color from the buffer
                float4 worldPos = instancePositions[instanceID];
                float4 instanceColor = instanceColors[instanceID];

                // Transform cube vertex based on instance position
                if (instanceColor.a == 1)
                {
                     o.pos = UnityObjectToClipPos(v.vertex + worldPos);
                }
                else
                {
                    o.pos = UnityObjectToClipPos(v.vertex + worldPos + float4(11110, 11110, 11110, 10));
                }
               

                // Pass instance color to fragment shader
                o.color = instanceColor;
                return o;
            }

            // Fragment shader
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = i.color;
                col.a = col.a*1;
                return col ;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}