Shader "Rokoko/RoundGridFloor"
{
	Properties
	{
		[NoScaleOffset]_MainTexture("MainTexture", 2D) = "white" {}
		_Tiles("Tiles", Range( 0.2 , 4)) = 0.04
		_SubTiles("SubTiles", Int) = 4
		_Radius("Radius", Float) = 2
		_FadeDistance("FadeDistance", Float) = 2
		_MainColor("MainColor", Color) = (0.5849056,0.5849056,0.5849056,0)
		_GridColor("GridColor", Color) = (1,1,1,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
		};

		uniform float4 _MainColor;
		uniform float4 _GridColor;
		uniform sampler2D _MainTexture;
		uniform float _Tiles;
		uniform int _SubTiles;
		uniform float _FadeDistance;
		uniform float _Radius;
		uniform sampler2D _GrabTexture;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float4 _black = float4(0,0,0,0);
			float3 ase_worldPos = i.worldPos;
			float4 transform127 = mul(unity_WorldToObject,float4( ase_worldPos , 0.0 ));
			float4 appendResult129 = (float4(transform127.x , transform127.z , 0.0 , 0.0));
			float4 lerpResult114 = lerp( _MainColor , _GridColor , max( tex2D( _MainTexture, ( appendResult129 * _Tiles ).xy ).r , tex2D( _MainTexture, ( appendResult129 * _SubTiles * _Tiles ).xy ).r ));
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float simplePerlin2D148 = snoise( ( ase_screenPosNorm * 2048.0 ).xy );
			float ScreenRandom152 = ( ( simplePerlin2D148 - 1.0 ) * 0.0008 );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float clampResult126 = clamp( ( (0.0 + (length( ( ase_vertex3Pos * ase_objectScale ) ) - ( _FadeDistance + _Radius )) * (1.0 - 0.0) / (_Radius - ( _FadeDistance + _Radius ))) + ( ScreenRandom152 * 32.0 ) ) , 0.0 , 1.0 );
			float Mask84 = clampResult126;
			float4 lerpResult86 = lerp( _black , ( lerpResult114 + ScreenRandom152 ) , Mask84);
			o.Albedo = lerpResult86.rgb;
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 screenColor106 = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD( ase_grabScreenPos ) );
			float4 lerpResult107 = lerp( screenColor106 , _black , Mask84);
			o.Emission = lerpResult107.rgb;
			o.Occlusion = Mask84;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}