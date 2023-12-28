Shader "Custom/CloudMapWireframeShader"
{
Properties
    {
		_Texture("Main Tex",2D) = ""{}
	}
    SubShader
    {
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct a2v {
				float4 position:POSITION;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 position:SV_POSITION;
				float3 color:COLOR0;
			};

			sampler2D _Texture;
			//由于着色器用RGB进行计算，因此先转换为HSV
			float3 RGB2HSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
				float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

				float d = q.x - min(q.w, q.y);
				float e = 1.0e-10;
				return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}
			//根据HSV颜色值映射到对应的纹理坐标进行纹理采样
			float3 ColorConvert(float3 originalHSVc)
			{
				return tex2D(_Texture, (1,(1 - originalHSVc.x) * 3) * step(2.0 / 3.0, originalHSVc.x) + (1,originalHSVc.x * 3 / 2) * step(originalHSVc.x, 2.0 / 3.0));
			}
			v2f vert(a2v v)
			{
				v2f f;
				f.position = UnityObjectToClipPos(v.position);
				f.color = v.color;
				return f;
			}

			fixed4 frag(v2f f) :SV_Target
			{
				//float3 convertedColor = RGB2HSV(f.color);
				//f.color = ColorConvert(convertedColor);
				return fixed4(f.color,1);
			}
			ENDCG
		}
		}
    FallBack "Diffuse"

}
