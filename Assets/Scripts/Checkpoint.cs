using Mirror;
using Mirror.Examples.MultipleAdditiveScenes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//this script is attached to all the checkpoints in the race and measures the car position to the checkpoint

public class Checkpoint : NetworkBehaviour
{
    public GameObject coinPrefav;
    private bool isCollided = false;
    private float elapsTime;    

    //IMPORTANT NOTE: when adding new checkpoints follow the name scheme: Chk1, Chk2, ... Chk13, Chk14
    private void Start()
    {
    }


    [ServerCallback]
    void Update()
    {
        if (!isCollided) return;
        elapsTime += Time.deltaTime;
        if (elapsTime > 30.0f) // after 30s checkpoint be enabled to generate coin (isCollide == false)
            isCollided = false;
    }

    [ClientCallback] //ClientCallback
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("Checkpoint.OncollisionEnter: On Client It is checked!");
            CmdGeneratCoins();

        }
    }

    [Command]
    void CmdGeneratCoins()
    {
        //Debug.Log("Checkpoint.CmdGeneratCoins: On Server It is checked!");
        if (isCollided) return;
        isCollided = true;
        StartCoroutine(SpawnCoin(this.netIdentity.connectionToClient));        
    }


    private IEnumerator SpawnCoin(NetworkConnection conn)
    {
        //if (!NetworkServer.active) return;
        //Debug.Log("Checkpoint.SpawnCoin: On Server It is checked!");

        float dist = 120.0f; // distance between checkpoints,
        float startPos = 50.0f; // start postion of coin.
        uint coinNum = (uint)UnityEngine.Random.Range(1f, 5f);

        //.Log("Checkpoint.SpawnCoin: OnServer It is generated!: coinNum= " + coinNum);

        GameObject coinObj;
        for (uint i = 1; i <= coinNum; i++)
        {
            Vector3 coinPos = transform.position + transform.forward * (startPos + ((dist - startPos) / (coinNum + 1)) * i) + transform.right * UnityEngine.Random.Range(-1f, 1f);//new Vector3(UnityEngine.Random.Range(-4f, 4f), 0.0f, UnityEngine.Random.Range(-4f, 4f));
            
            coinObj = UnityEngine.Object.Instantiate(coinPrefav, coinPos, Quaternion.identity );
            coinObj.GetComponent<PhysicsCollision>().grade = coinNum;
            NetworkServer.Spawn(coinObj, conn);            
            
            SceneManager.MoveGameObjectToScene(coinObj, ((HasteMirrorNetManager)NetworkManager.singleton).subScenes[conn.identity.netId.ToString()]); //GetScene
            //Debug.Log("Checkpoint.OncollisionEnter: Coin.grade= " + coinObj.GetComponent<PhysicsCollision>().grade.ToString());            
        }
        yield return null;
    }

}
