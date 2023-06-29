using Mirror;
using Mirror.Examples.MultipleAdditiveScenes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SelectNFTCar : MonoBehaviour
{
    private int mIdx = 0;
    private static SelectNFTCar _instance = null;
    [SerializeField]
    private GameObject pannel;

    // bool next = false, prev = false;
    // float dist = 0.0f;
    //private int speed = 5000;

    public static SelectNFTCar Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SelectNFTCar();
            }
            return _instance;

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mIdx = 0;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (next)
        {
            if (dist < Screen.width)
            {
                dist += Time.deltaTime * speed;
                pannel.transform.Translate(new Vector3(-Time.deltaTime * speed, 0.0f, 0.0f));
                Debug.Log("Next: screen.width=" + Screen.width);
            }
            else
            {
                next = false;
            }
        }
        else if (prev)
        {
            //if (transform.position.x <= 100) return;
            if (dist < Screen.width)
            {
                dist += Time.deltaTime * speed;
                pannel.transform.Translate(new Vector3(Time.deltaTime * speed, 0.0f, 0.0f));
            }
            else
            {
                prev = false;
            }
        }
        */
    }

    public void NextCars()
    {        
        if (pannel.transform.localPosition.x > -5700)
            pannel.transform.Translate(new Vector3( -Screen.width, 0.0f, 0.0f));
        //Debug.Log("Next: x=" + pannel.transform.position.x + ", localX = " + pannel.transform.localPosition.x);
        //next = true;
        //prev = false;
        //dist = 0.0f;
    }
    public void PrevCars()
    {
        if (pannel.transform.localPosition.x < 0)
            pannel.transform.Translate(new Vector3( Screen.width, 0.0f, 0.0f));
        //prev = true;
        //next = false;
        //dist = 0.0f;
    }

    public void GetCarIdx(int idx)
    {
        mIdx = idx;
    }

    public void ExitCarSelection()
    {
        GameObject UIManangerObj = GameObject.FindWithTag("UIManager");
        UIManangerObj.GetComponent<UIMananger>().CarSelection.SetActive(false);
        UIManangerObj.GetComponent<UIMananger>().LeaderboardSelect.SetActive(true);
    }

    public void ChangePlayer()
    {
        Debug.Log("SelectCar.ChangePlayer!");
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScore>().CmdReady2Change(mIdx);
        GameObject UIManangerObj = GameObject.FindWithTag("UIManager");
        UIManangerObj.GetComponent<UIMananger>().CarSelection.SetActive(false);
        UIManangerObj.GetComponent<UIMananger>().LeaderboardSelect.SetActive(true);

        //CmdSetPlayerPrefab();        
    }

 /*   [Command]
    public void CmdSetPlayerPrefab()
    {
        Debug.Log("SelectNFTCar.CmdSetPqhdksdkaghlayerPrefab!");
        NetworkManager.singleton.SetPlayerPrefab(mIdx, this.gameObject);
        
    }

    [TargetRpc]
    public void RpcStartClient()
    {
        Debug.Log("SelectNFTCar.RpcStartClient!");
        GameObject UIObj = GameObject.Find("UI");
        UIObj.GetComponent<TitleScreen>().StartClient();
    }

    public void StartClient()
    {
        RpcStartClient();
    }
 */
}
