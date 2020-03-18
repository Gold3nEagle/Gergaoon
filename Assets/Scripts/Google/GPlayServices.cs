using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;
/// <summary>
/// ??? Where is Documentation?
/// </summary>
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
         PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkItef62N0LEAIQAw");
        
    }

    public void ShowAchievemntsUI()
    {
        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }
       
    public void UnlockAchievement(int achievement)
    {
        Debug.Log(achievement);
        Debug.Log("Unlocking achievements works!!");
        switch (achievement)
        {
            case 1:
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_50_candies, 100f, success => { });
                break;

            case 2:
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_100_candies, 100f, success => { });
                break;

            case 3:
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_professional , 100f, success => { });
                break;

            case 4:
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_survival_of_the_fittest, 100f, success => { });
                break;

            case 5:
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_you_did_your_best, 100f, success => { });
                break;
        }


    }
}
