using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVScript : MonoBehaviour
{

    Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    } 
     
    public void TVShow(int show)
    {
        switch (show)
        {
            case 0:
                Application.OpenURL("https://youtu.be/rR0dJmWFys4");
                break;

            case 1:
                Application.OpenURL("https://youtu.be/EJJFVCerdAU");
                break;

            case 2:
                Application.OpenURL("https://youtu.be/hYc-jw8dcFY");
                break;

            case 3:
                Application.OpenURL("https://youtu.be/q_1gnIvKDcY");
                break; 

        }
    }

    public void ShowTV()
    {
        anim.SetBool("showTV", true);
    }

    public void HideTV()
    {
        anim.SetBool("showTV", false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
