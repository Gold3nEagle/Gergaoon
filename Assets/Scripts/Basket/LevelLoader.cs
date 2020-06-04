﻿using System.Collections;
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

        switch (sceneIndex)
        {
            case 0:
                Screen.orientation = ScreenOrientation.Portrait;
                break;

            case 1:
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                break;

            case 2:
                Screen.orientation = ScreenOrientation.Portrait;
                break;
        }

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 9.0f);

            slider.value = progress;

            yield return null;
        }

    }

}