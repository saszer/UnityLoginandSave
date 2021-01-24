using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DatabaseControl;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class Login : MonoBehaviour
{
    public bool loadsceneonlogin = true;
    public string sceneToLoadOnLogin = "Game";
    public string LoginScene = "LoginScene";

    void Awake()
    {
        ResetAllUIElements();
        UserAccountManager.instance.LoginScene = LoginScene;
        UserAccountManager.instance.GameScene = sceneToLoadOnLogin;
    }


    string databaseName = "";
    bool canRunSequences = false;

    void Start()
    {
        //Gets the databaseName as it was setup through the editor
        GameObject linkObj = GameObject.Find("Link");
        if (linkObj == null)
        {
            Debug.LogError("DCP Error: Cannot find the link object in the scene so scripts running Command Sequences don't know the database name");
        }
        else
        {
            DCP_Demos_LinkDatabaseName linkScript = linkObj.gameObject.GetComponent<DCP_Demos_LinkDatabaseName>() as DCP_Demos_LinkDatabaseName;
            if (linkScript == null)
            {
                Debug.LogError("DCP Error: Cannot find the link script on link object so scripts running Command Sequences don't know the database name");
            }
            else
            {
                if (linkScript.databaseName == "")
                {
                    Debug.LogError("DCP Error: This demo scene has not been setup. Please setup the demo scene in the Setup window before use. Widnow>Database Control Pro>Setup Window");
                }
                else
                {
                    databaseName = linkScript.databaseName;
                    canRunSequences = true;

                    UserAccountManager.instance.databaseName = databaseName;
                }
            }
        }
    }

    //All public variables bellow are assigned in the Inspector

    //These are the GameObjects which are parents of groups of UI elements. The objects are enabled and disabled to show and hide the UI elements.
    public GameObject loginParent;
    public GameObject registerParent;
    public GameObject loggedInParent;
    public GameObject loadingParent;

    //These are all the InputFields which we need in order to get the entered usernames, passwords, etc
    public InputField Login_UsernameField;
    public InputField Login_PasswordField;
    public InputField Register_UsernameField;
    public InputField Register_PasswordField;
    public InputField Register_ConfirmPasswordField;

    //These are the UI Texts which display errors
    public Text Login_ErrorText;
    public Text Register_ErrorText;

    //This UI Text displays the username once logged in. It shows it in the form "Logged In As: " + username
    public Text LoggedIn_DisplayUsernameText;

    //These store the username and password of the player when they have logged in
    public static string playerUsername { get; protected set; } = "";
    private string playerPassword = "";

    //Called by Button Pressed Methods to Reset UI Fields
    void ResetAllUIElements()
    {
        //This resets all of the UI elements. It clears all the strings in the input fields and any errors being displayed
        Login_UsernameField.text = "";
        Login_PasswordField.text = "";
        Register_UsernameField.text = "";
        Register_PasswordField.text = "";
        Register_ConfirmPasswordField.text = "";
        Login_ErrorText.text = "";
        Register_ErrorText.text = "";
        LoggedIn_DisplayUsernameText.text = "";
    }

    //Called by Button Pressed Methods. These use DatabaseControl namespace to communicate with server.
    IEnumerator LoginUser()
    {

        //Run the command sequence called 'Login' on the database name which has been retrieved in the start method. Sends the username and password (from the ui input fields) as parameters
        IEnumerator e = DCP.RunCS(databaseName, "Login", new string[2] { playerUsername, playerPassword });
       // IEnumerator e = DCP.Login(playerUsername, playerPassword); // << Send request to login, providing username and password
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Success")
        {
            //Username and Password were correct. Stop showing 'Loading...' and show the LoggedIn UI. And set the text to display the username.
            ResetAllUIElements();
            loadingParent.gameObject.SetActive(false);
            loggedInParent.gameObject.SetActive(true);
            LoggedIn_DisplayUsernameText.text = "Logged In As: " + playerUsername;
            if (loadsceneonlogin)
            {
                UserAccountManager.instance.Login(playerUsername, playerPassword);
                SceneManager.LoadScene(sceneToLoadOnLogin);
            }
        }
        else
        {
            //Something went wrong logging in. Stop showing 'Loading...' and go back to LoginUI
            loadingParent.gameObject.SetActive(false);
            loginParent.gameObject.SetActive(true);
            if (response == "UserError")
            {
                //The Username was wrong so display relevent error message
                Login_ErrorText.text = "Error: Username not Found";
            }
            else
            {
                if (response == "PassError")
                {
                    //The Password was wrong so display relevent error message
                    Login_ErrorText.text = "Error: Password Incorrect";
                }
                else
                {
                    //There was another error. This error message should never appear, but is here just in case.
                    Login_ErrorText.text = "Error: Unknown Error. Please try again later.";
                }
            }
        }
    }

    //public Player_Properties playerPropJson = new Player_Properties();
    public PlayerPropJson playerPropJson;
    IEnumerator RegisterUser()
    {
        //Initialise data
        string data;
        playerPropJson.username = playerUsername;
        playerPropJson.carbonfootprint = 0;
        data = JsonUtility.ToJson(playerPropJson);

        // Run the command sequence called 'Register' on the database name which has been retrieved in the start method.Sends the username and password(from the ui input fields) as parameters
        IEnumerator e = DCP.RunCS(databaseName, "Register", new string[3] { playerUsername, playerPassword, data});
        //IEnumerator e = DCF.RegisterUser(playerUsername, playerPassword, "Hello World"); // << Send request to register a new user, providing submitted username and password. It also provides an initial value for the data string on the account, which is "Hello World".
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Success")
        {
            //Username and Password were valid. Account has been created. Stop showing 'Loading...' and show the loggedIn UI and set text to display the username.
            ResetAllUIElements();
            loadingParent.gameObject.SetActive(false);
            loggedInParent.gameObject.SetActive(true);
            LoggedIn_DisplayUsernameText.text = "Logged In As: " + playerUsername;

            if (loadsceneonlogin)
            {
                UserAccountManager.instance.Login(playerUsername, playerPassword);
                SceneManager.LoadScene(sceneToLoadOnLogin);
            }
        }
        else
        {
            //Something went wrong logging in. Stop showing 'Loading...' and go back to RegisterUI
            loadingParent.gameObject.SetActive(false);
            registerParent.gameObject.SetActive(true);
            if (response == "username in use")
            {
                //The username has already been taken. Player needs to choose another. Shows error message.
                Register_ErrorText.text = "Error: Username Already Taken";
            }
            else
            {
                //There was another error. This error message should never appear, but is here just in case.
                Login_ErrorText.text = "Error: Unknown Error. Please try again later.";
            }
        }
    }
   
   
    //UI Button Pressed Methods
    public void Login_LoginButtonPressed()
    {
        //Called when player presses button to Login

        //Get the username and password the player entered
        playerUsername = Login_UsernameField.text;
        playerPassword = Login_PasswordField.text;

        //Check the lengths of the username and password. (If they are wrong, we might as well show an error now instead of waiting for the request to the server)
        if (playerUsername.Length > 3)
        {
            if (playerPassword.Length > 5)
            {
                //Username and password seem reasonable. Change UI to 'Loading...'. Start the Coroutine which tries to log the player in.
                loginParent.gameObject.SetActive(false);
                loadingParent.gameObject.SetActive(true);
                StartCoroutine(LoginUser());
            }
            else
            {
                //Password too short so it must be wrong
                Login_ErrorText.text = "Error: Password Incorrect";
            }
        }
        else
        {
            //Username too short so it must be wrong
            Login_ErrorText.text = "Error: Username Incorrect";
        }
    }
    public void Login_RegisterButtonPressed()
    {
        //Called when the player hits register on the Login UI, so switches to the Register UI
        ResetAllUIElements();
        loginParent.gameObject.SetActive(false);
        registerParent.gameObject.SetActive(true);
    }
    public void Register_RegisterButtonPressed()
    {
        //Called when the player presses the button to register

        //Get the username and password and repeated password the player entered
        playerUsername = Register_UsernameField.text;
        playerPassword = Register_PasswordField.text;
        string confirmedPassword = Register_ConfirmPasswordField.text;

        //Make sure username and password are long enough
        if (playerUsername.Length > 3)
        {
            if (playerPassword.Length > 4)
            {
                //Check the two passwords entered match
                if (playerPassword == confirmedPassword)
                {
                    //Username and passwords seem reasonable. Switch to 'Loading...' and start the coroutine to try and register an account on the server
                    registerParent.gameObject.SetActive(false);
                    loadingParent.gameObject.SetActive(true);

                    StartCoroutine(RegisterUser());
                }
                else
                {
                    //Passwords don't match, show error
                    Register_ErrorText.text = "Error: Password's don't Match";
                }
            }
            else
            {
                //Password too short so show error
                Register_ErrorText.text = "Error: Password too Short";
            }
        }
        else
        {
            //Username too short so show error
            Register_ErrorText.text = "Error: Username too Short";
        }
    }
    public void Register_BackButtonPressed()
    {
        //Called when the player presses the 'Back' button on the register UI. Switches back to the Login UI
        ResetAllUIElements();
        loginParent.gameObject.SetActive(true);
        registerParent.gameObject.SetActive(false);
    }


    public void LoggedIn_LogoutButtonPressed()
    {
        //Called when the player hits the 'Logout' button. Switches back to Login UI and forgets the player's username and password.
        //Note: Database Control doesn't use sessions, so no request to the server is needed here to end a session.
        ResetAllUIElements();
        playerUsername = "";
        playerPassword = "";
        loginParent.gameObject.SetActive(true);
        loggedInParent.gameObject.SetActive(false);
    }
}


