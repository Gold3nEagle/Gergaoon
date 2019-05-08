using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ??? Where is Documentation?
/// </summary>
public class HatController : MonoBehaviour {

    public Camera cam;

    private float maxWidth;
    private Rigidbody2D rb;
    private Renderer renderer;
    private bool canControl;
    

    // Use this for initialization
    void Start () {
		if (cam == null)
        {
            cam = Camera.main;
        }

        renderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody2D>();

        //Getting the width and height of the screen.
        Vector3 upperCorner = new Vector3(Screen.width, Screen.height, 0.0f);
        Vector3 targetWidth = cam.ScreenToWorldPoint(upperCorner);
        float hatWidth = renderer.bounds.extents.x;
        maxWidth = targetWidth.x - hatWidth;

        canControl = false;

    }

    // Update is called once per physics timestamp
    void FixedUpdate () {

        //Will make the Hat controllable only when canControl is true.
        if (canControl)
        {

            Vector3 rawPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 targetPosition = new Vector3(rawPosition.x, -0.5f, 0.0f);
            float targetWidth = Mathf.Clamp(targetPosition.x, -maxWidth, maxWidth);
            targetPosition = new Vector3(targetWidth, targetPosition.y, targetPosition.z);
            rb.MovePosition(targetPosition);
        }
	}

    public void ToggleControl(bool toggle)
    {
        canControl = toggle;
    }

    public void HatEndPosition()
    {
        rb.MovePosition(new Vector2(4, 0));
    }

}
