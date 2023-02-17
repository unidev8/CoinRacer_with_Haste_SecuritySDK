using UnityEngine;
using TMPro;

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

}
