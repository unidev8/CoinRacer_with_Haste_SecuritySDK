using UnityEngine;
using TMPro;


namespace Mirror.Examples.MultipleAdditiveScenes
{
    public class PlayerScore : NetworkBehaviour
    {
        private const int cTime = 60;

        [SyncVar]
        public float timeRemaining = cTime;

        [SyncVar]
        public uint playerNumber;

        [SyncVar]
        public int score;

        [SyncVar]
        public bool hasEnded = false;

        [SyncVar]
        public bool hasStarted = false;

        private UIMananger UIManagerObj;
        private bool isScoreResult = false;
        private bool clientEnded = false;
        
        private void Start()
        {
            UIManagerObj = GameObject.Find("UIManager").GetComponent<UIMananger>();
            hasEnded = false;
            hasStarted = false;
            isScoreResult = false;
            clientEnded = false;
        }

        void OnGUI()
        {
            if (isLocalPlayer && !hasEnded)
            {
                UIManagerObj.txtScore.GetComponent<TMPro.TextMeshProUGUI>().text = score.ToString ();
                UIManagerObj.txtTimeleft.GetComponent<TMPro.TextMeshProUGUI>().text = ((int)timeRemaining).ToString ();                
            }           
        }
       
        [Command]
        public void cmdSetSync_hasStart (bool value)
        {
            hasStarted = value;
        }

        [Command]
        public void cmdSetSync_hasEnded(bool value)
        {
            hasEnded = value;
            isScoreResult = false; // sometime callback ScoreResult is called twice on one fuction call.
        }

        [ServerCallback]
        void Update() // OnClient?
        {
            if (!hasStarted) return; //hasStarted will be true at Countdown end

            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                //if (! clientEnded )
                {
                    Debug.Log("PlayerScore.Update: has Ended ! hasEnded =" + hasEnded + ", hasStart =" + hasStarted);
                    {
                        hasEnded = true;
                        hasStarted = false;
                        //CmdSetSync_hasEnded(true);//  for Comunicate latency Use clientEnded on check timeRemaing == 0
                        //CmdSetSync_hasStart(false);
                        //clientEnded = true;
                    }
                    RpcGetIdsforScore();       
                }
            }
        }

        [TargetRpc]
        void RpcGetIdsforScore()//
        {
            string playId = PlayerPrefs.GetString("HastePlayId");
            string leaderboardId = PlayerPrefs.GetString("HasteLeaderboardId");

            Debug.Log("PlayerScore.EndGame: playId =" + playId + ", leaderboardId=" + leaderboardId);
            cmdScore(playId, leaderboardId);
        }

        [Command]
        void cmdScore(string playId, string leaderboardId)
        {
            Debug.Log("PlayerSocre.cmdScore: playId= " + playId + ", leaderboardID=" + leaderboardId);
            StartCoroutine(HasteIntegration.Instance.Server.Score(score.ToString(), playId, leaderboardId, ScoreResult));
        }

        [TargetRpc]
        void RpcSetError(string errorMessage)
        {
            Debug.Log("PlayerScore.RpcSetError: errorMessage=" + errorMessage);
            //resultMsg = errorMessage;
            UIManagerObj.txtResult.GetComponent<TMPro.TextMeshProUGUI>().text = errorMessage;
            GameObject.Find("34523").GetComponent<TMPro.TextMeshProUGUI>().text = errorMessage;
        }

        [TargetRpc]
        void RpcEndGame(string message)
        {
            Debug.Log("PlayerScore.RpcEndGame: is started !");
            //resultMsg = message;
            //UIManagerObj.LeaderboardSelect.SetActive(false);
            UIManagerObj.InGameUI.SetActive(false);
            UIManagerObj.FinalScoreScreen.SetActive(true);
            UIManagerObj.txtResult.GetComponent<TMPro.TextMeshProUGUI>().text = message;

            GameObject.Find("34523").GetComponent<TMPro.TextMeshProUGUI>().text = message;

            Debug.Log("PlayerScore.RpcEndGame: is ended !");
        }        
          
        void ScoreResult(HasteServerScoreResult scoreResult)
        {
            if (scoreResult != null )//&& !isScoreResult)
            {
                Debug.Log("PlayerScore.ScoreResult: is started !");
                //isScoreResult = true;

                if (!string.IsNullOrEmpty(scoreResult.message))
                {
                    RpcEndGame(scoreResult.message);
                }
                else
                {
                    Debug.Log($"PlayerScore.ScoreResult:Your score was {score} and you placed {scoreResult.leaderRank + 1} on the leaderboard.");
                    // change scenes
                    RpcEndGame($"Your score was {score} and you placed {scoreResult.leaderRank + 1 } on the leaderboard.");
                }
            }
            Debug.Log("PlayerScore.ScoreResult: is ended !");
        }

        GameObject playerObj;
        private GameObject prevObj;
        [ClientCallback]
        private void OnCollisionEnter(Collision other)
        {
            if (hasEnded) return;
            if (other.gameObject.CompareTag("AICarCollider") || other.gameObject.CompareTag("RoadBand"))
            {
                if (other.gameObject == prevObj) return;
                prevObj = other.gameObject;

                GameObject txtTimeAdd = GameObject.FindWithTag("UI_TimeAdd");//Instantiate(txtScoreAddPrefab, txtScoreAddPrefab.transform.position, Quaternion.identity);
                GameObject txtScoreAdd = GameObject.FindWithTag("UI_ScoreAdd");//Instantiate(txtScoreAddPrefab, txtScoreAddPrefab.transform.position, Quaternion.identity);
                txtScoreAdd.GetComponent<DisplayPeriod>().displayPeriod = 3.0f;
                txtTimeAdd.GetComponent<DisplayPeriod>().displayPeriod = 3.0f;  

                txtScoreAdd.GetComponent<TMP_Text>().color = Color.red;
                txtTimeAdd.GetComponent<TMP_Text>().color = Color.red;
                txtScoreAdd.GetComponent<TMP_Text>().text = ( - 100).ToString();
                txtTimeAdd.GetComponent<TMP_Text>().text = ( - 2 ).ToString();
                CmdAICarCollideScore();
                //TargetSetScore(other.gameObject.GetComponent<NetworkIdentity>().connectionToClient);
                Debug.Log("CarConroller_Collision.Ohers OnCollisoionEnter: other = " + other.gameObject.ToString());
            }            
        }

        [Command]
        void CmdAICarCollideScore ()
        {
            score -= 100;
            timeRemaining -= 2;
            Debug.Log("PlayerScore.CmdAICarCollide: Score Down");
        }
       
    }    
}