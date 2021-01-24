using UnityEngine;
using UnityEngine.UI;

public class LoginSceneLink : MonoBehaviour
{
    public Text usernameText;
    void Start()
    {
        if(UserAccountManager.isLoggedIn)
           usernameText.text = UserAccountManager.playerUsername;
    }

    public void Logout()
    {
        if (UserAccountManager.isLoggedIn)
            UserAccountManager.instance.Logout();
    }
}
