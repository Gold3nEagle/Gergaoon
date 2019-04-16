using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour {
     
    public void RestartGame()
    {
        //Application.LoadLevel(Application.loadedLevel); Depricated
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single); 
    }
 
   
}
