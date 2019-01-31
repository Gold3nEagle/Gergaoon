using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public Camera cam;
    public GameObject[] balls;
    public float timeLeft;
    public Text timerText;
    public GameObject gameOverText;
    public GameObject restartButton;
    public HatController hatController;
    public int ballSpeed;
    public GameObject splashScreen;
    public GameObject startButton;

    private Rigidbody2D rb;
    private new Renderer renderer;
    private float maxWidth;
    private bool playing;

    // Use this for initialization
    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        playing = false;
        renderer = GetComponent<Renderer>();
       // rb = ball.GetComponent<Rigidbody2D>();
       // rb.gravityScale = ballSpeed;

        //Getting the width and height of the screen.
        Vector3 upperCorner = new Vector3(Screen.width, Screen.height, 0.0f);
        Vector3 targetWidth = cam.ScreenToWorldPoint(upperCorner);

        //ball.renderer.bounds.extents.x; I had to put it as 1.0f because the renderer property was duprecated. Needed to improvise so I got the width manually
        float ballWidth = 1.0f;
        //GetComponent<SpriteRenderer>().bounds.size.x;
        //Debug.Log(ballWidth);

        maxWidth = targetWidth.x - ballWidth;    
        UpdateText();
    }

    public void StartGame()
    {
        hatController.ToggleControl(true);
        splashScreen.SetActive(false);
        startButton.SetActive(false);
        StartCoroutine(Spawn());
        playing = true;
    }


    void FixedUpdate()
    {

        if (playing)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft < 0)
            {
                timeLeft = 0;
            }
            UpdateText();
        }
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2.0f);

        while (timeLeft > 0) {
            GameObject ball = balls[Random.Range(0, balls.Length)];
        Vector3 spawnPosition = new Vector3(Random.Range(-maxWidth, maxWidth), transform.position.y, transform.position.z);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(ball, spawnPosition, spawnRotation);
            
          

            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        }
        yield return new WaitForSeconds(0.5f);
        gameOverText.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        restartButton.SetActive(true);

    }

    void UpdateText()
    {
        timerText.text = "Time Left: \n" + Mathf.RoundToInt(timeLeft);

    }

}
