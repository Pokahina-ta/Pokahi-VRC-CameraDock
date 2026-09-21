using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Pokahi.VrcCameraDock
{
    public class CameraDockSetup : EditorWindow
    {
        public const string Root = "Assets/PokahiVrcCameraDock";
        RenderTexture feed;
        Camera source;
        GameObject dock;
        bool fit, flipY, flipX, guide = true;
        float size = 0.7f;
        string message = "";

        [MenuItem("Tools/Pokahi VRC Camera Dock/設置・映像を設定")]
        static void Open() { GetWindow<CameraDockSetup>("Camera Dock").minSize = new Vector2(440, 390); }

        void OnGUI()
        {
            EditorGUILayout.LabelField("VRCカメラを入れる撮影枠", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("撮影枠を配置し、既存カメラのRenderTextureを割り当てます。再生・音声・カメラワークの制御は行いません。", MessageType.Info);
            source = (Camera)EditorGUILayout.ObjectField("映像元カメラ（任意）", source, typeof(Camera), true);
            using (new EditorGUI.DisabledScope(source == null || source.targetTexture == null))
                if (GUILayout.Button("カメラのTarget Textureを使用")) feed = source.targetTexture;
            feed = (RenderTexture)EditorGUILayout.ObjectField("映像 RenderTexture", feed, typeof(RenderTexture), false);
            dock = (GameObject)EditorGUILayout.ObjectField("更新する設置枠（空なら新規）", dock, typeof(GameObject), true);
            fit = EditorGUILayout.Popup("画面比率", fit ? 1 : 0, new[] { "余白なし（端を切り取る）", "全体表示（必要な余白を付ける）" }) == 1;
            flipY = EditorGUILayout.Toggle("上下反転", flipY);
            flipX = EditorGUILayout.Toggle("左右反転", flipX);
            guide = EditorGUILayout.Toggle("設置ガイドを表示", guide);
            size = EditorGUILayout.Slider("枠の一辺（m）", size, 0.2f, 2f);
            using (new EditorGUI.DisabledScope(feed == null || EditorApplication.isPlayingOrWillChangePlaymode))
            {
                if (GUILayout.Button("設定してシーンへ配置／更新", GUILayout.Height(34)))
                {
                    try { dock = Install(feed, dock, fit, flipY, flipX, guide, size); message = "設定しました。枠を好きな位置に移動し、シーンを保存してください。"; }
                    catch (Exception e) { message = e.Message; }
                }
            }
            EditorGUILayout.HelpBox("通常のUnityカメラでは映像の差し替えは見えません。VRCのBuild & Testで、手持ちカメラのレンズを枠の中へ入れて確認してください。", MessageType.None);
            if (!string.IsNullOrEmpty(message)) EditorGUILayout.HelpBox(message, MessageType.Info);
        }

        public static GameObject Install(RenderTexture texture, GameObject existing, bool fit, bool flipY, bool flipX, bool showGuide, float size)
        {
            if (texture == null) throw new ArgumentException("RenderTextureを指定してください。");
            if (!AssetDatabase.Contains(texture)) throw new ArgumentException("Project内のRenderTextureアセットを指定してください。");
            if (float.IsNaN(size) || float.IsInfinity(size) || size <= 0) throw new ArgumentException("枠のサイズを確認してください。");
            if (existing != null && (!existing.scene.IsValid() || EditorUtility.IsPersistent(existing)))
                throw new ArgumentException("更新先はシーンに配置済みの枠を指定してください。");
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Prefabs/CameraDock.prefab");
            if (prefab == null) throw new InvalidOperationException("CameraDock.prefabが見つかりません。パッケージを再インポートしてください。");
            if (existing != null && (existing.transform.Find("Capture Volume") == null || existing.transform.Find("Guide") == null))
                throw new ArgumentException("CameraDockのルートオブジェクトを指定してください。");
            var shader = Shader.Find("Pokahi/VRC Camera Dock/Camera Feed");
            if (shader == null || ShaderUtil.ShaderHasError(shader)) throw new InvalidOperationException("シェーダーのコンパイルを確認してください。");
            if (!AssetDatabase.IsValidFolder("Assets/PokahiCameraDockSettings")) AssetDatabase.CreateFolder("Assets", "PokahiCameraDockSettings");
            var material = new Material(shader) { name = "CameraDock Feed" };
            material.SetTexture("_MainTex", texture); material.SetFloat("_FeedActive", 1);
            material.SetFloat("_AspectMode", fit ? 1 : 0); material.SetFloat("_FlipY", flipY ? 1 : 0); material.SetFloat("_FlipX", flipX ? 1 : 0);
            // Separate asset per application: another dock/scene's feed is never overwritten.
            AssetDatabase.CreateAsset(material, AssetDatabase.GenerateUniqueAssetPath("Assets/PokahiCameraDockSettings/CameraDock Feed.mat"));
            bool created = existing == null;
            var result = existing != null ? existing : (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (created) { Undo.RegisterCreatedObjectUndo(result, "Place Camera Dock"); result.transform.position = new Vector3(0, 1.4f, 0); }
            var capture = result.transform.Find("Capture Volume").GetComponent<MeshRenderer>();
            var frame = result.transform.Find("Guide").gameObject;
            Undo.RecordObject(result.transform, "Resize Camera Dock"); Undo.RecordObject(capture, "Set Camera Feed"); Undo.RecordObject(frame, "Set Guide Visibility");
            result.transform.localScale = Vector3.one * size; capture.sharedMaterial = material; frame.SetActive(showGuide);
            PrefabUtility.RecordPrefabInstancePropertyModifications(result.transform);
            PrefabUtility.RecordPrefabInstancePropertyModifications(capture);
            PrefabUtility.RecordPrefabInstancePropertyModifications(frame);
            EditorSceneManager.MarkSceneDirty(result.scene); AssetDatabase.SaveAssets(); Selection.activeGameObject = result;
            return result;
        }
    }
}
