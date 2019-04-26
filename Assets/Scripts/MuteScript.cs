using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteScript : MonoBehaviour
{

    private bool toggle = false;
    public Sprite sound, mute;
    public Image buttonImage;

    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
    }

    public void MuteAll()
    {

        if (toggle == false)
        {
            AudioListener.volume = 0f;
            buttonImage.sprite = mute;
            toggle = true;
        }
        else
        {
            AudioListener.volume = 1f;
            buttonImage.sprite = sound;
            toggle = false;
        }
    }


    public void PPAccess()
    {
        Application.OpenURL("https://haethamalhaddad.blogspot.com/2019/04/blog-post.html");
    }


    public void InstAcess()
    {
        Application.OpenURL("https://www.instagram.com/gold3neagle/");
    }

    public void TwitAcess()
    {
        Application.OpenURL("https://twitter.com/eaglegamer47");
    }
     

}
