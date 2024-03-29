using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HasteClientIntegration : HasteRequestBase
{
    private string authServerUrl = "https://authservice.hastearcade.com"; // These should be production
    private string authClientUrl = "https://authclient.hastearcade.com";
    private bool completedLoginFlow = false;
    private System.Action<HasteLoginResult> _finalCallback;
    private System.Action<HasteCliResult> _cliCallback;
    private HasteServerAuthResult configuration;
    public HasteLeaderboardDetail[] Leaderboards { get; set; }

    public IEnumerator Login(System.Action<HasteCliResult> callback)
    {
        this._cliCallback = callback;
        var data = new Dictionary<string, string>();

        // make the initial request to the cli service to initiate a login
        data.Add("description", $"{SystemInfo.operatingSystem} - {SystemInfo.deviceName}");

        var wholePath = authServerUrl + "/cli";
        Debug.Log("HasteServerIntegration.Login: PostRequest/url= " + wholePath + ", description =" + $"{SystemInfo.operatingSystem} - {SystemInfo.deviceName}");

        yield return this.PostRequest<HasteCliResult>($"{authServerUrl}/cli", data, this.ParseCliRequest);

    }

    private void ParseCliRequest(HasteCliResult result)  
    {
        if (result != null && !String.IsNullOrEmpty(result.token))
        {
            this._cliCallback(result);
        }
    }

    public IEnumerator WaitForLogin(HasteCliResult cliResult, System.Action<HasteLoginResult> finalCallback)
    {
        this._finalCallback = finalCallback;
        var browserUrl = $"{authClientUrl}{cliResult.browserUrl}";
        var cliUrl = $"{authServerUrl}{cliResult.cliUrl}/{cliResult.requestorId}";

        Application.OpenURL(browserUrl);

        // loop until the user logs in
        while (!completedLoginFlow)
        {
            yield return new WaitForSeconds(3f);
            
            Debug.Log("HasteServerIntegration.WaitForLogin: GetRequest/url =" + cliUrl + "? token =" + cliResult.token );

            yield return this.GetRequest<HasteLoginResult>($"{cliUrl}", this.ParseLoginCheck, cliResult.token);
        }
    }
    private void ParseLoginCheck(HasteLoginResult loginResult)
    {
        if (loginResult != null && !String.IsNullOrEmpty(loginResult.access_token))
        {
            completedLoginFlow = true;
            var jwtService = new JWTService();

            var expiration = jwtService.GetExpiryTimestamp(loginResult.access_token);
            loginResult.expiration = expiration;
            this._finalCallback(loginResult);
        }
    }

}