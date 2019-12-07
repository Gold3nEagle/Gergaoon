using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public Camera cam;
    public GameObject[] balls;
    public float timeLeft;
    public Text timerText;
    public Text endScoreText;
    public GameObject EndGameObj; 
    public GameObject DLight;
    public GameObject Lights;
    public HatController hatController;
    public int ballSpeed;
    public GameObject splashScreen;
    public GameObject startButton;
    public GameObject restartButton;
    public GameObject GameBeginsObj;
    public GameObject colliderObj;

    public Score gameScore;
    public AdsScript ads;
    public SfxController sfx;

    private Rigidbody2D rb;
    private new Renderer renderer;
    private float maxWidth;
    public static bool playing;

    private float firstWait, secondWait;
    private int designatedTime= 100;
    private float yrotation = 100;
    private float startWait;
    
    void Start()
    {
        firstWait = 1.0f;
        secondWait = 2.0f;
        startWait = 2.0f;

        if (cam == null)
        {
            cam = Camera.main;
        }

        playing = false;
        renderer = GetComponent<Renderer>();
        //rb = balls[0].GetComponent<Rigidbody2D>();   Responsible for sprite's gravity. 
        // rb.gravityScale = ballSpeed;                Setting the gravity modifier to the rigidbody component. 

        //Getting the width and height of the screen.
        Vector3 upperCorner = new Vector3(Screen.width, Screen.height, 0.0f);
        Vector3 targetWidth = cam.ScreenToWorldPoint(upperCorner);

        //ball.renderer.bounds.extents.x; I had to put it as 1.0f because the renderer property was duprecated. Needed to improvise so I got the width manually
        float ballWidth = 1.0f;
 

        maxWidth = targetWidth.x - ballWidth;    
        UpdateText();
         
        

    }

    public void StartGame()
    {
        SetTimesPlayed();
        hatController.ToggleControl(true); 
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

            if (DLight.transform.rotation.y > 0)
            {
                yrotation = DLight.transform.rotation.y;
                DLight.transform.Rotate(new Vector3(0, --yrotation, 0));
            }

        }
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(startWait);

        while (timeLeft > designatedTime) {
            GameObject ball = balls[Random.Range(0, balls.Length)];
        Vector3 spawnPosition = new Vector3(Random.Range(-maxWidth, maxWidth), transform.position.y, transform.position.z);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(ball, spawnPosition, spawnRotation);
            
             
          yield return new WaitForSeconds(Random.Range(firstWait, secondWait));
        }
        if (timeLeft == 0)
        {
            GameBeginsObj.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            EndGameObj.SetActive(true);
            StartCoroutine(ShowScore());



            //Save and Post Score to leaderboard
            gameScore.SaveScore();
            gameScore.PostScore();
            

        } else if (designatedTime > 0) {
            startWait = 0;     
        firstWait -= 0.15f;
        secondWait -= 0.3f;
        designatedTime -= 10;
            if (designatedTime < 0 || firstWait < 0)
            { designatedTime = 0;
                firstWait = 0.1f;
                secondWait = 0.2f;

            }
        StartCoroutine(Spawn());
        }
         
    }
     

    public void AdjustDLight()
    { 
        while(yrotation > 0)
        {
            yrotation-=2f;
        DLight.transform.Rotate( new Vector3(0,yrotation , 0)); 
        } 
    }


    void UpdateText()
    {
        timerText.text =   Mathf.RoundToInt(timeLeft).ToString(); 
    }


    IEnumerator ShowScore()
    {
        yield return new WaitForSeconds(4.0f);
        colliderObj.SetActive(true);
        hatController.ToggleControl(false);
        hatController.HatEndPosition();
        Quaternion spawnRotation = Quaternion.identity;
        Vector3 spawnPosition = new Vector3(4, 8);

        for (int i = 0; i <= gameScore.score; ++i)
        {
            endScoreText.text = i.ToString();
            sfx.PlayScoreSound();
            yield return new WaitForSeconds(0.1f);
            GameObject endballs = balls[Random.Range(0, 7)];
            Instantiate( endballs, spawnPosition, spawnRotation); 
        }
        yield return new WaitForSeconds(2.0f);
        //Show Ad
        ads.ShowInterstitialAd();
        yield return new WaitForSeconds(4.0f); 
        restartButton.SetActive(true);
    }

    void SetTimesPlayed()
    {
        int TP = PlayerPrefs.GetInt("TP");
        TP++;
        PlayerPrefs.SetInt("TP", TP);
    }


    public void RestartGame()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

}
