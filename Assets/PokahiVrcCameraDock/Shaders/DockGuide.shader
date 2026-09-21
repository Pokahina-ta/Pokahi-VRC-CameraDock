Shader "Pokahi/VRC Camera Dock/Guide"
{
 Properties { _Color("Frame color", Color)=(0.8,0.65,1,1) }
 SubShader { Tags { "RenderType"="Opaque" } Pass {
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 fixed4 _Color; float _VRChatCameraMode, _VRChatMirrorMode, _VRChatFaceMirrorMode;
 struct v2f {float4 pos:SV_POSITION;};
 v2f vert(appdata_base v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);return o;}
 fixed4 frag(v2f i):SV_Target {clip(_VRChatCameraMode==0 && _VRChatMirrorMode==0 && _VRChatFaceMirrorMode==0 ? 1 : -1);return _Color;}
 ENDCG
 } }
 Fallback Off
}
