using Mirror;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Leaderboards : NetworkBehaviour
{
    private readonly SyncList<HasteLeaderboardDetail> hasteLeaderboards = new SyncList<HasteLeaderboardDetail>();
    private GameObject lstObj;
    private TMPro.TMP_Dropdown list;
    private Dictionary<string, string> dicLeaderBoard = new Dictionary<string, string>();
    private List<TMP_Dropdown.OptionData> lstData = new List<TMP_Dropdown.OptionData>();
    private float passTime = 0.0f;
    private string leaderBoardValue;
    public bool clickedPlay = false;

    // Start is called before the first frame update
    void Start()
    {
        this.name = "Leaderboards";
        if (isServer)
        {
            if (HasteIntegration.Instance.Server.Leaderboards == null)
                return;
            Debug.Log("Leaderboards.Start:  HasteIntegration.Instance.Server.Leaderboards.count=" + HasteIntegration.Instance.Server.Leaderboards.Length );
            // populate the sync list with results from the Haste Server integration
            foreach (var leaderboard in HasteIntegration.Instance.Server.Leaderboards)
            {
                hasteLeaderboards.Add(leaderboard);
                //Debug.Log("Leaderboards.Start: isServer - leaderboard= " + leaderboard);
            }
        }
        else
        {
            if ( hasteLeaderboards.Count == 0)
            {
                //GameObject.Find("LeaderboardSelection").GetComponent<LoadingProcess>().NotfyErro
                return;
            }
            GameObject.Find("LeaderboardSelection").GetComponent<LoadingProcess>().ActiveLeaderBoard();
            int idx = 0;
            // now that we are on the client with the leaderboard details we can dynamically add buttons to the canvas
            foreach (var leaderboard in hasteLeaderboards)
            {                    
                TMP_Dropdown.OptionData optData = new TMP_Dropdown.OptionData();

                switch (idx)
                {
                    case 0:
                        optData.text = "Practice - Free";
                        break;
                    case 1:
                        optData.text = "Nano - $0.01";
                        break;
                    case 2:
                        optData.text = "Micro - $0.10";
                        break;
                    case 3:
                        optData.text = "Macro - $1.00";
                        break;
                    case 4:
                        optData.text = "Mega - $10.00";
                        break;
                    case 5:
                        optData.text = "High Roller - $100";
                        break;
                }
                    
                //optData.text = leaderboard.name;
                lstData.Add(optData);
                dicLeaderBoard.Add(leaderboard.name, leaderboard.id);

                lstObj = GameObject.Find("drd_LeaderBoard");
                list = lstObj.GetComponent<TMPro.TMP_Dropdown>();
                idx += 1;
                //Debug.Log("Leaderboards.Start: isClient - label.text= " + leaderboard.name + ", button.name= " + leaderboard.id);
            }
            if (list)
                list.AddOptions(lstData);

            GameObject btn_Play = GameObject.Find("btn_Play");
            var button = btn_Play.GetComponentsInChildren<Button>().FirstOrDefault();
            button.onClick.AddListener(delegate { selectLeaderBoard(); }); // on Client Side

        }
    }

    //connect every 10s for keeping wss connection
    [ClientCallback]
    void LateUpdate()
    {
        if (list == null ) return;
        passTime += Time.deltaTime;
        if (passTime > 20.0f )
        {
            passTime = 0.0f;
            var jwtService = new JWTService();
            string _playerId = "";// jwtService.GetPlayerId(PlayerPrefs.GetString("HasteAccessToken"));
            string _leaderBoardId = "";// dicLeaderBoard.Values.ElementAt(list.value); ;
            //Debug.Log("Leaderboards.KeepConnect! leaderboardId =" + _leaderBoardId + ", leaderboardId =" + _playerId);
            CmdEmptyRequest(_leaderBoardId, _playerId);
        }
    }

    [Command]
    void CmdEmptyRequest(string leaderBoardId, string playerId)
    {
        StartCoroutine(HasteIntegration.Instance.Server.GetTopScore(leaderBoardId, playerId, EmptyFunc));
    }

    private void EmptyFunc(HasteServerTopScore EmptyFunc)
    {
        //int i = 0;
    }      

    public void selectLeaderBoard()
    {
        //Debug.Log("Leaderboards.selectLeaderBoard!_Client: clicked = " + clickedPlay );
        if (clickedPlay) return;
        GameObject.Find("LeaderboardSelection").GetComponent<LoadingProcess>().LoadingBars.SetActive(true);
        clickedPlay = true;
        int lstIdx = list.value;
        //string leaderBoardValue = dicLeaderBoard[list.value ];
        leaderBoardValue = dicLeaderBoard.Values.ElementAt(lstIdx);

        PlayerPrefs.SetString("HasteLeaderboardId", leaderBoardValue);
        Debug.Log("selectLeaderBoard: HasteLeaderboardId =" + leaderBoardValue + " saved in PlayerPrefs!");
        Debug.Log("selectLeaderBoard - payment flow-1: JWT= " + PlayerPrefs.GetString("HasteAccessToken") + ", leaderboardId=" + leaderBoardValue);
        CmdSelectPayment(PlayerPrefs.GetString("HasteAccessToken"), leaderBoardValue);
    }

    [Command]
    void CmdSelectPayment(string JWT, string leaderboardId)
    {
        Debug.Log("CmdSelectPayment - payment flow-2! JWT= " + JWT + ", leaderboardId =" + leaderboardId);
        // kick off the payment flow via haste Play endpoint
        //Debug.Log("CmdSelectPayment: HasteLeaderboardId =" + leaderboardId + " saved in PlayerPrefs!");
        StartCoroutine(HasteIntegration.Instance.Server.Play(JWT, leaderboardId, PlayResult));
    }

    [TargetRpc]
    void RpcSetError(string errorMessage)
    {
        Debug.Log("RpsSetError: erroMessage =" + errorMessage);
        GameObject txt_Info = GameObject.Find("txt_Notice");        
        txt_Info.GetComponent<TMPro.TextMeshProUGUI>().text = errorMessage;       
    }

    [TargetRpc]
    void RpcStartGame( string playId)
    {
        Debug.Log("RpcStartGame: HastePlayId =" + playId + " saved in PlayerPrefs!");
        PlayerPrefs.SetString("HastePlayId", playId);
        // hide all canvas elements to show the underlying game
        GameObject UIManangerObj = GameObject.FindWithTag("UIManager");
        UIManangerObj.GetComponent<UIMananger>().LeaderboardSelect.SetActive(false);
        UIManangerObj.GetComponent<UIMananger>().InGameUI.SetActive(true);
        
        var uiCamera = GameObject.Find("MenuCamera");
        uiCamera.SetActive(false);
        GameObject.Find("StartObjects").GetComponent<Continue>().Play();// client side
    }
    
    void PlayResult(HasteServerPlayResult playResult)
    {        
        if (playResult != null)
        {
            Debug.Log("PlayResult - payment flow-4: playId =" + playResult.id);
            string statusString = GameObject.Find("ServerStatus").GetComponent<TMP_Text>().text;
            GameObject.Find("ServerStatus").GetComponent<TMP_Text>().text = statusString + "\n" + "PlayResult: playId =" + playResult.id + ", playResult.message =" + playResult.message;

            if (!string.IsNullOrEmpty(playResult.message))
            {
                RpcSetError(playResult.message);
            }
            else
            {
                //Debug.Log("PlayResult: HastePlayId =" + playResult.id + " saved in PlayerPrefs!");
                //((HasteMirrorNetManager)NetworkManager.singleton).StartGameInstanceForPlayer(GetComponent<NetworkIdentity>().connectionToClient);
                RpcStartGame(playResult.id);
            }
        }
    }


}
