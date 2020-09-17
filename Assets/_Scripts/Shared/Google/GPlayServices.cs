using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;


public class GPlayServices : MonoBehaviour
{

    public bool loginSuccessful;

    //Enable for iOS
    //string leaderboardID = "leaderboardGen1";

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
#if UNITY_ANDROID
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();
            SignIn();

#elif UNITY_IPHONE
            AuthenticateUser(); 
#endif

        }
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


    public void ShowGergaoonLeaderboard()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkItef62N0LEAIQDA");
        Social.ShowLeaderboardUI();
    }

    public void ShowBasketLeaderboard()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkItef62N0LEAIQCw");
        Social.ShowLeaderboardUI();
    }

    public void ShowAchievemntsUI()
    {
        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }

    public void UnlockAchievement(int achievement)
    {
        //Debug.Log(achievement);
        //Debug.Log("Unlocking achievements works!!");
        switch (achievement)
        {
            case 1:
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_50_candies, 100f, success => { });
                break;

            case 2:
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_100_candies, 100f, success => { });
                break;

            case 3:
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_professional, 100f, success => { });
                break;

            case 4:
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_survival_of_the_fittest, 100f, success => { });
                break;

            case 5:
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_you_did_your_best, 100f, success => { });
                break;

            case 6:
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_finished_10_levels, 100f, success => { });
                break;

            case 7:
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_finished_100_levels, 100f, success => { });
                break;

            case 8:
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_its_over_4000, 100f, success => { });
                break;

        }
    }

    // BELOW IS FOR iOS

    //void AuthenticateUser()
    //{
    //    Social.localUser.Authenticate((bool success) =>
    //    {
    //        if (success)
    //        {
    //            loginSuccessful = true;
    //            Debug.Log("success");
    //        }
    //        else
    //        {
    //            Debug.Log("unsuccessful");
    //        }
    //        // handle success or failure
    //    });
    //}


    //public void PostScoreOnLeaderBoard(int myScore)

    //{
    //    if (loginSuccessful)
    //    {
    //        Social.ReportScore(myScore, leaderboardID, (bool success) =>
    //        {
    //            if (success)
    //                Debug.Log("Successfully Uploaded");
    //            // handle success or failure
    //        });
    //    }
    //    else
    //    {
    //        Social.localUser.Authenticate((bool success) =>
    //        {
    //            if (success)
    //            {
    //                loginSuccessful = true;
    //                Social.ReportScore(myScore, leaderboardID, (bool successful) =>
    //                {
    //                    // handle success or failure
    //                });
    //            }
    //            else
    //            {
    //                Debug.Log("unsuccessful");
    //            }
    //            // handle success or failure
    //        });
    //    }
    //}

}