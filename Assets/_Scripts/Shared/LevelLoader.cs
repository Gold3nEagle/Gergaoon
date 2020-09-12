using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{

    public GameObject loadingScreen;
    public Slider slider;
     

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
    }


    IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);

        CheckToDestroyMusic(sceneIndex);

        if(sceneIndex == 2)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        } else
        {
            Screen.orientation = ScreenOrientation.Portrait;
        } 

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 9.0f);

            slider.value = progress;

            yield return null;
        }

    }

    void CheckToDestroyMusic(int sceneIndex)
    {
        if(sceneIndex == 2 || sceneIndex == 3 || sceneIndex == 4)
        {
            GameObject MainmusicGO = GameObject.FindGameObjectWithTag("Music");
            Destroy(MainmusicGO);
        }
    }

}
