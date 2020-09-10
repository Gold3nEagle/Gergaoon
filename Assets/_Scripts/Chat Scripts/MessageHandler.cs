using TMPro;
using UnityEngine;

public class MessageHandler : MonoBehaviour
{
    public TextMeshProUGUI text, sender;
    public GameObject editButton;
    public GameObject deleteButton;
    FixArabicTMProUGUI fixArabic;

    public Message message;
    public string messageId;
    public bool isOwner;

    private void Start()
    {

        fixArabic = GetComponentInChildren<FixArabicTMProUGUI>(); 

        sender.text = $"{message.senderNickname}:";

        text.text = fixArabic.ReturnFixedString(message.text);

        if (!isOwner) return;
        editButton.SetActive(true);
        deleteButton.SetActive(true);
    }

    public void EditMessage() =>
        APIHandler.Instance.databaseAPI.EditMessage(messageId, MainSceneHandler.Instance.textIF.text,
            () => Debug.Log("Message edited!"), Debug.Log);

    public void DeleteMessage() =>
        APIHandler.Instance.databaseAPI.DeleteMessage(messageId, () => Debug.Log("Message deleted!"), Debug.Log);
}