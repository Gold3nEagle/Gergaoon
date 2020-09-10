using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignInSceneHandler : MonoBehaviour
{
    public TMP_InputField emailIF;
    public TMP_InputField nicknameIF;
    public TMP_InputField passwordIF;

    public GameObject chatPanel, loginPanel;

    private bool signInComplete;
    FixArabicTMProUGUI fixArabic;

    private void Start()
    {
        fixArabic = GetComponent<FixArabicTMProUGUI>();
    }


    public void SignUp() => APIHandler.Instance.authAPI.SignUpUser(emailIF.text, passwordIF.text,
        () =>
        {
            var newUser = new User {nickname = nicknameIF.text};
            APIHandler.Instance.databaseAPI.PostUser(newUser,
                () =>
                {
                    APIHandler.Instance.authAPI.SetUser(newUser);
                    signInComplete = true; 
                }, Debug.Log);
        }, Debug.Log);

    public void SignIn()
    {
        APIHandler.Instance.authAPI.SignInUser(emailIF.text, passwordIF.text,
            () => APIHandler.Instance.databaseAPI.GetUser(user =>
            {
                APIHandler.Instance.authAPI.SetUser(user);
                signInComplete = true; 
            }, Debug.Log),
            Debug.Log);
    }

    void CheckLogin()
    {
        if (signInComplete == true)
        {
            loginPanel.SetActive(false);
            chatPanel.SetActive(true); 
        }
    }

    private void Update()
    {
        if (!signInComplete) return;
        loginPanel.SetActive(false);
        chatPanel.SetActive(true);

    }
}