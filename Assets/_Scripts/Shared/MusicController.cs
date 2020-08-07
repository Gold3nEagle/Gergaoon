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
    string currentScene;

    private void Awake()
    {

        GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        } 

        currentScene = SceneManager.GetActiveScene().name;
        if(currentScene == "SetLocale" || currentScene == "OverWorld")
        {
            DontDestroyOnLoad(gameObject);
            PlayMenuMusic();
        }

        if(currentScene == "GameScene" || currentScene == "Bejeweled")
        {
            GameObject mainMusicGO = GameObject.FindGameObjectWithTag("Music");
            Destroy(mainMusicGO);
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        sfxAudioSource = GetComponent<AudioSource>();  

        //Run on Hat menu and Bejeweled menu
        if (currentScene == "GameScene" || currentScene == "Bejeweled") { 
        PlayMenuMusic();
        }
    }
     

    public void StartGameMusic()
    {
        musicAudioSource.clip = clips[1];
        musicAudioSource.Play();
    } 

    public void PlayMenuMusic()
    { 
        musicAudioSource.clip = clips[0]; 
        musicAudioSource.Play();
        if (currentScene == "Bejeweled")
        {
            musicAudioSource.loop = true;
        }

    }

    //This one is for bejeweled, for hat game its in the hat controller
     public void PlayRandomDestroyNoise()
    {
        int clipToPlay = Random.Range(0, destroyNoise.Length);
        sfxAudioSource.clip = destroyNoise[clipToPlay];
        sfxAudioSource.Play();
    }

    public void PlayCheeringSound()
    {
        musicAudioSource.loop = false;
        musicAudioSource.clip = clips[2];
        musicAudioSource.Play();
    }

}
