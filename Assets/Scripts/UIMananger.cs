using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMananger : MonoBehaviour
{
    public GameObject clientUI;
    public GameObject serverUI;
    public GameObject InGameUI;
    public GameObject LeaderboardSelect;
    public GameObject FinalScoreScreen;
    public GameObject txtTimeleft;
    public GameObject txtScore;
    public GameObject txtResult;
    public GameObject txtSpeed;
    public GameObject TitleScreen;
    public GameObject CarSelection;
    public GameObject txt_Score_Inc;
    public GameObject txt_Score_Dec;
    public GameObject txt_Time_Inc;
    public GameObject txt_Time_Dec;
    
    public bool isSound;

    private static UIMananger _instance = null;

    private UIMananger()
    {
    }

    public void ActiveNFTCarMenu ()
    {
        CarSelection.SetActive(true);
        LeaderboardSelect.SetActive(false);
    }

    public static UIMananger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIMananger();
            }
            return _instance;
        }
    }

    public void SoundToggle(Toggle tglSound)
    {
        AudioSource[] sources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        
        if (tglSound.isOn)
        {
            isSound = true;

            for (int index = 0; index < sources.Length; ++index)
            {
                sources[index].mute = false;
                sources[index].volume = 0.3f;
                //Debug.Log("SoundToggle source=" + sources[index].name);
            }
        }
        else
        {
            isSound = false;

            for (int index = 0; index < sources.Length; ++index)
            {
                sources[index].mute = true;
                //Debug.Log("SoundToggle source=" + sources[index].name);
            }
        }
        //Debug.Log("UIManager.SoundToggle: toggle is on =" + tglSound.isOn + ", isSound =" + isSound );
    }

    private void Start()
    {
        isSound = true;
    }
}
