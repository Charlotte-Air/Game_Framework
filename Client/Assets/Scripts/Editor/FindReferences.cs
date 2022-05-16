using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class FindReferences
{
    [MenuItem("FindAssets/Find References All", false, 10)]
    static private void FindAll()
    {
        Find(new List<string>(){".prefab",".unity",".mat",".asset"});
    }
 
    [MenuItem("FindAssets/Find References in .prefab", false, 11)]
    static private void FindPrefabs()
    {
        Find(new List<string>(){".prefab"});
    }

    [MenuItem("FindAssets/Find References in .unity", false, 12)]
    static private void FindScenes()
    {
        Find(new List<string>(){".unity"});
    }

    [MenuItem("FindAssets/Find References in .mat", false, 13)]
    static private void FindMaterials()
    {
        Find(new List<string>(){".mat"});
    }

    [MenuItem("FindAssets/Find References in .asset", false, 14)]
    static private void FindAssets()
    {
        Find(new List<string>(){".asset"});
    }

    static private void Find(List<string> withoutExtensions)
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!string.IsNullOrEmpty(path))
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
                .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
            int startIndex = 0;
 
            EditorApplication.update = delegate()
            {
                string file = files[startIndex];
            
                 bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);
 
                if (Regex.IsMatch(File.ReadAllText(file), guid))
                {
                    Debug.Log(file, AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file)));
                }
 
                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                    Debug.Log("匹配结束");
                }
 
            };
        }
    }

    static private bool VFind()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return (!string.IsNullOrEmpty(path));
    }
 
    static private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }
}