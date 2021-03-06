﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneHandler : MonoBehaviour
{
    public static MainSceneHandler Instance;

    public TMP_InputField textIF;

    public GameObject messagePrefab, adPanel;
    public Transform messagesContainer;

    public Text totalCandyDisplay;
    int totalCandy;

    public Dictionary<string, MessageHandler> messages = new Dictionary<string, MessageHandler>();

    private void Awake() => Instance = this;

    private void Start()
    {
        APIHandler.Instance.databaseAPI.ListenForNewMessages(CreateMessage, Debug.Log);
        APIHandler.Instance.databaseAPI.ListenForEditedMessages(EditMessage, Debug.Log);
        APIHandler.Instance.databaseAPI.ListenForDeletedMessages(DeleteMessage, Debug.Log);

        DisplayTotalCandy();
        

    }

    public void SendMessage()
    { 
        if(GetTotalCandy() > 0)
        { 
        APIHandler.Instance.databaseAPI.PostMessage(
            new Message(APIHandler.Instance.authAPI.GetUser().nickname, APIHandler.Instance.authAPI.GetUserId(),
                textIF.text),
            () => Debug.Log("Message was sent!"), Debug.Log);

        textIF.text = " ";
            SpendCandy();
        } else
        {
            adPanel.SetActive(true);
        }
    }

    private void CreateMessage(Message message, string messageId)
    {
        var newMessage = Instantiate(messagePrefab, transform.position, Quaternion.identity);
        newMessage.transform.SetParent(messagesContainer, false);

        var newMessageHandler = newMessage.GetComponent<MessageHandler>();
        newMessageHandler.message = message;
        newMessageHandler.messageId = messageId;
        newMessageHandler.isOwner = message.senderUserId == APIHandler.Instance.authAPI.GetUserId();

        messages.Add(messageId, newMessageHandler);
         
    }

    private void Update()
    {
        if (messagesContainer.childCount > 18)
        {
            Transform[] allChildren = messagesContainer.GetComponentsInChildren<Transform>();

            for (int i = 1; i < messagesContainer.childCount; i++)
            {
                Destroy(allChildren[i].gameObject);
                Debug.Log("Destroyed Message");
            }

        }
    }

    private void EditMessage(string messageId, string newText) => messages[messageId].text.text = $"{messages[messageId].message.senderNickname}: {newText}";

    private void DeleteMessage(string messageId)
    {
        Destroy(messages[messageId].gameObject);
        messages.Remove(messageId);
    }

    public void SignOut()
    {
        APIHandler.Instance.databaseAPI.StopListeningForMessages();
        APIHandler.Instance.authAPI.SignOut();
        SceneManager.LoadScene("OverWorld");
    }


    void MessageCandyHandler()
    {

    }

    void SpendCandy()
    {
        totalCandy = PlayerPrefs.GetInt("totalCandy");
        totalCandy -= 5;
        PlayerPrefs.SetInt("totalCandy", totalCandy);
        DisplayTotalCandy();
    }

    public void DisplayTotalCandy()
    {
        totalCandy = PlayerPrefs.GetInt("totalCandy");
        totalCandyDisplay.text = totalCandy.ToString();
    }

    public int GetTotalCandy()
    {
        int totalCandy = PlayerPrefs.GetInt("totalCandy");

        return totalCandy;
    }

}