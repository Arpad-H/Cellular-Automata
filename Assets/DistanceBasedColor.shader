Shader "Custom/DistanceBasedColor"
{
    Properties
    {
        _Center ("Center Point", Vector) = (0, 0, 0)
        _MinDistanceColor ("Min Distance Color", Color) = (1, 1, 1, 1) // White
        _MaxDistanceColor ("Max Distance Color", Color) = (0, 0, 0, 1) // Black
        _MaxDistance ("Max Distance", Float) = 10.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members instancePos)
#pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // StructuredBuffer to hold instance-specific positions and colors
            StructuredBuffer<float4> instancePositions;
            StructuredBuffer<float4> instanceColors;

            struct appdata
            {
                float4 vertex : POSITION;
                uint instanceID : SV_InstanceID; // Use instance ID to index into buffers
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 instancePos; // Holds the per-instance position
            };

            // Properties (uniforms)
            float4 _Center; // Center of the grid
            float4 _MinDistanceColor; // Color for cubes close to the center
            float4 _MaxDistanceColor; // Color for cubes far from the center
            float _MaxDistance; // The maximum distance for color interpolation

            v2f vert(appdata v)
            {
                v2f o;

                // Get instance-specific position from the buffer using the instanceID
                float4 instancePosition = instancePositions[v.instanceID];

                // Apply instance-specific position to the vertex position
                o.pos = UnityObjectToClipPos(v.vertex + instancePosition);
                o.instancePos = instancePosition; // Pass the instance position to the fragment shader

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Compute distance from the center for coloring
                float distanceFromCenter = distance(i.instancePos.xyz, _Center.xyz);
                float t = saturate(distanceFromCenter / _MaxDistance); // Normalize to [0, 1]

                // Interpolate between the min and max colors based on distance
                fixed4 interpolatedColor = lerp(_MinDistanceColor, _MaxDistanceColor, t);

                // Get the instance-specific color from the buffer
                fixed4 instanceColor = instanceColors[i.instancePos.w]; // Using w as index
                
                // Modulate the interpolated color with the per-instance color
                return interpolatedColor * instanceColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
