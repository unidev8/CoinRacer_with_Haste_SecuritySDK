using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reload : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayAgain()
    {
        Debug.Log("PlayerScore.PlayAgain: is started!");
       
        StartCoroutine(((HasteMirrorNetManager)NetworkManager.singleton).ReLoadAsyncScene());
      
    }

}
