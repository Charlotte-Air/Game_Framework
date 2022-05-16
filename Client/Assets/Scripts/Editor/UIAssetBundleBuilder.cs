using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace SKY 
{
    public class UIAssetBundleBuilder : EditorWindow 
    {
        [MenuItem("AssetBundle/BuilderInterface")]
        public static void ShowWindow() {
            Rect wr = new Rect(500, 300, 400, 800);
            UIAssetBundleBuilder window = EditorWindow.GetWindowWithRect<UIAssetBundleBuilder>(wr, true, "BuilderInterface");
            window.Show();
        }

        private AssetBundlePack.PackType type;
        private List<string> lstExclude2;

        public void Awake() {
            type = GetAssetBundleTypeSelect();  // AssetBundlePack.PackType.WebPlayer;
            lstExclude2 = new List<string> { };
        }

        // 当前Unity选择的平台
        static AssetBundlePack.PackType GetAssetBundleTypeSelect() {
#if UNITY_WEBPLAYER
            return AssetBundlePack.PackType.WebPlayer;
#elif (UNITY_IOS || UNITY_IPHONE)
            return AssetBundlePack.PackType.iOS;
#elif UNITY_ANDROID
            return AssetBundlePack.PackType.Android;
#elif UNITY_STANDALONE
            return AssetBundlePack.PackType.Windows;
#else
            return AssetBundlePack.PackType.Windows;
#endif
        }

        void OnGUI() {
            GUILayout.Space(6f);
            EditorGUILayout.LabelField("制作AssetBundle包");
            type = (AssetBundlePack.PackType)EditorGUILayout.EnumPopup("请选择平台", type);

            EditorGUILayout.LabelField("");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Step 1:", GUILayout.Width(60));
            if (GUILayout.Button("生成txt表格")) {
                EditorApplication.delayCall = () => {
                    AssetBundleBuilder.GenerateTxtTblFiles();
                    AssetBundleMsgBox.ShowWindow("完成", "生成txt表格完成!");
                };
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Step 2:", GUILayout.Width(60));
            if (GUILayout.Button("将UI资源打成图集")) {
                EditorApplication.delayCall = () => {
                    AssetBundleBuilder.PackerAtlas();
                    AssetBundleMsgBox.ShowWindow("完成", "PackersAtlas完成!");
                };
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Step 3: ", GUILayout.Width(60));
            EditorGUILayout.LabelField("资源优化");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(60));
            if (GUILayout.Button("光照贴图优化"))
            {
                EditorApplication.delayCall = () =>
                {
                    AssetBundleBuilder.OptimizeLightmaps();
                    AssetBundleMsgBox.ShowWindow("完成", "光照贴图优化完成!");
                };
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Step 4:", GUILayout.Width(60));

            if (GUILayout.Button("清除资源"))
            {
                EditorApplication.delayCall = () =>
                {
                    AssetBundleBuilder.ClearStreamingAssets(type);
                    AssetBundleMsgBox.ShowWindow("完成", "清除资源完成!");
                };
            }

            if (GUILayout.Button("清除标签"))
            {
                EditorApplication.delayCall = () =>
                {
                    AssetBundleBuilder.ClearAssetBundle();
                    AssetBundleMsgBox.ShowWindow("完成", "清除所有标签完成!");
                };
            }
            if (GUILayout.Button("打标签")) {
                EditorApplication.delayCall = () => {
                    AssetBundleBuilder.MarkAssetBundle();
                    AssetBundleMsgBox.ShowWindow("完成", "打标签完成!");
                };
            }
            //if (GUILayout.Button("清标签")) {
            //    EditorApplication.delayCall = () => {
            //        AssetBundleBuilder.ClearAssetBundle();
            //        AssetBundleMsgBox.ShowWindow("完成", "清标签完成!");
            //    };
            //}
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Step 5:", GUILayout.Width(60));

            if (GUILayout.Button("资源打包")) {
                EditorApplication.delayCall = () => {
                    AssetBundleBuilder.BuildAssetBundlePack(type);
                    AssetBundleMsgBox.ShowWindow("完成", "资源打包完成!");
                };
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Step 6:", GUILayout.Width(60));
            EditorGUILayout.LabelField("排除场景(不打进AssetBundle包，大小写不敏感)");
            if (GUILayout.Button("+", GUILayout.Width(20))) {
                lstExclude2.Add("");
            }
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < lstExclude2.Count; i++) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(60));
                lstExclude2[i] = EditorGUILayout.TextField(lstExclude2[i]).ToLower();
                if (GUILayout.Button("-", GUILayout.Width(20))) {
                    lstExclude2.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(60));
            if (GUILayout.Button("场景打包")) {
                EditorApplication.delayCall = () => {
                    AssetBundleBuilder.BuildAllSceneAssetBundle(type, lstExclude2);
                    AssetBundleMsgBox.ShowWindow("完成", "场景打包完成!");
                };
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Step 7:", GUILayout.Width(60));
            if (GUILayout.Button("写版本信息")) {
                EditorApplication.delayCall = () => {
                    AssetBundleBuilder.WriteConfig(type);
                    AssetBundleMsgBox.ShowWindow("完成", "写版本信息完成!");
                };
            }
            EditorGUILayout.EndHorizontal();

            // if (GUILayout.Button("测试按键")) {
            //     Debug.Log(System.DateTime.Now.Date.ToString("MMdd"));
            //     ShowNotification(new GUIContent("测试完成"));
            // }

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("");
            if (GUILayout.Button("一键打包")) {
                AssetBundleBuilder.Build();
            }

            EditorGUILayout.LabelField("");
            if (GUILayout.Button("打开文件夹")) {
                var path = Application.dataPath;
                path = path.Replace("/", "\\");
                path = path.Replace("\\Assets", "\\Build");
                System.Diagnostics.Process.Start("explorer", path);
            }
            //if (GUILayout.Button("打包(exe制作)")) {
            //    Caching.ClearCache();
            //    build();
                
            //}
            
            //if (GUILayout.Button("一键打包(资源,场景,exe制作)")) {
            //    Caching.ClearCache();
            //    EditorCoroutineRunner.StartEditorCoroutine((Routine1()));  
            //}
        }
        void Update()
        {
           
            if (DateTime.Now.Hour == 11 && DateTime.Now.Minute == 0)
            {
                //Debug.Log(DateTime.Now.Hour);
            }
        }
        IEnumerator Routine1() {
            //FileUtil.DeleteFileOrDirectory(Application.dataPath + "/StreamingAssets/");
            AssetDatabase.Refresh();
            yield return 1;
            AssetBundleBuilder.ClearAll();
            AssetBundleMsgBox.ShowWindow("完成", "清除所有标签完成!");
            yield return 1;
            AssetBundleBuilder.MarkAssetBundle();
            AssetBundleMsgBox.ShowWindow("完成", "打标签完成!");
            yield return 1;
            AssetBundleBuilder.BuildAssetBundlePack(type);
            AssetBundleMsgBox.ShowWindow("完成", "资源打包完成!");
            yield return 1;
            AssetBundleBuilder.BuildAllSceneAssetBundle(type, lstExclude2);
            AssetBundleMsgBox.ShowWindow("完成", "场景打包完成!");
            yield return 1;
            AssetBundleBuilder.WriteConfig(type);
            AssetDatabase.Refresh();
            AssetBundleMsgBox.ShowWindow("完成", "写版本信息完成!");
            yield return 1;
            build();
            yield return 1;
            Check();
            yield return 1;
        }
        static string _build_path = "Build/";
        void build() {
            // 获取打包的场景列表
            // string[] levels = { "Assets/Scenes/initscene.unity" };
            string[] levels = AssetBundleBuilder._GetLevelsFromBuildSettings();

            if (!Directory.Exists(_build_path))
                Directory.CreateDirectory(_build_path);
#if QQ_GAME_PC
            //FileUtil.CopyFileOrDirectory(Application.dataPath + "/Plugins/Editor/QQDLL", "C:/Users/admin/Desktop/PC4");
            //string[] fileArray = { "config.ini", "msvcr120.dll", "msvcp120.dll" };
            //for (int i = 0; i < fileArray.Length; i++) {
            //    FileUtil.ReplaceFile(Application.dataPath + "/Plugins/Editor/QQDLL/" + fileArray[i], path + fileArray[i]);
            //}
            FileTool.CopyFolder(Application.dataPath + "/Plugins/Editor/QQDLL/", _build_path, new List<string> { ".meta" });
#endif
            BuildPipeline.BuildPlayer(levels, _build_path + "Raiden.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
            // BuildPipeline.BuildPlayer(levels, _build_path + "tututu22.exe", BuildTarget.StandaloneWindows, BuildOptions.ConnectWithProfiler);

            // FileTool.CopyFolder(Application.dataPath + "/DownLoadRes/", _build_path + "tututu22_Data/StreamingAssets/DownLoadRes/", new List<string> { ".meta" });

            AssetBundleMsgBox.ShowWindow("完成", "打包完成!");
        
            
        }

        static VersionConfig onlineConfig = new VersionConfig();
        static VersionConfig localConfig = new VersionConfig();
        //对比资源文件哪些更新哪些没有更新
        [MenuItem("Tools/Check for updated resources")]
        public static void Check() {
            //FTPTool.UploadData("C:/Users/admin/Desktop/PC4/UpdateRes/");
            EditorCoroutineRunner.StartEditorCoroutine(CheckRes());

        }

        [MenuItem("Tools/FTPToolUploadData")]
        public static void FTPToolUploadData()
        {
           // FTPTool.UploadData("C:/Users/admin/Desktop/PC4/UpdateRes/");
        }

        static IEnumerator CheckRes() {

            localConfig.fileMD5s.Clear();
            onlineConfig.fileMD5s.Clear();

            CommonDefine.ASSETBUNDLE_HTTP = Application.dataPath + "/StreamingAssets/";
            string path = "file://" + CommonDefine.GetVersionConfigPathWWW(CommonDefine._VESION_CONFIG);
            WWW localWWW = new WWW(path);

            while (!localWWW.isDone) {
                yield return null;
            }

            //yield return localWWW;

            if (localWWW.error != null) {
                Debug.LogError("LoadVersionConfig error:{0} " + localWWW.error);
                yield break;
            }


            localConfig.ReadFromBytes(localWWW.bytes);
            yield return 1;


            CommonDefine.ASSETBUNDLE_HTTP = "http://cdn.ttt.changic.cn/update/";
            path = CommonDefine.GetVersionConfigPathWWW(CommonDefine._VESION_CONFIG);
            WWW www = new WWW(path);

            while (!www.isDone) {
                yield return null;
            }
            //yield return www;
            if (www.error != null) {
                Debug.LogError("LoadVersionConfig error:{0} " + www.error);
                yield break;
            }

            onlineConfig.ReadFromBytes(www.bytes);
            yield return 1;


            CommonDefine.ASSETBUNDLE_HTTP = _build_path+"UpdateRes/";

            FileUtil.DeleteFileOrDirectory(CommonDefine.ASSETBUNDLE_HTTP);
            yield return 1;
            FileTool.CopyFolder(Application.dataPath + "/StreamingAssets/", CommonDefine.ASSETBUNDLE_HTTP, new List<string> { ".meta" });
            FileTool.CopyFolder(Application.dataPath + "/DownLoadRes/", CommonDefine.ASSETBUNDLE_HTTP + "/DownLoadRes/", new List<string> { ".meta" });
            yield return 1;
            foreach (var pair in onlineConfig.fileMD5s) {

                string filePath = pair.Key;
                string MD5 = pair.Value.ToString();
                FileMD5 localMD5;
                if (localConfig.fileMD5s.TryGetValue(filePath, out localMD5) && localMD5.ToString() == MD5) {
                    string file = CommonDefine.GetAssetBundleFullPath(filePath);
                    FileUtil.DeleteFileOrDirectory(file);
                }

            }

            yield return 1;

            DirectoryInfo dis = new DirectoryInfo(CommonDefine.ASSETBUNDLE_HTTP);

            int len = dis.GetDirectories().Length;
            FileTool.DeleteEmptyDir(CommonDefine.ASSETBUNDLE_HTTP);
            AssetBundleMsgBox.ShowWindow("完成", "检查更新资源完成!");
            yield return null;



        }


       
    
        void OnInspectorUpdate() {
            this.Repaint();
        }
    }


    public class AssetBundleMsgBox : EditorWindow {

        string strMsg;

        public static void ShowWindow(string title, string message) {
            Rect wr = new Rect(500, 300, 400, 200);
            AssetBundleMsgBox window = (AssetBundleMsgBox)EditorWindow.GetWindowWithRect(typeof(AssetBundleMsgBox), wr, true, title);
            window.strMsg = message;
            window.Show();
        }
        void OnGUI() {
            GUILayout.Space(10f);
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField(strMsg, style);
            if (GUILayout.Button("OK")) {
                Close();
            }
        }
    }

}













