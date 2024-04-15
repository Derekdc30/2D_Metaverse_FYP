using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    // 方法来加载场景，传入场景名
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 方法来异步加载场景，传入场景名
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncRoutine(sceneName));
    }

    // 协程处理异步场景加载
    private IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // 等待加载完成
        while (!asyncLoad.isDone)
        {
            // 这里可以输出进度 asyncLoad.progress 或显示加载动画
            yield return null;
        }
    }
}
