using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadScene : MonoBehaviour
{
    private Slider loadingSliderA;
    private Slider loadingSliderB;
    private float loadingSpeed = 1;
    private float targetValue;
    public AsyncOperation operation;

    void Start()
    {
        SceneManager.Instance.Load += Load;
        this.loadingSliderA = GameObject.FindGameObjectWithTag("LoginSliderA").GetComponent<Slider>();
        this.loadingSliderB = GameObject.FindGameObjectWithTag("LoginSliderB").GetComponent<Slider>();
        this.loadingSliderA.value = 0.0f;
        this.loadingSliderB.value = 0.0f;
        StartCoroutine(AsyncLoading());
    }

    void OnDestroy()
    {
        SceneManager.Instance.Load -= Load;
    }

    IEnumerator AsyncLoading()
    {
        operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GlobeScene.nextSceneName);
        operation.allowSceneActivation = false; //阻止当加载完成自动切换
        yield return operation;
    }

    void Update()
    {
        if (operation != null)
        {
            targetValue = operation.progress;
            if (operation.progress >= 0.9f)
            {
                targetValue = 1.0f;
            }
            if (targetValue != loadingSliderA.value)
            {
                loadingSliderA.value = Mathf.Lerp(loadingSliderA.value, targetValue, Time.deltaTime* loadingSpeed);
                loadingSliderB.value = Mathf.Lerp(loadingSliderB.value, targetValue, Time.deltaTime* loadingSpeed);
                if (Mathf.Abs(loadingSliderB.value - targetValue) < 0.01f)
                {
                    loadingSliderA.value = targetValue;
                    loadingSliderB.value = targetValue;
                }
            }
            if ((int)(loadingSliderB.value * 100) == 100)
            {
                SceneManager.Instance.LoadEnter();
            }
        }
    }

    void Load()
    {
        operation.allowSceneActivation = true;  //允许异步加载完毕后自动切换场景
    }

}
