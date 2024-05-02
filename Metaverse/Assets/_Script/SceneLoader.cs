using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public Slider progressBar; // 进度条引用
    public TMP_Text loadingText; // 加载文本引用
    public Canvas loadingCanvas; // 加载画面的Canvas引用

    void Start()
    {
        // 初始化时隐藏加载界面
        loadingCanvas.gameObject.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneAsync(string sceneName)
    {
        // 显示加载画面
        loadingCanvas.gameObject.SetActive(true);
        StartCoroutine(LoadSceneAsyncRoutine(sceneName));
    }

    private IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            progressBar.value = asyncLoad.progress;
            loadingText.text = "Loading... " + (asyncLoad.progress * 100) + "%";

            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // 加载完成后隐藏加载界面
        loadingCanvas.gameObject.SetActive(false);
    }
}
