using UnityEngine;
using UnityEngine.U2D;
using System.Collections;
using UnityEditor;
using UnityEditor.U2D;
using System.IO;
using System.Text;
using System.Collections.Generic;

// using ComponentAce.Compression.Libs.zlib;

namespace SKY
{
    public static class AssetBundleBuilder
    {
        public static void BuildAssetBundlePack(AssetBundlePack.PackType type)
        {
            MarkAssetBundle();
            string output_dir = Application.streamingAssetsPath + "/" + AssetBundlePack.GetName(type);
            if (!Directory.Exists(output_dir))
                Directory.CreateDirectory(output_dir);
            _BuildAssetBundle(output_dir, type);
        }

        private static BuildTarget _GetBuildTarget(AssetBundlePack.PackType type)
        {
            switch(type)
            {
                case AssetBundlePack.PackType.Windows:
                    return BuildTarget.StandaloneWindows;
                case AssetBundlePack.PackType.iOS:
                    return BuildTarget.iOS;
                case AssetBundlePack.PackType.Android:
                    return BuildTarget.Android;
                // case AssetBundlePack.PackType.WebPlayer:
                //     return BuildTarget.WebPlayer;
            }
            return BuildTarget.StandaloneWindows;
        }

        internal struct Bundle4Json
        {
            public string f;
            public string[] d;
        }

        internal struct Bundle4JsonList
        {
            public List<Bundle4Json> bundle;
        }

        private static void _BuildAssetBundle(string dir, AssetBundlePack.PackType type)
        {
            AssetBundleManifest manifest = null;
            switch(type)
            {
                case AssetBundlePack.PackType.Windows:
                case AssetBundlePack.PackType.iOS:
                case AssetBundlePack.PackType.Android:
                    manifest = BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.ChunkBasedCompression, _GetBuildTarget(type));
                    break;
                case AssetBundlePack.PackType.WebPlayer:
                    manifest = BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, _GetBuildTarget(type));
                    break;
            }

            Bundle4JsonList t = new Bundle4JsonList();
            List<Bundle4Json> lst = t.bundle = new List<Bundle4Json>();
            var allAssetBundles = manifest.GetAllAssetBundles();
            for (int j = 0; j < allAssetBundles.Length; j++)
            {
                var ab = allAssetBundles[j];
                Bundle4Json bundle = new Bundle4Json();
                bundle.f = ab;
                var deps = manifest.GetAllDependencies(ab);
                bundle.d = new string[deps.Length];
                for (int i = 0; i < deps.Length; i++)
                {
                    bundle.d[i] = deps[i];
                }
                lst.Add(bundle);
            }
            string buf = JsonTool.ToJson(t);

            string filePath = Application.streamingAssetsPath + "/" + AssetBundlePack.GetName(type) + "/" + CommonDefine._BUNDLE_FILE;

            FileStream stream = File.Open(filePath, FileMode.Create);
            var bytes = System.Text.Encoding.UTF8.GetBytes(buf);
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
            Debug.Log("Bundle file save to " + filePath);
        }

        public static void GenerateTxtTblFiles()
        {
            string DIR = CommonDefine.ASSETBUNDLE_DIR;
            Debug.Log("Start GenerateTxtTblFiles");
            if (Directory.Exists(DIR))
            {
                _CopyDataToData1();
            }
        }

        private static bool DeleteAllFile(string fullPath)
        {
            //获取指定路径下面的所有资源文件  然后进行删除
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo direction = new DirectoryInfo(fullPath);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
        
                Debug.Log(files.Length);
                int count = files.Length;
                for (int i = 0; i < count; i++)
                {
                    string FilePath = fullPath + "/" + files[i].Name;
                    File.Delete(FilePath);
                }
                return true;
            }
            return false;
        }

        private static void _CopyDataToData1()
        {
            Debug.Log("Start Copy Data To Data1");
            string DIR = CommonDefine.ASSETBUNDLE_DIR + "data/";
            string DIR1 = CommonDefine.ASSETBUNDLE_DIR + "data1/";
            if (!Directory.Exists(DIR)) {
                Debug.LogWarning(DIR + " is not exist!");
                return;
            }
            
            if (!Directory.Exists(DIR1)) Directory.CreateDirectory(DIR1);
            DeleteAllFile(DIR1);

            var dirs = Directory.GetDirectories(DIR);
            DirectoryInfo direction = new DirectoryInfo(DIR);
            FileInfo[] files = direction.GetFiles("*.tbl", SearchOption.AllDirectories);
            Debug.Log("data files count: " + files.Length);
            string str = string.Empty;
            string allNameStr = string.Empty;
            for(int i = 0; i < files.Length; i++)
            {
                var fileName = files[i].Name;
                if (fileName.EndsWith(".tbl"))
                {
                    var fileContent = File.ReadAllText(DIR + fileName, Encoding.UTF8);
                    var path = DIR1 + fileName + ".txt";
                    allNameStr += "data1/" +fileName + ".txt\n";
                    File.WriteAllText(path, fileContent, Encoding.UTF8);
                }
            }
            allNameStr = allNameStr.Substring(0, allNameStr.Length - 1);
            File.WriteAllText(DIR1 + "alltbllist.txt", allNameStr, Encoding.UTF8);
        }

        public static void MarkAssetBundle()
        {
            string DIR = CommonDefine.ASSETBUNDLE_DIR;
            if (Directory.Exists(DIR))
            {
                //MarkByPathIncludeAll(DIR, "test" + CommonDefine.ASSETBUNDLE_SUFFIX);
                _MarkByPath(DIR);
            }
        }

        private static void _MarkByPath(string path)
        {
            _MarkCurDirectoryFile(path, CommonDefine.PathToAssetBundleName(path));
            var dirs = Directory.GetDirectories(path);
            for(int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                string newStr = dir.Replace("\\","/");
                if (newStr == "Assets/AssetBundle/data"|| newStr.Contains("Assets/AssetBundle/ui/atlases"))// 排除data和atlases目录
                    continue;
                if (newStr.Contains(".svn"))
                    continue;

                _MarkByPath(dir);
            }
        }

        private static void _MarkByPathIncludeAll(string path, string assetBundleName)
        {
            var dirs = Directory.GetDirectories(path);
            for(int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                if (!dir.Contains(".svn"))
                    _MarkByPathIncludeAll(dir, assetBundleName);
            }

            _MarkCurDirectoryFile(path, assetBundleName);
        }

        private static void _MarkCurDirectoryFile(string path, string assetBundleName)
        {
            path = path.Replace('\\', '/');

            var strFiles = Directory.GetFiles(path);
            string fileName;
            string markFile;
            AssetImporter importer;
            for (int i = 0; i < strFiles.Length; i++)
            {
                if (strFiles[i].Contains(".svn") || strFiles[i].EndsWith(".meta"))
                    continue;

                fileName = Path.GetFileName(strFiles[i]);
                if (fileName.StartsWith("."))
                {
                    continue;
                }
                markFile = strFiles[i].Replace('\\', '/');
                importer = AssetImporter.GetAtPath(markFile);
                if (importer != null && importer.assetBundleName != null && importer.assetBundleName != assetBundleName)
                {
                    importer.assetBundleName = assetBundleName;
                }
            }
        }

        public static void ClearStreamingAssets(AssetBundlePack.PackType type)
        {
            string filePath = Application.streamingAssetsPath + "/" + AssetBundlePack.GetName(type);
            if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath, true);
            }
        }

        public static void ClearAssetBundle()
        {
            string DIR = CommonDefine.ASSETBUNDLE_DIR;
            if (Directory.Exists(DIR))
            {
                _ClearByPath(DIR);
            }
        }

        public static void ClearAll()
        {
            string DIR = "Assets/";
            _ClearByPath(DIR);
        }

        // 清除除了AssetBundle目录的标签
        public static void ClearAllExceptAssetBundle()
        {
            string DIR = "Assets/";
            var dirs = Directory.GetDirectories(DIR);
            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                if (dir != "Assets/AssetBundle")
                {
                    _ClearByPath(dirs[i]);
                }
            }
        }

        private static void _ClearByPath(string path)
        {
            var dirs = Directory.GetDirectories(path);
            for (int i = 0; i < dirs.Length; i++)
                _ClearByPath(dirs[i]);

            _ClearAllFile(path);
        }

        private static void _ClearAllFile(string path)
        {
            var strFiles = Directory.GetFiles(path);
            string fileName;
            string markFile;
            AssetImporter importer;
            for (int i = 0; i < strFiles.Length; i++)
            {
                if (strFiles[i].Contains(".svn") || strFiles[i].EndsWith(".meta") || strFiles[i].EndsWith(".cs") || strFiles[i].EndsWith(".js"))
                    continue;

                fileName = Path.GetFileName(strFiles[i]);
                if (fileName.StartsWith("."))
                {
                    continue;
                }
                markFile = strFiles[i].Replace('\\', '/');
                importer = AssetImporter.GetAtPath(markFile);
                if (importer != null && (importer.assetBundleName != null || importer.assetBundleName != ""))
                {
                    importer.assetBundleName = null;
                }
            }
        }

        [MenuItem(CommonDefine.SIMULATE_ASSETBUNDLE_MENU_NAME)]
        public static void ToggleSimulateAssetBundle()
        {
            ResourceManager.simulateAssetBundleInEditor = !ResourceManager.simulateAssetBundleInEditor;
            EditorPrefs.SetBool("SimulateAssetBundle", ResourceManager.simulateAssetBundleInEditor);
        }

        [MenuItem(CommonDefine.SIMULATE_ASSETBUNDLE_MENU_NAME, true)]
        public static bool ToggleSimulateAssetBundleValidate()
        {
            Menu.SetChecked(CommonDefine.SIMULATE_ASSETBUNDLE_MENU_NAME, ResourceManager.simulateAssetBundleInEditor);
            return true;
        }

        public static void WriteConfig(AssetBundlePack.PackType type)
        {
            string dir = Application.streamingAssetsPath + "/" + AssetBundlePack.GetName(type);
            Debug.Log("dir = "+ dir);
            VersionConfig cfg = new VersionConfig(dir);
            cfg.SaveToFile(type);
            writeVersion(type);
            writeSdkRes2AndroidAssets();
        }

        private static void writeVersion(AssetBundlePack.PackType type)
        {
            string versionReadFile = Application.dataPath + "/AssetBundle/data1/version.tbl.txt";
            string versionFile = Application.streamingAssetsPath + "/" + AssetBundlePack.GetName(type) + "/version/version.txt";
            string versionStr = "";
            try {
                using (FileStream fs = File.OpenRead(versionReadFile))
                {
                    byte[] b = new byte[1024];
                    UTF8Encoding temp = new UTF8Encoding(true);
                    while (fs.Read(b, 0, b.Length) > 0)
                    {
                        versionStr = temp.GetString(b);
                    }
                }
                if (string.IsNullOrEmpty(versionStr))
                {
                    Debug.LogError("读版本错误");
                    return;
                }
                string[] strContent = versionStr.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
                if (strContent.Length >= 4)
                {
                    string[] lineData = strContent[3].Split('\t');
                    if (lineData.Length >= 2)
                    {
                        versionStr = lineData[1];
                    }
                }
                using (FileStream fs = File.Create(versionFile))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(versionStr);
                    fs.Write(info, 0, info.Length);
                }
            }
            catch
            {
                Debug.LogError("读写版本错误");
            }
        }

        private static void writeSdkRes2AndroidAssets()
        {
            string orignPath = Application.dataPath + "/GameRes/sdk_assets_res";
            string destPath = Application.dataPath + "/StreamingAssets";
            if (!Directory.Exists(orignPath) || !Directory.Exists(destPath)) return;
            CopyDirectory(orignPath, destPath);
        }

        public static void CopyDirectory(string srcPath, string destPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                for (int j = 0; j <  fileinfo.Length; j++)
                {
                    var i = fileinfo[j];
                    if (i.Name.Contains(".svn"))
                    {
                        continue;
                    }
                    if (i is DirectoryInfo)
                    {     //判断是否文件夹
                        if (!Directory.Exists(destPath + "\\" + i.Name))
                        {
                            Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                        }
                        CopyDirectory(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                    }
                    else
                    {
                        File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("copy wrong "+e.Message);
                throw;
            }
        }

        // 压缩场景函数
        public static void BuildAllSceneAssetBundleZip(AssetBundlePack.PackType type, List<string> exclude = null)
        {
            string[] levels = _GetLevelsFromBuildSettings();
            if (levels.Length == 0)
            {
                Debug.Log("Nothing to build.");
                return;
            }

            var pathRoot = Application.dataPath + "/AssetBundle/ZipStream";
            if (Directory.Exists(pathRoot)) {
                Directory.Delete(pathRoot, true);
            }

            var path = Path.Combine(Application.streamingAssetsPath, AssetBundlePack.GetName(type)) + "/scenes/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            for (int j = 0; j < levels.Length; j++)
            {
                var unit = levels[j];
                string unitName = Path.GetFileNameWithoutExtension(unit).ToLower();
                if (exclude != null && exclude.Contains(unitName))
                {
                    continue;
                }

                //string targetName = path + unitName + ".unity3d";
                //BuildPipeline.BuildPlayer(new string[] { unit }, targetName, _GetBuildTarget(type), BuildOptions.BuildAdditionalStreamedScenes);


                UnityEngine.SceneManagement.Scene scece = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(unit);
                for (int i = 0; i < scece.GetRootGameObjects().Length; i++) {
                    Debug.Log(scece.GetRootGameObjects()[i].name);
                }

                string targetTemp = path + unitName + ".unity3d";
                BuildPipeline.BuildPlayer(new string[] { unit }, targetTemp, _GetBuildTarget(type), BuildOptions.BuildAdditionalStreamedScenes);

                string output_dir = Path.Combine(pathRoot, unitName);
                if (!Directory.Exists(output_dir))
                    Directory.CreateDirectory(output_dir);

                //ZipTool.ZipFile(targetTemp, output_dir + "/" + unitName + ".bytes", string.Empty);
            }

            //Directory.Delete(path, true);
        }

        // 不压缩场景
        public static void BuildAllSceneAssetBundle(AssetBundlePack.PackType type, List<string> exclude = null) {
            string[] levels = _GetLevelsFromBuildSettings();
            if (levels.Length == 0) {
                Debug.Log("Nothing to build.");
                return;
            }

            var path = Path.Combine(Application.streamingAssetsPath, AssetBundlePack.GetName(type)) + "/scenes/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            for (int i = 0; i < levels.Length; i++) {
                var unit = levels[i];
                string unitName = Path.GetFileNameWithoutExtension(unit).ToLower();
                if (exclude != null && exclude.Contains(unitName)) {
                    continue;
                }

                string targetName = path + unitName + ".unity3d";
                BuildPipeline.BuildPlayer(new string[] { unit }, targetName, _GetBuildTarget(type), BuildOptions.BuildAdditionalStreamedScenes);
                // BuildPipeline.BuildPlayer(new string[] { unit }, targetName, BuildTarget.WebPlayer, BuildOptions.BuildAdditionalStreamedScenes);                
            }
        }

        public static string[] _GetLevelsFromBuildSettings()
        {
            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                {
                    levels.Add(EditorBuildSettings.scenes[i].path);
                }
            }
            return levels.ToArray();
        }

//         [MenuItem("")]
//         public static void ClearCache()
//         {
// 
//         }

        public static void PackerAtlas(){
            if (Directory.Exists(CommonDefine.ASSETBUNDLE_ATLASTEXTURE_DIR))
            {
                Directory.Delete(CommonDefine.ASSETBUNDLE_ATLASTEXTURE_DIR, true);
                Directory.CreateDirectory(CommonDefine.ASSETBUNDLE_ATLASTEXTURE_DIR);
            }

            string DIR = CommonDefine.ASSETBUNDLE_UIIMAGE_DIR;
            if (Directory.Exists(DIR))
            {
                PackPath(DIR);
            }
        }

        private static void PackPath(string resPath)
        {
            _packCurDirectoryTexture(resPath);
            var dirs = Directory.GetDirectories(resPath);
            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                if (dir.Contains(".svn"))
                    continue;

                string _newdir = dir.Replace(@"\","/");
                Debug.Log("_newdir = " +_newdir);
                PackPath(_newdir);
            }
        }

        private static void _packCurDirectoryTexture(string folderPath) {
            List<Object> assets = FilterAssets(folderPath);

            if (assets.Count > 0)
            {
                string[] atlasPath = AtlasPath(folderPath);
                if(!Directory.Exists(atlasPath[0])) {
                    Directory.CreateDirectory(atlasPath[0]);
                }
                SpriteAtlas atlas = GenerateAtlas();
                atlas.Add(assets.ToArray());
                //if (AssetDatabase.DeleteAsset(atlasPath[1]))
                //    Debug.Log(string.Format("Deleta Old Atlas: {0}", atlasPath[1]));
                AssetDatabase.CreateAsset(atlas, atlasPath[1]);
                
                Debug.Log(string.Format("Packing Path: {0} into one Atlas: {1} ", folderPath, atlasPath[1]));
            }
        }

        static string[] AtlasPath(string resFolderPath) 
        {
            string _atlasPath = resFolderPath.Replace("uitexture","atlases");

            Debug.Log("resFolderPath = "+resFolderPath);
            string[] arr = resFolderPath.Split('/');
            string atlasName = arr[arr.Length-1];

            return new string[]{_atlasPath,_atlasPath + "/" + atlasName + ".spriteatlas"};
        }

        static List<Object> FilterAssets(string folderPath) {
            List<Object> objects = new List<Object>();
            DefaultAsset folderAsset = AssetDatabase.LoadAssetAtPath(folderPath, typeof(DefaultAsset)) as DefaultAsset;

            List<string> filterFiles = null;
            //文件夹过滤
            if (atlasWhiteDic.Count > 0 && atlasWhiteDic.ContainsKey(folderPath))
            {
                if (atlasWhiteDic[folderPath].Count == 0)
                {
                    return objects;
                }
                filterFiles = atlasWhiteDic[folderPath];
            }
            
            if (Directory.Exists(folderPath)) {
                DirectoryInfo dir = new DirectoryInfo(folderPath);
                FileInfo[] files = dir.GetFiles("*", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {
                    var fi = files[i];
                    //文件过滤
                    if (filterFiles != null)
                    {
                        bool bIngoneImg = false;
                        for (int j = 0; j < filterFiles.Count; j++)
                        {
                            var imageName = filterFiles[j];
                            if (imageName.Equals(fi.Name))
                            {
                                bIngoneImg = true;
                                break;
                            }
                        }
                        if (bIngoneImg)
                        {
                            continue;
                        }
                    }
                    string spritePath = FullPath2Relative(fi.FullName);
                    Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                    
                    if (sp != null)
                    {
                        if (sp.rect.width < 1024 && sp.rect.height < 1024)
                        {
                            objects.Add(sp);
                        }
                    }

                }
            }
            return objects;
        }

        static string FullPath2Relative(string fullPath) {
            string relativePath = fullPath.Substring(fullPath.IndexOf("Assets"));
            relativePath = relativePath.Replace("\\", "/");
            return relativePath;
        }

        
        // 设置图集的参数 可根据项目具体情况进行设置
        private static SpriteAtlas GenerateAtlas()
        {
            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = true,
                enableTightPacking = false,
                padding = 2,
            };

            SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };

            SpriteAtlas atlas = new SpriteAtlas();
            atlas.SetPackingSettings(packSetting);
            atlas.SetTextureSettings(textureSetting);
            ApplyTexturePlatFormCompressSettingDefault(atlas);
            atlas.SetIncludeInBuild(true);
            atlas.SetIsVariant(false);
            return atlas;
        }

        private static void ApplyTexturePlatFormCompressSettingDefault(SpriteAtlas atlas)
        {
            TextureImporterPlatformSettings textureCompressDefault = new TextureImporterPlatformSettings();
            textureCompressDefault.overridden = false;
            textureCompressDefault.name = "DefaultTexturePlatform";
            textureCompressDefault.maxTextureSize = 1024;
            textureCompressDefault.textureCompression = TextureImporterCompression.Compressed;
            textureCompressDefault.format = TextureImporterFormat.ETC2_RGBA8Crunched;
            //textureCompressDefault.compressionQuality = (int)UnityEngine.TextureCompressionQuality.Best;
            atlas.SetPlatformSettings(textureCompressDefault);

            TextureImporterPlatformSettings textureCompressIOS = new TextureImporterPlatformSettings();
            textureCompressIOS.name = "iPhone";
            textureCompressIOS.overridden = true;
            textureCompressIOS.maxTextureSize = 1024;
            textureCompressIOS.textureCompression = TextureImporterCompression.Compressed;
            textureCompressIOS.format = TextureImporterFormat.ETC2_RGBA8Crunched;
            //textureCompressIOS.compressionQuality = (int)UnityEngine.TextureCompressionQuality.Best;
            atlas.SetPlatformSettings(textureCompressIOS);

            TextureImporterPlatformSettings textureCompressAndroid = new TextureImporterPlatformSettings();
            textureCompressAndroid.name = "Android";
            textureCompressAndroid.overridden = true;
            textureCompressAndroid.maxTextureSize = 1024;
            textureCompressAndroid.textureCompression = TextureImporterCompression.Compressed;
            textureCompressAndroid.format = TextureImporterFormat.ETC2_RGBA8Crunched;
            //textureCompressAndroid.compressionQuality = (int)UnityEngine.TextureCompressionQuality.Best;
            atlas.SetPlatformSettings(textureCompressAndroid);

        }

        /// <summary>
        /// 图集白名单
        /// 这些路径下资源不自动打包图集
        /// </summary>
        private static Dictionary<string, List<string>> atlasWhiteDic = new Dictionary<string, List<string>>()
        {
            {"Assets/AssetBundle/ui/uitexture/imgbg" ,new List<string>(){
                //"chapter_bg.png"
            } },
        };

        // 光照贴图优化
        public static void OptimizeLightmaps() {
            if (Directory.Exists(CommonDefine.LIGHTMAP_DIR))
            {
                _OptimizeByPath(CommonDefine.LIGHTMAP_DIR);
            }
        }

        private static void _OptimizeByPath(string path)
        {
            _ModifyCurDirectoryLightmapInfo(path);
            var dirs = Directory.GetDirectories(path);
            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                _OptimizeByPath(dir);
            }
        }

        // 修改当前文件夹下的Lightmap
        private static void _ModifyCurDirectoryLightmapInfo(string path)
        {
            path = path.Replace('\\', '/');

            var strFiles = Directory.GetFiles(path);

            for (int i = 0; i < strFiles.Length; i++)
            {
                if (strFiles[i].Contains(".svn") || strFiles[i].EndsWith(".meta"))
                    continue;

                string fileName = Path.GetFileName(strFiles[i]);
                if (fileName.Contains(".exr") && fileName.StartsWith("Lightmap"))
                {
                    _ModifyLightmapInfo(strFiles[i].Replace('\\', '/'));
                }                
            }
        }

        private static void _ModifyLightmapInfo(string _path) {
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(_path);
            if (importer != null && importer.textureType == TextureImporterType.Lightmap)
            {
                TextureImporterPlatformSettings _settings = importer.GetDefaultPlatformTextureSettings();
                _settings.maxTextureSize = 512;
                _settings.format = TextureImporterFormat.Automatic;
                importer.SetPlatformTextureSettings(_settings);
                importer.streamingMipmaps = false;
                importer.mipmapEnabled = false;
                //AssetDatabase.ImportAsset(_path);
            }
        }
 
        static float timeStamp = 0;
        public static long GetTimeStamp()
        {
            System.TimeSpan ts = System.DateTime.Now - new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            return System.Convert.ToInt64(ts.TotalMilliseconds);
        }

        public static string GetDate()
        {
            System.TimeSpan ts = System.DateTime.Now - new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.Days.ToString();
        }
        
        static IEnumerator Routine1(AssetBundlePack.PackType type) {
            //FileUtil.DeleteFileOrDirectory(Application.dataPath + "/StreamingAssets/");
            AssetDatabase.Refresh();
            yield return 1;
            GenerateTxtTblFiles();
            Debug.Log("生成txt表格完成");
            yield return 1;
            PackerAtlas();
            Debug.Log("图集打包完成!");
            yield return 1;
            OptimizeLightmaps();
            Debug.Log("光照贴图优化完成!");
            yield return 1;
            ClearAllExceptAssetBundle();
            Debug.Log("清除所有标签完成!");
            yield return 1;
            BuildAssetBundlePack(type);
            Debug.Log("资源打包完成!");
            yield return 1;
            BuildAllSceneAssetBundle(type);
            Debug.Log("场景打包完成!");
            // yield return 1;
            WriteConfig(type);
            AssetDatabase.Refresh();
            Debug.Log("写版本信息完成!");
            yield return 1;
            buildPlatform(type);
            Debug.Log("打包完成!");
            yield return 1;
            // Check();
            // yield return 1;
        }

        static void Routine2(AssetBundlePack.PackType type) {
            //FileUtil.DeleteFileOrDirectory(Application.dataPath + "/StreamingAssets/");
            
            // ClearStreamingAssets(type);
            // AssetDatabase.Refresh();
            // GenerateTxtTblFiles();
            // AssetDatabase.Refresh();
            // Debug.Log("生成txt表格完成");
            // PackerAtlas();
            // Debug.Log("图集打包完成!");
            // AssetDatabase.Refresh();
            // OptimizeLightmaps();
            // Debug.Log("光照贴图优化完成!");
            // ClearAllExceptAssetBundle();
            // AssetDatabase.Refresh();
            // Debug.Log("清除所有标签完成!");
            // AssetDatabase.Refresh();
            Debug.Log("开始资源打包");
            BuildAssetBundlePack(type);
            Debug.Log("资源打包完成!");
            // BuildAllSceneAssetBundle(type);
            // Debug.Log("场景打包完成!");
            // yield return 1;
            WriteConfig(type);
            AssetDatabase.Refresh();
            Debug.Log("写版本信息完成!");
            buildPlatform(type);
            Debug.Log("打包完成!");
            // Check();
        }

        public static void BuildBefore()
        {
            // 获取外部传参
            string[] args = System.Environment.GetCommandLineArgs();
            var type = AssetBundlePack.PackType.Android;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "SKY.AssetBundleBuilder.Build")
                {
                    var versionStr = args[i + 1];
                    var platformStr = args[i + 2];
                    if (platformStr == "IOS")
                    {
                        type = AssetBundlePack.PackType.iOS;
                    }
                    else if (platformStr == "Windows")
                    {
                        type = AssetBundlePack.PackType.Windows;
                    }
                }
            }
            ClearStreamingAssets(type);
            AssetDatabase.Refresh();
            GenerateTxtTblFiles();
            AssetDatabase.Refresh();
            Debug.Log("生成txt表格完成");
            PackerAtlas();
            Debug.Log("图集打包完成!");
            AssetDatabase.Refresh();
            OptimizeLightmaps();
            Debug.Log("光照贴图优化完成!");
            ClearAllExceptAssetBundle();
            AssetDatabase.Refresh();
            Debug.Log("清除所有标签完成!");
            AssetDatabase.Refresh();
        }

        static string _build_path = "Build/";
        public static void Build()
        {
            // 获取外部传参
            string[] args = System.Environment.GetCommandLineArgs();
            var type = AssetBundlePack.PackType.Android;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "SKY.AssetBundleBuilder.Build")
                {
                    var versionStr = args[i + 1];
                    var platformStr = args[i + 2];
                    if (platformStr == "IOS")
                    {
                        type = AssetBundlePack.PackType.iOS;
                    }
                    else if (platformStr == "Windows")
                    {
                        type = AssetBundlePack.PackType.Windows;
                    }
                }
            }

            Debug.Log("Start Build");
            // EditorCoroutineRunner.StartEditorCoroutine((Routine1(type)));
            Routine2(type);
        }

        public static void buildPlatform(AssetBundlePack.PackType type)
        {
            string[] levels = _GetLevelsFromBuildSettings();
            var branchStr = "_trunk_";
            var dateStr = System.DateTime.Now.ToString("MMdd_HHmm");
            var name = "raiden_" + branchStr + dateStr;
            switch(type)
            {
                case AssetBundlePack.PackType.Android:
                    BuildPipeline.BuildPlayer(levels, _build_path + name + ".apk", BuildTarget.Android, BuildOptions.None);
                    break;
                case AssetBundlePack.PackType.iOS:
                    BuildPipeline.BuildPlayer(levels, _build_path + name + ".ipa", BuildTarget.iOS, BuildOptions.None);
                    break;
                case AssetBundlePack.PackType.Windows:
                    BuildPipeline.BuildPlayer(levels, _build_path + name + ".exe", BuildTarget.StandaloneWindows, BuildOptions.None);
                    break;
            }
        }
    }
}
