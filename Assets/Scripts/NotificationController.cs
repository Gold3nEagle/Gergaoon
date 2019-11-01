using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase;
using System;
using System.Threading.Tasks;

public class NotificationController : MonoBehaviour
{


    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    protected bool isFirebaseInitialized = false;
    private string topic = "TestTopic";


    // Log the result of the specified task, returning true if the task
    // completed successfully, false otherwise.
    protected bool LogTaskCompletion(Task task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            Debug.Log(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            Debug.Log(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string errorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    errorCode = String.Format("Error.{0}: ",
                      ((Firebase.Messaging.Error)firebaseEx.ErrorCode).ToString());
                }
                Debug.Log(errorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            Debug.Log(operation + " completed");
            complete = true;
        }
        return complete;
    }


    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.
    protected virtual void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    // Setup message event handlers.
    void InitializeFirebase()
    {
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.SubscribeAsync(topic).ContinueWithOnMainThread(task => {
            LogTaskCompletion(task, "SubscribeAsync");
        });
        Debug.Log("Firebase Messaging Initialized");

        // This will display the prompt to request permission to receive
        // notifications if the prompt has not already been displayed before. (If
        // the user already responded to the prompt, thier decision is cached by
        // the OS and can be changed in the OS settings).
        Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(
          task => {
              LogTaskCompletion(task, "RequestPermissionAsync");
          }
        );
        isFirebaseInitialized = true;
    }

    public virtual void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message");
        var notification = e.Message.Notification;
        if (notification != null)
        {
            Debug.Log("title: " + notification.Title);
            Debug.Log("body: " + notification.Body);
            var android = notification.Android;
            if (android != null)
            {
                Debug.Log("android channel_id: " + android.ChannelId);
            }
        }
        if (e.Message.From.Length > 0)
            Debug.Log("from: " + e.Message.From);
        if (e.Message.Link != null)
        {
            Debug.Log("link: " + e.Message.Link.ToString());
        }
        if (e.Message.Data.Count > 0)
        {
            Debug.Log("data:");
            foreach (System.Collections.Generic.KeyValuePair<string, string> iter in
                     e.Message.Data)
            {
                Debug.Log("  " + iter.Key + ": " + iter.Value);
            }
        }
    }

    public virtual void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    public void ToggleTokenOnInit()
    {
        bool newValue = !Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled;
        Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = newValue;
        Debug.Log("Set TokenRegistrationOnInitEnabled to " + newValue);
    }

    // Exit if escape (or back, on mobile) is pressed.
    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // End our messaging session when the program exits.
    public void OnDestroy()
    {
        Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
    }



}
