using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class APIHandler : MonoBehaviour
{
    public static APIHandler Instance;
    public DatabaseAPI databaseAPI;
    public AuthAPI authAPI;
    public GameObject loginPanel;
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        databaseAPI = GetComponent<DatabaseAPI>();
        authAPI = GetComponent<AuthAPI>();
        loginPanel.SetActive(true);
    }
}
