﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupManager : MonoBehaviour
{
    // Start is called before the first frame update
    private IEnumerator Start()
    {

        while (!LocaleManager.instance.GetIsReady()) { 

        yield return null;
        }

        SceneManager.LoadScene("MainMenu");

    }

}
  
