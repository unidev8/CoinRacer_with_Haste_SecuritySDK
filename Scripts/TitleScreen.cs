using System;
using UnityEngine;
using System.Globalization;
using Mirror;
using Mirror.SimpleWeb;


public enum Role
{
    Server,
    Client
}

public class TitleScreen : MonoBehaviour
{
    public Role Role;

    private bool clientStarted = false;

    void Start()
    {
        // Based on whether or not the application is the client or server
        // show the correct UI elements. Server has no user interaction capabilities
        GameObject UIManangerObj = GameObject.Find("UIManager");        

        //clientUI = GameObject.Find("ClientUI");
        //serverUI = GameObject.Find("ServerUI");

        if (Role == Role.Server || Application.platform == RuntimePlatform.LinuxPlayer)
        {
            UIManangerObj.GetComponent<UIMananger>().clientUI.SetActive(false);            
            UIManangerObj.GetComponent<UIMananger>().FinalScoreScreen.SetActive(false);
            UIManangerObj.GetComponent<UIMananger>().LeaderboardSelect.SetActive(false);            
            UIManangerObj.GetComponent<UIMananger>().InGameUI.SetActive(false);
            UIManangerObj.GetComponent<UIMananger>().CarSelection.SetActive(false);
            UIManangerObj.GetComponent<UIMananger>().serverUI.SetActive(true);

            //UIMananger.Instance.serverUI.SetActive(true);
            //UIMananger.Instance.clientUI.SetActive(false);            

            NetworkManager.singleton.StartServer();
        }
        else
        {
            //UIMananger.Instance.serverUI.SetActive(false);
            //UIMananger.Instance.clientUI.SetActive(false);
            //UIMananger.Instance.InGameUI.SetActive(false);

            UIManangerObj.GetComponent<UIMananger>().clientUI.SetActive(false);
            UIManangerObj.GetComponent<UIMananger>().serverUI.SetActive(false);
            UIManangerObj.GetComponent<UIMananger>().InGameUI.SetActive(false);
            UIManangerObj.GetComponent<UIMananger>().FinalScoreScreen.SetActive(false);
            UIManangerObj.GetComponent<UIMananger>().LeaderboardSelect.SetActive(false);
            UIManangerObj.GetComponent<UIMananger>().CarSelection.SetActive(false);
            Login();
        }
    }

    public void Login()
    {
        //Debug.Log("TitleScreen.Login is started!");        
        MobileDetection.GetTokenFromBrowser();
#if UNITY_EDITOR
        StartNonBrowserLoginFlow();
#elif UNITY_WEBGL

    // call out to the browser window and pull the token/expiration date from hasteClient
    // then set the player prefs for quicker access
    // or just do it every time?
    var token = MobileDetection.GetTokenFromBrowser();
    PlayerPrefs.SetString("HasteAccessToken", token);

    Debug.Log("TitleScreen.Login: token= "+ token);

    StartClient();
    //SelectPlayer();
#else
  StartNonBrowserLoginFlow();
#endif
    }

    private void StartNonBrowserLoginFlow()
    {
        // The access token should be stored somewhere.
        // then in this function you should check that storage location to see if the user is already logged in
        // it does have an expiration date embedded in teh access_token. This should be parsed and stored as well
        // the expiratin date must be checked to ensure their token is still valid
        var expirationDate = !PlayerPrefs.HasKey("HasteAccessToken") ?
            DateTime.MinValue :
            (!PlayerPrefs.HasKey("HasteExpiration") ?
                DateTime.MinValue :
                DateTime.ParseExact(PlayerPrefs.GetString("HasteExpiration"), "yyyyMMddHHmmss", new CultureInfo("en-US")));
        if (expirationDate < DateTime.Now)
        {
            // Starts the login flow, which will open the browser. The login function expects a callback
            // The callback will ultimately have an error message or the login details.
            StartCoroutine(HasteIntegration.Instance.Client.Login(this.CompletedLoginInit));
        }
        else
        {
            // The user is already logged in
            //SelectPlayer();
            StartClient();            
        }
    }

    private void SelectPlayer()
    {
        GameObject UIManangerObj = GameObject.Find("UIManager");
        UIManangerObj.GetComponent<UIMananger>().CarSelection.SetActive(true);
        UIManangerObj.GetComponent<UIMananger>().TitleScreen.SetActive(false);
    }

    public void StartClient()
    {
        if (!clientStarted)
        {
            GameObject UIManangerObj = GameObject.Find("UIManager");
            UIManangerObj.GetComponent<UIMananger>().CarSelection.SetActive(false);
            UIManangerObj.GetComponent<UIMananger>().TitleScreen.SetActive(false);
            UIManangerObj.GetComponent<UIMananger>().LeaderboardSelect.SetActive(true);

            clientStarted = true;
            //UIMananger.Instance.clientUI.SetActive(true);
            
            // Active the network client, which will create a player on the server.
            // The player is required to perform any kind of commands or RPCS
            // which are done in the Leaderboard selection
            var serverUrl = MobileDetection.GetServerUrl();
            NetworkManager.singleton.networkAddress = serverUrl;
            var networkClient = NetworkManager.singleton.GetComponent<SimpleWebTransport>();
            networkClient.clientUseWss = serverUrl != "localhost";
            networkClient.port = (serverUrl != "localhost" ? (ushort)443 : (ushort)7778);

            NetworkManager.singleton.StartClient();
                        
            Debug.Log("TitleScreen.StartClient: serverUrl=" + serverUrl + ", port= " + networkClient.port.ToString());
        }
    }

    private void CompletedLoginInit(HasteCliResult cliResult)
    {
        // Once the /cli endpoint is hit, then we need to poll to wait for the user to truly login
        StartCoroutine(HasteIntegration.Instance.Client.WaitForLogin(cliResult, this.CompletedLogin));
    }
    private void CompletedLogin(HasteLoginResult loginResult)
    {
        PlayerPrefs.SetString("HasteAccessToken", loginResult.access_token);
        PlayerPrefs.SetString("HasteExpiration", loginResult.expiration.ToString("yyyyMMddHHmmss"));
        Debug.Log("CompletedLogin: HasteAccessToken= " + loginResult.access_token + ", HasteExpiration= " + loginResult.expiration.ToString () + " saved in PlayerPrefs");

        if (String.IsNullOrEmpty(loginResult.access_token))
        {
            Debug.LogError("An error occurred during login");
            // TODO Need to display an error message to the user here
        }
        else
        {
            StartClient();
            //SelectPlayer();
        }

    }
}