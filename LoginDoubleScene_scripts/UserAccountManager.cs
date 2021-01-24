using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using DatabaseControl;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UserAccountManager : MonoBehaviour
{
    public static UserAccountManager instance;
    public string LoginScene = "LoginScene";
    public UnityEvent OnDataRecieved;

    public static bool isLoggedIn = false;
    //public string DataString;  //You can store one string Data //passed in SetData fn itself

    void Awake()
    {   
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    //These store the username and password of the player when they have logged in
    public static string playerUsername { get; protected set; } = "";
    private string playerPassword = "";

    public void Logout()
    {
        playerUsername = "";
        playerPassword = "";
        isLoggedIn = false;
        SceneManager.LoadScene(LoginScene);
    }

    public void Login(string username, string password)
    {
        playerUsername = username;
        playerPassword = password;
        isLoggedIn = true;
    }

    void LoadData()
    {
       // string response; 
      //  StartCoroutine(GetData(response));
       // return response;
    }

    public IEnumerator GetData(Action<string> outcome)
    {
        IEnumerator e = DCF.GetUserData(playerUsername, playerPassword); // << Send request to get the player's data string. Provides the username and password
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Error")
        {
            Debug.Log("Error Fetching Data");
        }
        else
        {
            /*
            //The player's data was retrieved. Goes back to loggedIn UI and displays the retrieved data in the InputField
            loadingParent.gameObject.SetActive(false);
            loggedInParent.gameObject.SetActive(true);
            LoggedIn_DataOutputField.text = response;
            */

            if (OnDataRecieved != null) // Just a Unity Event
            {
                OnDataRecieved.Invoke();
            }

            outcome.Invoke(response);
        }
    }
    IEnumerator SetDataCourotine(string data)
    {
        IEnumerator e = DCF.SetUserData(playerUsername, playerPassword, data); // << Send request to set the player's data string. Provides the username, password and new data string
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Success")
        {
            Debug.Log("Data Synced To Player ID");
        }
        else
        {
            Debug.Log("Error Sending Data");
        }
    }

    //UI Button Pressed Methods
    public void SetData(string DataString)
    {
        StartCoroutine(SetDataCourotine(DataString));
    }

}

