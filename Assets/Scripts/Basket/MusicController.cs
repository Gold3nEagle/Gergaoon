using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{


    AudioSource sfxAudioSource;

    public AudioSource musicAudioSource;
    public AudioClip[] clips;
    public AudioClip[] destroyNoise;

    // Start is called before the first frame update
    void Start()
    {
        sfxAudioSource = GetComponent<AudioSource>(); 

        string currentScene = SceneManager.GetActiveScene().name;

        //Run on Hat menu and Bejeweled menu
        if (currentScene == "GameScene" || currentScene == "Bejeweled") { 
        PlayMenuMusic();
        }
    }
     

    public void StartGameMusic()
    {
        musicAudioSource.clip = clips[0];
        musicAudioSource.Play();
    } 

    public void PlayMenuMusic()
    { 
        musicAudioSource.clip = clips[1]; 
        musicAudioSource.Play();

    }

    //This one is for bejeweled, for hat game its in the hat controller
     public void PlayRandomDestroyNoise()
    {
        int clipToPlay = Random.Range(0, destroyNoise.Length);
        sfxAudioSource.clip = destroyNoise[clipToPlay];
        sfxAudioSource.Play();
    }

}
