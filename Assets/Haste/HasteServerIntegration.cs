using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class HasteServerIntegration : HasteRequestBase
{
    private string _apiUrl = "https://api.hastearcade.com";
    private DateTime _tokenExpiration = DateTime.MinValue;    
    private HasteServerAuthResult _configuration;
    public HasteLeaderboardDetail[] Leaderboards { get; set; }
    
    public IEnumerator GetHasteLeaderboards(System.Action<HasteAllLeaderboards> callback)
    {
        if (string.IsNullOrEmpty(_configuration.gameId))
        {
            Debug.LogError("An error occurred during server authentication. Please double check your client id and secret or contact support.");
        }
        var path = $"/arcades/{_configuration.arcadeId}/developergames/{_configuration.gameId}";

        var wholePath = _apiUrl + path;
        Debug.Log("GetHasteLeaderboards: GetRequest/url =" + wholePath + ",  _configuration.access_token =" + _configuration.access_token );

        yield return this.GetRequest<HasteAllLeaderboards>($"{_apiUrl}{path}", callback, _configuration.access_token);
  }

  public IEnumerator ConfigureHasteServer(HasteServerAuthResult serverAuthResult, System.Action<HasteAllLeaderboards> leaderboardCallback)
  {
    _configuration = serverAuthResult;
    TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
    _tokenExpiration = DateTimeOffset.FromUnixTimeSeconds((long)(span.TotalSeconds + serverAuthResult.expires_in)).LocalDateTime;

    Debug.Log("ConfigureHasteServer: _configuration =" + _configuration + "_tokwnExpiration =" + _tokenExpiration);
    yield return GetHasteLeaderboards(leaderboardCallback);
  }

  public IEnumerator GetServerToken(string hasteServerClientId, string hasteServerSecret, string hasteServerEnvironment, System.Action<HasteServerAuthResult> callback)
  {
        // first you need to get a token 
        var path = "/oauth/writetoken";
        var data = new Dictionary<string, string>();
        data.Add("clientId", hasteServerClientId);
        data.Add("clientSecret", hasteServerSecret);
        data.Add("environment", hasteServerEnvironment);

        var wholePath = _apiUrl + path;
        string statusString = GameObject.Find("ServerStatus").GetComponent<TMP_Text>().text;
        GameObject.Find("ServerStatus").GetComponent<TMP_Text>().text = statusString + "\n" + "GetServerToken: is requested!";

        Debug.Log("GetServerToken: PostRequest/url =" + wholePath + ", clientId =" + hasteServerClientId + ", clientSecret =" + hasteServerSecret + ", environment =" + hasteServerEnvironment);

        yield return this.PostRequest<HasteServerAuthResult>(wholePath, data, callback);//$"{_apiUrl}{path}"
  }

  public IEnumerator Play(string jwt, string leaderboardId, System.Action<HasteServerPlayResult> callback)
  {
        var jwtService = new JWTService();
        var playerId = jwtService.GetPlayerId(jwt);
        // first you need to get a token 
        var path = $"/arcades/{_configuration.arcadeId}/games/{_configuration.gameId}/play";
        var data = new Dictionary<string, string>();
        data.Add("playerId", playerId);
        data.Add("leaderboardId", leaderboardId);

        var wholePath = _apiUrl + path;
        Debug.Log("Play - payment flow-3: PostRequest/url =" + wholePath + ", playerId =" + playerId + ", leaderboardId =" + leaderboardId );

        string statusString = GameObject.Find("ServerStatus").GetComponent<TMP_Text>().text;
        GameObject.Find("ServerStatus").GetComponent<TMP_Text>().text = statusString + "\n" + "Play: playerId =" + playerId + ", leaderboardId = "+ leaderboardId;

        yield return this.PostRequest<HasteServerPlayResult>($"{_apiUrl}{path}", data, callback, _configuration.access_token);
  }

  public IEnumerator GetLeaders(string leaderboardId, System.Action<HasteServerLeaderResults> callback)
  {
    var path = $"/arcades/{_configuration.arcadeId}/games/{_configuration.gameId}/leaders/{leaderboardId}";
    yield return this.GetRequest<HasteServerLeaderResults>($"{_apiUrl}{path}", callback);
  }

  public IEnumerator GetTopScore(string leaderboardId, string playerId, System.Action<HasteServerTopScore> callback)
  {
        var path = $"/arcades/{_configuration.arcadeId}/games/{_configuration.gameId}/topscore/{leaderboardId}/{playerId}";
        Debug.Log("HasteServerIntegration.GetTopScore: PostRequest/url= " + _apiUrl+path + "? leaderboardId= " + leaderboardId + ", playerId= " + playerId);

        yield return this.GetRequest<HasteServerTopScore>($"{_apiUrl}{path}", callback);
  }


  public IEnumerator Score(string score, string playId, string leaderboardId, System.Action<HasteServerScoreResult> callback)
  {
       // first you need to get a token 
       var path = $"/arcades/{_configuration.arcadeId}/games/{_configuration.gameId}/score";
       var data = new Dictionary<string, string>();
       data.Add("playId", playId);
       data.Add("leaderboardId", leaderboardId);
       data.Add("score", score);

       var wholePath = _apiUrl + path;
       Debug.Log("HasteServerIntegration.Score: PostRequest/url= " + wholePath + "? playId= " + playId + ", leaderboardId= " + leaderboardId + ", score= " + score);

       yield return this.PostRequest<HasteServerScoreResult>($"{_apiUrl}{path}", data, callback, _configuration.access_token);
       
   }

    void LateUpdate()
    {

    }       
}