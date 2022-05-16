/// <summary>
/// 资源加载器
/// </summary>
class Resloader
{
    /// <summary>
    /// 加载
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    /// <param name="path">路径</param>
    /// <returns></returns>
    public static T Load<T>(string path) where T : UnityEngine.Object => UnityEngine.Resources.Load<T>(path);
}