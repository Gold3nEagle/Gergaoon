using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanZoom : MonoBehaviour
{
    Vector3 touchStart;
    public float zoomOutMin = 500;
    public float zoomOutMax = 1000;
    public float minX, maxX, minY, maxY;
    public float zoomSpeed = 500;
 

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

        if(Camera.main.transform.position.y >= maxY)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,  maxY, -10f);
        }

        if (Camera.main.transform.position.y <= minY)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, minY, -10f);
        }

        if (Camera.main.transform.position.x <= minX)
        {
            Camera.main.transform.position = new Vector3(minX, Camera.main.transform.position.y, -10f);
        }

        if (Camera.main.transform.position.x >= maxX)
        {
            Camera.main.transform.position = new Vector3(maxX, Camera.main.transform.position.y, -10f);
        }

        Zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    void Zoom(float increment)
    {
        increment = increment * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }

}
