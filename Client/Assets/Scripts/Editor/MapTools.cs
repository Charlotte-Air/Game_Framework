using System.Collections;
using System.Collections.Generic;
using Common.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// 扩展菜单
/// 用于保存传送点位置输出成表文件
/// </summary>
public class MapTools
{
    [MenuItem("Map Tools/Export Teleporters")] //创建一级菜单
    public static void ExportTeleporters()
    {
		DataManager.Instance.Load(); //加载配置表
        Scene current = EditorSceneManager.GetActiveScene(); //先获取当前场景
        string currentScene = current.name;
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确认");
            return;
        }
        //List<TeleporterObject> allTeleporterObjects = new List<TeleporterObject>();
        foreach (var map in DataManager.Instance.Maps) //遍历地图
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity"; //根据地图名生成原始路径
            if (!System.IO.File.Exists(sceneFile)) //判断是否存在
            {
                Debug.LogWarningFormat("Scene { 0 } not existed!", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single); //打开场景
            TeleporterObject[] teleporters = GameObject.FindObjectsOfType<TeleporterObject>();
            foreach (var teleporter in teleporters) //遍历每个传送点
            {
                if (!DataManager.Instance.Teleporters.ContainsKey(teleporter.ID)) //检查配置表中的ID是否存在
                {
                    EditorUtility.DisplayDialog("错误",string.Format("地图: {0} 中的配置 Teleporter: [ {1} ] 中不存在",map.Value.Resource,teleporter.ID),"确定");
                    return;;
                }
                TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.ID];
                if (def.MapID != map.Value.ID) //判断地图ID是否正确
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图: {0} 中的配置 Teleporter: [ {1} ] MapID: { 2 } 错误", map.Value.Resource, teleporter.ID,def.MapID), "确定");
                    return;
                }
                def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
            }
        }
        DataManager.Instance.SaveTeleporters(); //写入文件
        EditorSceneManager.OpenScene("X:/@Air_MMORPG/Src/Client/Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("提示", "传送点导出完成", "确定");
    }

    [MenuItem("Map Tools/Export SpawnPoints")] //创建一级菜单
    public static void ExportSpawnPoints()
    {
        DataManager.Instance.Load(); //加载配置表
        Scene current = EditorSceneManager.GetActiveScene(); //先获取当前场景
        string currentScene = current.name;
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确认");
            return;
        }
        if (DataManager.Instance.SpawnPoints == null)
            DataManager.Instance.SpawnPoints = new Dictionary<int, Dictionary<int, SpawnPointDefine>>();
        foreach (var map in DataManager.Instance.Maps) //遍历地图
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity"; //根据地图名生成原始路径
            if (!System.IO.File.Exists(sceneFile)) //判断是否存在
            {
                Debug.LogWarningFormat("Scene { 0 } not existed!", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single); //打开场景
            SpawnPonint[] spawnPonints = GameObject.FindObjectsOfType<SpawnPonint>();
            if (!DataManager.Instance.SpawnPoints.ContainsKey(map.Value.ID)) //检查配置表中的ID是否存在
            {
                DataManager.Instance.SpawnPoints[map.Value.ID] = new Dictionary<int, SpawnPointDefine>();
            }
            foreach (var sp in spawnPonints)
            {
                if (!DataManager.Instance.SpawnPoints[map.Value.ID].ContainsKey(sp.ID))
                {
                    DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID] = new SpawnPointDefine();
                }
                SpawnPointDefine def = DataManager.Instance.SpawnPoints[map.Value.ID][sp.ID];
                def.ID = sp.ID; //ID
                def.MapID = sp.ID; //地图ID
                def.Position = GameObjectTool.WorldToLogicN(sp.transform.position); //方向
                def.Direction = GameObjectTool.WorldToLogicN(sp.transform.forward); //位置
            }
        }
        DataManager.Instance.SaveSpawnPoints(); //写入文件
        EditorSceneManager.OpenScene("X:/@Air_MMORPG/Src/Client/Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("提示", "刷怪点导出完成", "确定");
    }

    [MenuItem("Map Tools/Generate NavData")]
    public static void GenerateNavData()
    {
        Material red = new Material(Shader.Find("Particles/Alpha Blended"));
        red.color=Color.gray;
        red.SetColor("_TintColor",Color.gray);
        GameObject go = GameObject.Find("MinimapBoudingBox");
        if (go != null)
        {
            GameObject root = new GameObject("Root");
            BoxCollider bound = go.GetComponent<BoxCollider>();
            float step = 1f;
            for (float x = bound.bounds.min.x; x < bound.bounds.max.x; x +=step)
            {
                for (float z = bound.bounds.min.z; z < bound.bounds.max.z; z += step)
                {
                    for (float y = bound.bounds.min.y; y < bound.bounds.max.y + 5f; y += step)
                    {
                        var pos = new Vector3(x, y, z);
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(pos, out hit, 0.5f, NavMesh.AllAreas))
                        {
                            if (hit.hit)
                            {
                                var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                box.name = "Hit" + hit.mask;
                                box.GetComponent<MeshRenderer>().sharedMaterial = red;
                                box.transform.SetParent(root.transform, true);
                                box.transform.position = pos;
                                box.transform.localScale = Vector3.one * 0.9f;
                            }
                        }
                    }
                }
                    
            }
        }
    }
}
