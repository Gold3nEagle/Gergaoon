using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxController : MonoBehaviour
{

    AudioSource audioSource;
    public AudioClip scoreClip;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = scoreClip;
    }

    public void PlayScoreSound()
    {
        audioSource.Play();
    } 


}
