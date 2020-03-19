using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ShowShop()
    {
        animator.SetBool("ShopShown", true);
    }

    public void HideShop()
    {
        animator.SetBool("ShopShown", false);
    }


}
