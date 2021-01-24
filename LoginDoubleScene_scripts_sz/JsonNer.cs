using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JsonNer : MonoBehaviour
{

    public Player_Properties playerproperties;
    public PlayerPropJson recievedplayerproperties;
    public Text output;

    //public void SendPlayerProperties(Player_Properties playerproperties)
    public void SendPlayerProperties()
    {
        string json = JsonUtility.ToJson(playerproperties);
        UserAccountManager.instance.SetData(json);
        Debug.Log(json);
        Debug.Log("SendingData");
    }

    public void GetPlayerProperties()
    {
        StartCoroutine(UserAccountManager.instance.GetData(CallBackAction));

    }
    void CallBackAction(string response)
    {
        Debug.Log(response);
        output.text = response;
        recievedplayerproperties = JsonUtility.FromJson<PlayerPropJson>(response);
        SetPlayerPropertiesfromRecieved();
    }

    void SetPlayerPropertiesfromRecieved()
    {
        playerproperties.level = recievedplayerproperties.level;
        //do same for all other properties
    }

}
