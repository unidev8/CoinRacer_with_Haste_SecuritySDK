using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Mirror;
using Mirror.Examples.MultipleAdditiveScenes;
using TMPro;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

// This example was started from the MultiSceneAdditive example in Mirror
[AddComponentMenu("")]
public class HasteMirrorNetManager : NetworkManager
{
    [Header("Spawner Setup")]
    [Tooltip("Checkpoint Prefab for generationg coins")]
    public GameObject[] rewardPrefab; // checkpoint for generating coins

    [Tooltip("Leaderboard Prefab")]
    public GameObject leaderboardPrefab;
    [Header("Game Setup")]
    [Scene]
    public string gameScene;

    readonly Dictionary<string, Scene> subScenes = new Dictionary<string, Scene>();

    readonly Dictionary<string, GameObject> leaderboards = new Dictionary<string, GameObject>();

    public void StartGameInstanceForPlayer(NetworkConnection conn)
    {
        PlayerScore playerScore = conn.identity.GetComponent<PlayerScore>();
        playerScore.playerNumber = conn.identity.netId;
        //playerScore.timeRemaining = 15; in unity editor;
        //playerScore.hasStarted = true;

        var newScene = subScenes[conn.identity.netId.ToString()];
        //Spawner.InitialSpawn(newScene);

        SceneManager.MoveGameObjectToScene(conn.identity.gameObject, subScenes[(conn.identity.netId.ToString())]);
    }

    public void EndGameInstanceForPlayer(NetworkConnection conn)
    {
        /*
        THIS IS WHERE YOU WOULD PERFORM ANY SERVER SIDE CLEANUP OF YOUR PLAYER, ETC

        Example:
        var player = conn.identity.gameObject.GetComponent<Player>();
        player.obstacles.Clear();

        */
    }

    public override void OnStartServer()
    {
        DotEnv.Load("./.env");

        var secret = "o9b-N_qQo1Z-QRmnDfFujR8uTD-ZSia9OsFwHX9wR2FWgvKvON6OQSqSbhfKKcbM";//System.Environment.GetEnvironmentVariable("HASTE_SERVER_SECRET");
        var clientId = "RODg4x2V5Rm7WMoaqYIbhwCdbqsY5NIAbchYrOgsQDPijNn81ktKCtn1hHdRCIqM";// System.Environment.GetEnvironmentVariable("HASTE_SERVER_CLIENT_ID");
        var environment = "production";// System.Environment.GetEnvironmentVariable("HASTE_SERVER_ENVIRONMENT");

        if (string.IsNullOrEmpty(secret))
        {
            secret = System.Environment.GetEnvironmentVariable("HASTE_SERVER_SECRET", EnvironmentVariableTarget.User);
            clientId = System.Environment.GetEnvironmentVariable("HASTE_SERVER_CLIENT_ID", EnvironmentVariableTarget.User);
            environment = System.Environment.GetEnvironmentVariable("HASTE_SERVER_ENVIRONMENT", EnvironmentVariableTarget.User);
        }

        if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(environment))
        {
            Debug.LogError("Please ensure that you have created a .env file in your root directory or you have set user level environment variables.");
        }
        Debug.Log("HasteMirrorNetMangager.OnstartServer: HASTE_SERVER_CLIENT_ID= " + clientId + ", HASTE_SERVER_SECRET= " + secret + ", HASTE_SERVER_ENVIRONMENT= " + environment);

        StartCoroutine(HasteIntegration.Instance.Server.GetServerToken(clientId, secret, environment, GetHasteTokenCompleted));
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var newScene = SceneManager.LoadScene(gameScene, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });

        subScenes.Add(conn.identity.netId.ToString(), newScene);
        conn.Send(new SceneMessage { sceneName = gameScene, sceneOperation = SceneOperation.LoadAdditive });

        SceneManager.MoveGameObjectToScene(conn.identity.gameObject, subScenes[(conn.identity.netId.ToString())]);
        SpawnLeaderboard(conn);
        // spawn checkpoint to generating coins
        for (int i = 0; i < rewardPrefab.Length; i++)
        {
            StartCoroutine(SpawnRewardPoint(conn, rewardPrefab[i]));
        }
    }

    void SpawnLeaderboard(NetworkConnection conn)
    {
        if (!NetworkServer.active) return;

        Vector3 spawnPosition = new Vector3(0, 0, 0);
        GameObject leaderboard = UnityEngine.Object.Instantiate(((HasteMirrorNetManager)NetworkManager.singleton).leaderboardPrefab, spawnPosition, Quaternion.identity);
        leaderboard.name = "Leaderboards";
        NetworkServer.Spawn(leaderboard, conn);
        SceneManager.MoveGameObjectToScene(leaderboard, subScenes[(conn.identity.netId.ToString())]);        
    }


    IEnumerator SpawnRewardPoint(NetworkConnection conn, GameObject rewardPrefab)
    {
        if (!NetworkServer.active) yield return null;

        yield return new WaitForSeconds(0.05f);

        Vector3 spawnPosition = rewardPrefab.transform.position;
        Quaternion spawnRoat = rewardPrefab.transform.rotation;
        GameObject rewardPoint = UnityEngine.Object.Instantiate(rewardPrefab, spawnPosition, spawnRoat);
        //Debug.Log("SpawnRewardPoint: rewardPoin = " + rewardPoint.ToString());
        NetworkServer.Spawn(rewardPoint, conn);
        //SceneManager.MoveGameObjectToScene(rewardPoint, subScenes[(conn.identity.netId.ToString())]);        
    }

    private void GetHasteTokenCompleted(HasteServerAuthResult result)
    {
        if (result != null)
        {
            StartCoroutine(HasteIntegration.Instance.Server.ConfigureHasteServer(result, ConfigureHasteServerCompleted));
        }
    }

    private void ConfigureHasteServerCompleted(HasteAllLeaderboards leaderboards)
    {
        if (leaderboards != null)
        {
            HasteIntegration.Instance.Server.Leaderboards = leaderboards.leaderboards;
            string statusString = GameObject.Find("ServerStatus").GetComponent<TMP_Text>().text;
            GameObject.Find("ServerStatus").GetComponent<TMP_Text>().text = statusString + "\n" + "ConfigureHasteServerCompleted:LeaderBoards is sucessfully loaded!";
            Debug.Log("HasteMiroNetManager.ConfigureHasteServerCompleted: leaderoards = " + leaderboards+ "");
        }
    }

    /// <summary>
    /// This is called when a server is stopped - including when a host is stopped.
    /// </summary>
    public override void OnStopServer()
    {
        NetworkServer.SendToAll(new SceneMessage { sceneName = gameScene, sceneOperation = SceneOperation.UnloadAdditive });
        StartCoroutine(ServerUnloadSubScenes());
    }

    // Unload the subScenes and unused assets and clear the subScenes list.
    IEnumerator ServerUnloadSubScenes()
    {
        foreach (var scene in subScenes)
            yield return SceneManager.UnloadSceneAsync(scene.Value);

        subScenes.Clear();
        yield return Resources.UnloadUnusedAssets();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        try
        {
            StartCoroutine(ClientUnloadSubScenes(conn.identity.netId.ToString()));
            base.OnServerDisconnect(conn);
        }
        catch (Exception ex)
        {
            Debug.LogError("An error occurred while disconnecting a client. The error is " + ex.Message + ". The inner message is " + ex.InnerException.Message);
        }
    }

    public Nullable<Scene> GetScene(string netId)
    {
        Scene sceneToRemove;
        if (subScenes.TryGetValue(netId, out sceneToRemove))
            return sceneToRemove;
        else
            return null;
    }

    // Unload all but the active scene, which is the "container" scene
    IEnumerator ClientUnloadSubScenes(string netId)
    {
        Scene? sceneToRemove = GetScene(netId);
        if (sceneToRemove != null)
        {
            subScenes.Remove(netId);
            yield return SceneManager.UnloadSceneAsync(sceneToRemove.Value);
        }
    }

    public IEnumerator ReLoadAsyncScene() // Self
    {
        Debug.Log("HasteMirrorNetManager.ReloadAsyncScene: Client Scene will be reload!");
        // Set the current Scene to be able to unload it later
        //Scene currentScene = SceneManager.GetActiveScene();
        //AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(0);
        // The Application loads the Scene in the background at the same time as the current Scene.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);

        // Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

    }

}
