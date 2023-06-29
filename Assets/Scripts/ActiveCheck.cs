using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //if (!GameObject.Find("UI")) return;
        string isServer = GameObject.Find("UI").GetComponent<TitleScreen>().Role.ToString();
        if (isServer == "Server" )
        {
            gameObject.SetActive(false);
            //Debug.Log("CarController.Start! name = " + gameObject.name);
            return;

        }
    }

}
