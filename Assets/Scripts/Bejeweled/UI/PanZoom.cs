using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanZoom : MonoBehaviour
{
    Vector3 touchStart;
    public float zoomOutMin = 500;
    public float zoomOutMax = 1000;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if(Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * 0.01f);

        } else if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += direction;
        }

        if(Camera.main.transform.position.y >= 1500)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,  1500f, -10f);
        }

        if (Camera.main.transform.position.y <= 500)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, 500f, -10f);
        }

        if (Camera.main.transform.position.x <= 250)
        {
            Camera.main.transform.position = new Vector3(250f, Camera.main.transform.position.y, -10f);
        }

        if (Camera.main.transform.position.x >= 1000)
        {
            Camera.main.transform.position = new Vector3(1000f, Camera.main.transform.position.y, -10f);
        }

        Zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    void Zoom(float increment)
    {
        increment = increment * 500;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }

}
