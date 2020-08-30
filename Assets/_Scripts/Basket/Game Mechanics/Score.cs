using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms; 


public class Score : MonoBehaviour {

    public Text scoreText, totalCandyDisplay;
    public int ballValue;  
    public int score;
    public AudioClip[] clips;

    int totalCandy;

    AudioSource audioSource;
    

	// Use this for initialization
	void Start () {
        totalCandyDisplay = GameObject.FindGameObjectWithTag("totalCoins").GetComponent<Text>();
        scoreText = GameObject.FindGameObjectWithTag("scoreNum").GetComponent<Text>();
        DisplayTotalCandy();
        audioSource = GetComponent<AudioSource>();
        score = 0;
        UpdateScore();
        
        
	} 

     void OnTriggerEnter2D()
    {
        audioSource.clip = clips[Random.Range(0, 2)];
        audioSource.Play();
        score += ballValue;
        UpdateScore();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bomb")
        {
            score -= ballValue * 3;
            UpdateScore();
        }
    } 

    void UpdateScore()
    {
        scoreText.text = score.ToString();
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt("Score", score);
        int TP = PlayerPrefs.GetInt("TP");

         
        CalculateTotalCandy();
    }

    public void PostScore()
    { 

    }

    void CalculateTotalCandy()
    {
        if(score < 0)
        {
            score = 0;
        }
        totalCandy += score;
        PlayerPrefs.SetInt("totalCandy", totalCandy); 
    }

    public void DisplayTotalCandy()
    {
        totalCandy = PlayerPrefs.GetInt("totalCandy");
        totalCandyDisplay.text = totalCandy.ToString();
    }

    public int GetTotalCandy()
    {
        totalCandy = PlayerPrefs.GetInt("totalCandy"); 
        return totalCandy;
    }

    public void SetTotalCandy(int total)
    {
        PlayerPrefs.SetInt("totalCandy", total);
    }

}
