using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Login : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button goToRegisterButton;
    public static Login instance;
    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    ArrayList credentials;

    void Start()
    {
        loginButton.onClick.AddListener(login);
        goToRegisterButton.onClick.AddListener(moveToRegister);
        if (File.Exists(Application.dataPath + "/credentials.txt"))
        {
            credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.txt"));
        }
        else
        {
            Debug.Log("Credential file doesn't exist");
        }
    }

    void login()
    {
        bool isExists = false;

        credentials = new ArrayList(File.ReadAllLines(Application.dataPath + "/credentials.txt"));

        foreach (var i in credentials)
        {
            string line = i.ToString();
            if(i.ToString().Substring(0,i.ToString().IndexOf(":")).Equals(usernameInput.text) &&
               i.ToString().Substring(i.ToString().IndexOf(":")+1).Equals(passwordInput.text))
            {
                isExists = true;
                break;
            }
        }

        if (isExists)
        {
            Debug.Log($"Logging in '{usernameInput.text}'");
            loadMainMenu();
        }
        else
        {
            Debug.Log("Incorrect credentials.");
        }
    }
    void moveToRegister()
    {
        SceneManager.LoadScene("Register");
    }
    void loadMainMenu()
    {
        Client.instance.ConnectToServer();
        SceneManager.LoadScene("MainMenu");
    }

}
