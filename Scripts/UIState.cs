
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIState : MonoBehaviour
{
    private GameObject UI;
    private GameObject playerUI;
    // Start is called before the first frame update
    void Start()
    {

        UI = GameObject.Find("UI");
        playerUI = GameObject.Find("playerUI");        
        //if (UI.GetComponent<TitleScreen>().Role == 0 )        
            playerUI.SetActive(false);
        //else
            //playerUI.SetActive(true);        
    }

    // Update is called once per frame
   
}
