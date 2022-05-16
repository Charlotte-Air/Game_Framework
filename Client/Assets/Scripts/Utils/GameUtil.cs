class GameUtil : Singleton<GameUtil>
{
    public bool InScreen(UnityEngine.Vector3 position) => UnityEngine.Screen.safeArea.Contains(UnityEngine.Camera.main.WorldToScreenPoint(position));
}