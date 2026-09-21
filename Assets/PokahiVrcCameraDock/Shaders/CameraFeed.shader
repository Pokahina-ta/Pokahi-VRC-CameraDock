Shader "Pokahi/VRC Camera Dock/Camera Feed"
{
 Properties
 {
  _MainTex("Camera feed (RenderTexture)", 2D) = "black" {}
  [Toggle] _FeedActive("Enable camera feed", Float) = 0
  [Enum(Fill crop,0,Fit letterbox,1)] _AspectMode("Aspect ratio", Float) = 0
  [Toggle] _FlipY("Flip vertically", Float) = 0
  [Toggle] _FlipX("Flip horizontally", Float) = 0
 }
 SubShader
 {
  Tags { "Queue"="Overlay+100" "RenderType"="Transparent" "DisableBatching"="True" "IgnoreProjector"="True" }
  Cull Off ZWrite Off ZTest Always
  Pass
  {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   sampler2D _MainTex;
   float4 _MainTex_TexelSize;
   float _FeedActive, _AspectMode, _FlipY, _FlipX;
   float _VRChatCameraMode, _VRChatMirrorMode, _VRChatFaceMirrorMode;
   float3 _VRChatPhotoCameraPos;
   struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };
   v2f vert(appdata_base v)
   {
    v2f o;
    o.pos=float4(v.vertex.xy*2,UNITY_NEAR_CLIP_VALUE,1);
    o.pos.y*=_ProjectionParams.x;
    o.uv=v.texcoord.xy;
    return o;
   }
   fixed4 frag(v2f i):SV_Target
   {
    bool handheld=(_VRChatCameraMode==1 || _VRChatCameraMode==2);
    bool photo=(_VRChatCameraMode==3 && distance(_WorldSpaceCameraPos,_VRChatPhotoCameraPos)<0.05);
    clip((handheld || photo) && _VRChatMirrorMode==0 && _VRChatFaceMirrorMode==0 && _FeedActive>0.5 ? 1 : -1);
    float3 local=mul(unity_WorldToObject,float4(_WorldSpaceCameraPos,1)).xyz;
    clip(0.5-max(abs(local.x),max(abs(local.y),abs(local.z))));
    float2 uv=i.uv;
    float outputAspect=_ScreenParams.x/_ScreenParams.y;
    float sourceAspect=_MainTex_TexelSize.z/max(_MainTex_TexelSize.w,1);
    if (_AspectMode<0.5)
    {
     if(outputAspect>sourceAspect) uv.y=(uv.y-0.5)*(sourceAspect/outputAspect)+0.5;
     else uv.x=(uv.x-0.5)*(outputAspect/sourceAspect)+0.5;
    }
    else
    {
     if(outputAspect>sourceAspect) uv.x=(uv.x-0.5)*(outputAspect/sourceAspect)+0.5;
     else uv.y=(uv.y-0.5)*(sourceAspect/outputAspect)+0.5;
     if(any(uv<0) || any(uv>1)) return fixed4(0,0,0,1);
    }
    #if UNITY_UV_STARTS_AT_TOP
    if(_MainTex_TexelSize.y<0) uv.y=1-uv.y;
    #endif
    if(_FlipY>0.5) uv.y=1-uv.y;
    if(_FlipX>0.5) uv.x=1-uv.x;
    return fixed4(tex2D(_MainTex,uv).rgb,1);
   }
   ENDCG
  }
 }
 Fallback Off
}
