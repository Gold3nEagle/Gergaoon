using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ??? Where is Documentation?
/// </summary>

public class Score : MonoBehaviour {

    public Text scoreText;
    public int ballValue;

    public GPlayServices playServ;

    public int score;

	// Use this for initialization
	void Start () {
        score = 0;
        UpdateScore();
	}


     void OnTriggerEnter2D()
    {
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

    public void PostScore()
    {
        playServ.AddScoreToLeaderboard(GPGSIds.leaderboard, score);
    }

}
