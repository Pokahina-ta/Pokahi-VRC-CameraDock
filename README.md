# Pokahi VRC Camera Dock

**VRCの手持ちカメラを枠の中へ入れて、MMDなどの別カメラ映像を画面いっぱいに表示する、ワールド向けShader・Prefabです。**

カメラ調整ツールとは独立しています。再生中のカメラが出力しているRenderTextureを利用します。

## 配布内容

- 設置用 `CameraDock.prefab`（約70cmの枠）
- 手持ちカメラ向けの映像差し替えシェーダー
- ガイド枠、設定用マテリアル
- 日本語の配置・映像設定ウィンドウ
- 上下／左右反転、余白なし／全体表示の設定

## 導入

1. [unitypackage](Packages/Pokahi-VRC-CameraDock-0.1.0.unitypackage) をダウンロードします。
2. Unityの `Assets → Import Package → Custom Package` からインポートします。
3. `Tools → Pokahi VRC Camera Dock → 設置・映像を設定` を開きます。
4. **映像 RenderTexture** に、MMDカメラが使用中のRenderTextureを指定します。
5. **設定してシーンへ配置／更新**を押し、作られた枠を好きな場所へ動かします。
6. シーンを保存し、VRCのBuild & Testで手持ちカメラのレンズを枠の中へ入れます。外へ出すと通常の映像に戻ります。

映像元カメラを指定して「カメラのTarget Textureを使用」を押すこともできます。ツールは既存カメラの設定を変更しません。映像の生成・再生・音声は元のシステム側で行います。

## 手動でPrefabを置く場合

`Assets/PokahiVrcCameraDock/Prefabs/CameraDock.prefab` をシーンへ配置します。付属の `CameraFeed.mat` を複製し、`Camera feed` にRenderTextureを指定して `Enable camera feed` をオンにします。そのマテリアルを子の `Capture Volume` のMeshRendererへ割り当ててください。テンプレートは未設定のまま黒画面にしないよう無効にしてあります。

設定ウィンドウでは枠ごとに別マテリアルを `Assets/PokahiCameraDockSettings` に作ります。他シーンの映像を変更しません。再設定時にも新しいマテリアルを作るため、不要になった古い設定ファイルは参照を確認して整理してください。

## 画面の設定

| 項目 | 動作 |
|---|---|
| 余白なし | 縦横比を保って画面を埋めます。比率が異なると映像の端が切れます。 |
| 全体表示 | 映像全体を表示し、必要な上下または左右の余白を黒にします。 |
| 上下／左右反転 | 映像元に合わせて手動補正します。通常はオフです。 |
| 設置ガイド | カメラを入れる場所を示す枠です。不要なら非表示にできます。 |

## 対応範囲

- Unity 2022.3.22f1 / Built-in Render Pipeline / Windows向け。URP・HDRP・Quest・Android・iOSは対象外です。
- 通常視点・ミラーへの映像差し替えを除外します。判定には[VRChat公式のShader Globals](https://creators.vrchat.com/worlds/udon/vrc-graphics/vrchat-shader-globals/)を使用しています。
- カメラモード1・2と、手持ちカメラの位置に一致するモード3を対象にします。すべての撮影・配信経路への対応は保証しません。モード0として描画される経路は差し替えません。
- 通常のUnityカメラやSceneビューではVRCのカメラ判定がないため、映像差し替えは見えません。
- この配布版はUnityでの描画テスト済みです。配布版そのもののVRC実機テストは未実施です。導入先のBuild & Testでライブ表示と保存した写真を確認してください。
- シェーダーとPrefabは実行時のUdonや独自MonoBehaviourを必要としません。ワールドの公開自体にはVRChat Worlds SDKが必要です。

## 注意点

- RenderTextureを使い回すため、元映像以上に解像度が上がる機能ではありません。音声をカメラへ転送・録音する機能もありません。
- 動的にRenderTextureを別インスタンスへ切り替える再生システムでは、マテリアルの `_MainTex` も更新してください。
- ガイドはVRC手持ちカメラの通常描画・ミラーでは非表示ですが、映像元のUnityカメラには映り込む場合があります。撮影外へ配置するか、ガイドを非表示にしてください。
- 枠を重ねて置かないでください。複数の映像が競合する場合があります。
- 元のカメラ映像にUIやアバターが含まれていれば、そのまま映ります。それらを個別に除去する機能ではありません。

[詳しい手順とトラブル対処](Documentation/GUIDE.ja.md) / [検証記録](Documentation/VALIDATION.md)

## ライセンス

MIT License。モデル・音楽・ダンスモーション・他のワールドの素材は含みません。VRChat公式製品ではありません。
