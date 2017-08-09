using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstSceneLoader : MonoBehaviour
{
    public RectTransform LoadingBar;

    public void BeginClick()
    {
        LoadingBar.gameObject.SetActive(true);
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
	{
	    AsyncOperation loadingOperation = SceneManager.LoadSceneAsync("Main");
        while (!loadingOperation.isDone)
        {
            SetProgres(loadingOperation.progress);
            yield return null;
        }
    }

    void SetProgres(float value)
    {
        if (value > 0)
        {
            LoadingBar.offsetMax = new Vector2(Screen.width / value, LoadingBar.offsetMax.y);
        }
    }
}
