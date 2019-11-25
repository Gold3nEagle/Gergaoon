using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkySlide : MonoBehaviour
{

    GameController gameController;
    float xAxis = 6.4f;

    // Start is called before the first frame update
    void Start()
    { 
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.playing)
        {
            this.transform.position = new Vector3(xAxis, transform.position.y, transform.position.z);
            xAxis -= 0.0005f;
        }
    }
}
