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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGameMusic()
    {
        audioSource.clip = clips[0];
        audioSource.Play();
    }
}
