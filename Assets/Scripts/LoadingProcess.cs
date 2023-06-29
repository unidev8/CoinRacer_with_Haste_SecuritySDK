using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading;
using System;

public class LoadingProcess : MonoBehaviour
{
    public GameObject drdLeaderBoard;
    public GameObject LoadingBars;
    public GameObject txt_Howto;
    public GameObject btnPlay;
    public GameObject txt_Notice;
    public GameObject btnSelNFTCar;
    public GameObject tglSound;

    // Start is called before the first frame update
    void Start()
    {
        LoadingBars.SetActive(true);
        drdLeaderBoard.SetActive(false);        
        txt_Howto.SetActive(false);
        btnPlay.SetActive(false);
        //btnSelNFTCar.SetActive(false);
        //tglSound.SetActive(false);
    }

    public void ChangeValue()
    {
        TMPro.TMP_Dropdown list = drdLeaderBoard.GetComponent<TMPro.TMP_Dropdown>();
        int idx = list.value;
        TMP_Dropdown.OptionData[] listOptions = new TMP_Dropdown.OptionData[list.options.Count];
        listOptions = list.options.ToArray() ;
        string selText = listOptions[idx].text.ToString();
        if (selText == "Macro - $1.00" || selText == "Mega - $10.00" || selText == "High Roller - $100")
        {
            txt_Notice.GetComponent<TMP_Text>().text = $"Sorry, this game is still in testing mode, and {selText} play is not yet available. Come back later";
            btnPlay.SetActive(false);
        }
        else
        {
            btnPlay.SetActive(true);
            txt_Notice.GetComponent<TMP_Text>().text = "";
        }
        GameObject.Find("Leaderboards").GetComponent<Leaderboards>().clickedPlay = false;
       
        //Debug.Log("ChangeValue: idx = " + idx.ToString() + ", selText = " + selText);
    }

    public void ActiveLeaderBoard()
    {      
        //Debug.Log("LoadingProcess.ActiveLeaderBoard:!");
        LoadingBars.SetActive(false);
        drdLeaderBoard.SetActive(true);
        txt_Howto.SetActive(true);
        btnPlay.SetActive(true);
        //btnSelNFTCar.SetActive(true);
        //tglSound.SetActive(true);
    }

    
}
