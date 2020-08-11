using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DentedPixel;
using UnityEngine.UI;

public class DoTweenController : MonoBehaviour
{

    public LeanTweenType inType;
    public LeanTweenType outType; 
    public float timeDelay;

    // Start is called before the first frame update
    void Start()
    {

        if (gameObject.CompareTag("WiggleButton"))
        {
            Wiggle();
        } 
    }

    public void OnEnable()
    { 

        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), timeDelay).setEase(inType);
    }

    public void OnComplete()
    {
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), timeDelay).setEase(outType).setOnComplete(DisableGameObject);
    }

    void DisableGameObject()
    {
       gameObject.SetActive(false);
    }

    public void Wiggle()
    {
        LeanTween.scale(gameObject, new Vector3(0.95f, 1.1f, 0), 0.4f).setLoopPingPong();
    }

}
