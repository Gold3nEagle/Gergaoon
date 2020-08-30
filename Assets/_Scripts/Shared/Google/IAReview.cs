using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Play.Review;

public class IAReview : MonoBehaviour
{

    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;

    // Start is called before the first frame update
    void Start()
    {
        int timesPlayed = PlayerPrefs.GetInt("firstMatch3");
        int beenRated = PlayerPrefs.GetInt("BeenRated");
        timesPlayed++;
        PlayerPrefs.SetInt("firstMatch3", timesPlayed);

        if (timesPlayed > 5 && beenRated == 0) ;
        {
            StartCoroutine(ReviewGame());  
        } 
    }


    IEnumerator ReviewGame()
    {
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();


        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
        PlayerPrefs.SetInt("BeenRated", 1);
    }
     
}
