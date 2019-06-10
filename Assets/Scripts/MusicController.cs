using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{


    AudioSource audioSource; 
    public AudioClip[] clips;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayRandomMusic();
    }
     

    public void StartGameMusic()
    {
        audioSource.clip = clips[0];
        audioSource.Play();
    }
     

    public void ContinueRunningMusic()
    {
        audioSource.Play();
    }

    public void PlayRandomMusic()
    {
        int num = Random.Range(1, 5);
        audioSource.clip = clips[num];
        audioSource.Play();

    }

     

}
