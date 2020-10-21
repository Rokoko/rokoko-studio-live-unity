// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Floor/RoundFloor"
{
	Properties
	{
		[NoScaleOffset]_MainTexture("MainTexture", 2D) = "white" {}
		_Tiles("Tiles", Range( 0.2 , 8)) = 0.04
		_Radius("Radius", Float) = 2
		_FadeDistance("FadeDistance", Float) = 2
		_Tint("Tint", Color) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
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

		uniform float4 _Tint;
		uniform sampler2D _MainTexture;
		uniform float _Tiles;
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
			float4 tex2DNode111 = tex2D( _MainTexture, ( appendResult129 * _Tiles ).xy );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float simplePerlin2D145 = snoise( ( ase_screenPosNorm * 2048.0 ).xy );
			float ScreenRandom150 = ( ( simplePerlin2D145 - 1.0 ) * 0.0008 );
			float4 appendResult157 = (float4(ScreenRandom150 , ScreenRandom150 , ScreenRandom150 , 0.0));
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float clampResult126 = clamp( ( ( ScreenRandom150 * 4.0 ) + (0.0 + (length( ( ase_vertex3Pos * ase_objectScale ) ) - ( _FadeDistance + _Radius )) * (1.0 - 0.0) / (_Radius - ( _FadeDistance + _Radius ))) ) , 0.0 , 1.0 );
			float Mask84 = clampResult126;
			float4 lerpResult86 = lerp( _black , ( ( _Tint * tex2DNode111 ) + appendResult157 ) , Mask84);
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
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
1519;518;1811;1059;373.3069;45.93286;1;True;True
Node;AmplifyShaderEditor.ScreenPosInputsNode;146;317.2799,-731.9501;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;153;341.9099,-554.7551;Float;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;False;0;2048;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;160;585.2299,-665.0952;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;145;748.5499,-697.0951;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;130;-2124.388,-428.0235;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;137;-903.1921,-33.53546;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ObjectScaleNode;134;-896.1921,120.4645;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;161;1019.703,-692.5537;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;148;872.8716,-514.8192;Half;False;Constant;_subpixel;subpixel;10;0;Create;True;0;0;False;0;0.0008;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-598.9555,607.2492;Float;False;Property;_FadeDistance;FadeDistance;6;0;Create;True;0;0;False;0;2;9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;127;-1823.388,-437.0235;Float;False;1;0;FLOAT4;20,20,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;-589.1921,29.46454;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;147;1134.307,-511.2391;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-352.7222,684.8953;Float;False;Property;_Radius;Radius;5;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;151;-2.420105,258.225;Float;False;150;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;150;1324.307,-491.2391;Float;False;ScreenRandom;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;129;-1593.388,-556.0234;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;125;-150.5376,510.1237;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;138;-1582.388,-391.0236;Float;False;Property;_Tiles;Tiles;2;0;Create;True;0;0;False;0;0.04;1;0.2;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;133;-327.1921,132.4645;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;-1197.388,-594.0234;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TFHCRemapNode;124;109.9944,476.2878;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;275.6931,330.0671;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-1825.788,-841.0571;Float;True;Property;_MainTexture;MainTexture;0;1;[NoScaleOffset];Create;True;0;0;False;0;None;fc8b9f4312f03ff4ea0aca74cd06c12f;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ColorNode;141;-948.2568,-883.9335;Float;False;Property;_Tint;Tint;9;0;Create;True;0;0;False;0;0,0,0,0;0.117647,0.1098039,0.1137255,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;158;-20.31262,-434.4338;Float;False;150;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;111;-1006.07,-668.8263;Float;True;Property;_TextureSample1;Texture Sample 1;9;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;149;439.0948,340.3649;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;126;629.6295,475.075;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;157;353.7221,-373.489;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-603.5977,-684.9129;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;45.02455,-341.24;Float;False;84;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;106;777.6548,-189.9321;Float;False;Global;_GrabScreen0;Grab Screen 0;8;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;84;863.8721,474.9304;Float;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;123;799.5237,114.9214;Float;False;84;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;102;411.6117,-38.82932;Float;False;Constant;_black;black;9;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;159;574.7977,-424.8738;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;-1187.089,-358.2042;Float;False;3;3;0;FLOAT4;0,0,0,0;False;1;INT;0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;25;-598.0596,-1018.035;Float;False;Property;_MainColor;MainColor;7;0;Create;True;0;0;False;0;0.5849056,0.5849056,0.5849056,0;0.5849056,0.5849056,0.5849056,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;39;-431.003,-801.3267;Float;False;Property;_GridColor;GridColor;8;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;107;1048.637,-65.39899;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldPosInputsNode;94;-709.0233,413.0014;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;86;789.4653,-396.7219;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;114;-43.10918,-765.0408;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;122;1190.246,104.9812;Float;False;84;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;154;544.9229,679.3065;Float;False;Constant;_Float1;Float 1;10;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;119;-1263.953,44.76419;Float;False;Constant;_ff;ff;3;0;Create;True;0;0;False;0;4;4;1;16;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;140;-1461.388,-307.0237;Float;False;Property;_SubTiles;SubTiles;3;0;Create;True;0;0;False;0;4;4;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;155;1260.729,-350.7839;Float;False;Constant;_Float2;Float 2;10;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;95;-480.9125,396.5108;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;117;-992.4498,-449.3242;Float;True;Property;_TextureSample2;Texture Sample 2;9;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMaxOpNode;120;-389.4804,-467.6964;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;93;-324.0916,340.3794;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;16;-714.3265,275.1477;Float;False;Property;_Center;Center;4;1;[HideInInspector];Create;True;0;0;False;0;0.6,0.8;0.6,0.8;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;90;1431.994,-183.2848;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;Floor/RoundFloor;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;160;0;146;0
WireConnection;160;1;153;0
WireConnection;145;0;160;0
WireConnection;161;0;145;0
WireConnection;127;0;130;0
WireConnection;135;0;137;0
WireConnection;135;1;134;0
WireConnection;147;0;161;0
WireConnection;147;1;148;0
WireConnection;150;0;147;0
WireConnection;129;0;127;1
WireConnection;129;1;127;3
WireConnection;125;0;30;0
WireConnection;125;1;8;0
WireConnection;133;0;135;0
WireConnection;139;0;129;0
WireConnection;139;1;138;0
WireConnection;124;0;133;0
WireConnection;124;1;125;0
WireConnection;124;2;8;0
WireConnection;162;0;151;0
WireConnection;111;0;2;0
WireConnection;111;1;139;0
WireConnection;149;0;162;0
WireConnection;149;1;124;0
WireConnection;126;0;149;0
WireConnection;157;0;158;0
WireConnection;157;1;158;0
WireConnection;157;2;158;0
WireConnection;143;0;141;0
WireConnection;143;1;111;0
WireConnection;84;0;126;0
WireConnection;159;0;143;0
WireConnection;159;1;157;0
WireConnection;118;0;129;0
WireConnection;118;1;140;0
WireConnection;118;2;138;0
WireConnection;107;0;106;0
WireConnection;107;1;102;0
WireConnection;107;2;123;0
WireConnection;86;0;102;0
WireConnection;86;1;159;0
WireConnection;86;2;85;0
WireConnection;114;0;25;0
WireConnection;114;1;39;0
WireConnection;114;2;120;0
WireConnection;95;0;94;1
WireConnection;95;1;94;3
WireConnection;117;0;2;0
WireConnection;117;1;118;0
WireConnection;120;0;111;1
WireConnection;120;1;117;1
WireConnection;93;0;16;0
WireConnection;93;1;95;0
WireConnection;90;0;86;0
WireConnection;90;2;107;0
WireConnection;90;5;122;0
ASEEND*/
//CHKSM=C508FCA374A838C255F108B15BE6F9AA2AD88E9B