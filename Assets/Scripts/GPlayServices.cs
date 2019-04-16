using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GPlayServices : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        SignIn();
    }

    //Signing in the user upon starting the game.
    void SignIn()
    {
        Social.localUser.Authenticate(success => { });
    }

    public void AddScoreToLeaderboard(string leaderboardId, int score)
    {
        Social.ReportScore(score, leaderboardId, success => { });
    }
 
    public void ShowLeaderboardsUI()
    {
         Social.ShowLeaderboardUI();
         PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkItef62N0LEAIQAA");
    }


}
