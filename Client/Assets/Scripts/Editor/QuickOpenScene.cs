using UnityEditor;
using UnityEditor.SceneManagement;
 
public class QuickOpenScene
{
    [MenuItem("OpenScene/Loading")]
    static void OpenHotupdateScene()
    {
        EditorSceneManager.OpenScene("Assets/Scene/Loading.unity");
    }

    [MenuItem("OpenScene/CharSelect")]
    static void OpenInitScene()
    {
        EditorSceneManager.OpenScene("Assets/Scene/CharSelect.unity");
    }

    [MenuItem("OpenScene/Map01")]
    static void OpenNormalScene()
    {
        EditorSceneManager.OpenScene("Assets/Scene/Map01.unity");
    }

    [MenuItem("OpenScene/Map02")]
    static void OpenBattleScene()
    {
        EditorSceneManager.OpenScene("Assets/Scene/Map02.unity");
    }

    [MenuItem("OpenScene/Map03")]
    static void OpenHeroDisplayScene()
    {
        EditorSceneManager.OpenScene("Assets/Scene/Map03.unity");
    }

    [MenuItem("OpenScene/CharSelect")]
    static void OpenGalaxyScene()
    {
        EditorSceneManager.OpenScene("Assets/Scene/CharSelect.unity");
    }

    [MenuItem("OpenScene/TestScene")]
    static void OpenFollowPathScene()
    {
        EditorSceneManager.OpenScene("Assets/Scene/Test.unity");
    }
}