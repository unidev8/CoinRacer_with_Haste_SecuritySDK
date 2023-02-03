using UnityEngine;

public class UIMananger : MonoBehaviour
{
    /*[SerializeField]
    private GameObject _clientUI;
    [SerializeField]
    private GameObject _serverUI;
    [SerializeField]
    private GameObject _InGameUI;
    [SerializeField]
    private GameObject _LeaderboardSelect;
    [SerializeField]
    private GameObject _FinalScoreScreen;
*/
    //[HideInInspector]
    public GameObject clientUI;// { get { return _clientUI; } }
    //[HideInInspector]
    public GameObject serverUI;// { get { return _serverUI; } }
    //[HideInInspector]
    public GameObject InGameUI;// { get { return _InGameUI; } }
    //[HideInInspector]
    public GameObject LeaderboardSelect;// { get { return _LeaderboardSelect; } }
    //[HideInInspector]
    public GameObject FinalScoreScreen;// { get { return _FinalScoreScreen; } }

    public GameObject txtTimeleft;// { get { return _InGameUI; } }
    //[HideInInspector]
    public GameObject txtScore;// { get { return _FinalScoreScreen; } }

    public GameObject txtResult;

    //private static UIMananger _instance = null;

    private UIMananger()
    {
    }

    /*public static UIMananger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIMananger();
            }
            return _instance;
        }
    }*/

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
