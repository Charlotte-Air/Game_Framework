using UnityEditor;
using UnityEditor.SceneManagement;
 
public class QuickOpenScene
{
    [MenuItem("OpenScene/Loading")]
    static void OpenHotupdateScene()
    {
        EditorSceneManager.OpenScene("Assets/Levels/Loading.unity");
    }

    [MenuItem("OpenScene/CharSelect")]
    static void OpenInitScene()
    {
        EditorSceneManager.OpenScene("Assets/Levels/CharSelect.unity");
    }

    [MenuItem("OpenScene/Map01")]
    static void OpenNormalScene()
    {
        EditorSceneManager.OpenScene("Assets/Levels/Map01.unity");
    }

    [MenuItem("OpenScene/Map02")]
    static void OpenBattleScene()
    {
        EditorSceneManager.OpenScene("Assets/Levels/Map02.unity");
    }

    [MenuItem("OpenScene/Map03")]
    static void OpenHeroDisplayScene()
    {
        EditorSceneManager.OpenScene("Assets/Levels/Map03.unity");
    }

    [MenuItem("OpenScene/CharSelect")]
    static void OpenGalaxyScene()
    {
        EditorSceneManager.OpenScene("Assets/Levels/CharSelect.unity");
    }

    [MenuItem("OpenScene/TestScene")]
    static void OpenFollowPathScene()
    {
        EditorSceneManager.OpenScene("Assets/Levels/Test.unity");
    }
}