using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseControl;
using System;

public class FetchAllDatabase : MonoBehaviour
{
    public string databaseName = "VF";

    public int numbersofrowsinLastCheck = 0;
    public int currentrows;
    public int iterating;
    public PlayerPropJson[] playerpropjsonarray;
    int playernumber;
    void Start()
    {
        StartCoroutine(GetNumOfRows());
    }

    public IEnumerator GetNumOfRows()
    {
        // Run the command sequence called 'Get Data' on the database name which has been retrieved in the start method.Sends the username and password(from the ui input fields) as parameters
        IEnumerator e = DCP.RunCS(databaseName, "GetNumberOfRows");
        //IEnumerator e = DCF.GetUserData(playerUsername, playerPassword); // << Send request to get the player's data string. Provides the username and password
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        var response = e.Current; 
        currentrows = int.Parse(response.ToString());
        Debug.Log(currentrows);
        iterating = currentrows;
        numbersofrowsinLastCheck = currentrows;

        if(numbersofrowsinLastCheck != currentrows)
        Run();
    }

    void Run()
    {
        if (iterating == 0)
            return;
        playernumber = iterating;
        StartCoroutine(Iterate(CallBackAction));

    }

    public IEnumerator Iterate(Action<string> outcome)
    {
        // Run the command sequence called 'Get Data' on the database name which has been retrieved in the start method.Sends the username and password(from the ui input fields) as parameters
        IEnumerator e = DCP.RunCS(databaseName, "Iterate", new string[1] { iterating.ToString() });
        //IEnumerator e = DCF.GetUserData(playerUsername, playerPassword); // << Send request to get the player's data string. Provides the username and password
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        var response = e.Current as string; // << The returned string from the request
        Debug.Log(response);

        iterating--;
        outcome.Invoke(response);
        Run();

    }

    void CallBackAction(string response)
    {
        playerpropjsonarray[playernumber] = JsonUtility.FromJson<PlayerPropJson>(response);
    }

}


