using UnityEngine;
using TMPro;
using System.Collections;


using UnityEngine.Networking;

namespace Mirror.Examples.MultipleAdditiveScenes
{
    public class PlayerScore : NetworkBehaviour
    {
        [SyncVar]
        private int countTime = 50;
        [SyncVar]
        private double startTime;

        //[SyncVar]
        private int timeRemaining;

        [SyncVar]
        public uint playerNumber;

        [SyncVar]
        private int score;

        [SyncVar]
        private bool hasEnded = false;

        [SyncVar]
        private bool hasStarted = false;

        private UIMananger UIManagerObj;

        private GameObject txtScoreAdd;
        private GameObject txtTimeAdd;
        private int timeAdd;
        private int scoreAdd;
        

        [SerializeField]
        private AudioClip collectCoin;
        [SerializeField]
        private GameObject destoryFX;
        private const int vSpeedTimes = 2;
        private void Awake()
        {
            Debug.Log("PlayerScore.Awake!");
        }

        private void Start()
        {
            Debug.Log("PlayerScore.Start!");
            UIManagerObj = GameObject.Find("UIManager").GetComponent<UIMananger>();//UIMananger.Instance; //
            hasEnded = false;
            hasStarted = false;
            timeRemaining = countTime;
        } 

        [Command]
        public void CmdSetSync_hasStart(bool value)
        {
            hasStarted = value;
            if (value)
            {
                startTime = NetworkTime.time;
                Debug.Log("PlayerScore.CmdSetSync_hasStart: startTime =" + startTime);
            }
        }

        [Command]
        public void CmdSetSync_hasEnded(bool value)
        {
            hasEnded = value;
        }

        [ClientCallback]
        void Update() // 
        {
            if (!hasStarted) return; //hasStarted will be true at Countdown end

            if (timeRemaining > 0)
            {
                timeRemaining = countTime - (int)(NetworkTime.time - startTime);//Time.deltaTime; //Time.deltaTime;
                //Debug.Log("PlayerScore.Update: timeRemaining =" + timeRemaining);
            }
            else 
            {
                if (!hasEnded)
                {
                    //Debug.Log("PlayerScore.Update: has Ended ! hasEnded =" + hasEnded + ", hasStart =" + hasStarted);
                    
                    //hasEnded = true;
                    //hasStarted = false;
                    CmdSetSync_hasEnded(true);
                    CmdSetSync_hasStart(false);
                    
                    RpcGetIdsforScore();
                    //DisplayCount();
                }
            }
        }

        [ClientCallback]
        private void FixedUpdate()
        {
            if (!hasEnded && hasStarted)
            {
                StartCoroutine(DisplayCount());
            }
        }

        protected IEnumerator DisplayCount()
        {
            if (timeRemaining < 0) timeRemaining = 0;
            //int clientTimeRemain = countTime - (int)(NetworkTime.time - startTime);
            //if (clientTimeRemain < 0) clientTimeRemain = 0;
            UIManagerObj.txtScore.GetComponent<TextMeshProUGUI>().text = score.ToString();
            if (timeRemaining < 6)
            {
                UIManagerObj.txtTimeleft.GetComponent<TMP_Text>().color = Color.red;
            }
            else
                UIManagerObj.txtTimeleft.GetComponent<TMP_Text>().color = Color.white;
            UIManagerObj.txtTimeleft.GetComponent<TMP_Text>().text = timeRemaining.ToString(); //timeRemaining.ToString();

            //Debug.Log("PlayerSocre.DisplayCount!  timeRemaining =" + timeRemaining + ", countTime = " + countTime);
            yield return null;
        }

        //[TargetRpc]
        void RpcGetIdsforScore()//
        //void GetIdsforScore()
        {
            string playId = PlayerPrefs.GetString("HastePlayId");
            string leaderboardId = PlayerPrefs.GetString("HasteLeaderboardId");

            //Debug.Log("PlayerScore.EndGame: playId =" + playId + ", leaderboardId=" + leaderboardId);
            CmdScore(playId, leaderboardId);
        }

        [Command]
        void CmdScore(string playId, string leaderboardId)
        {
            //Debug.Log("PlayerSocre.cmdScore: playId= " + playId + ", leaderboardID=" + leaderboardId);
            StartCoroutine(HasteIntegration.Instance.Server.Score(score.ToString(), playId, leaderboardId, ScoreResult));
        }

        [TargetRpc]
        void RpcSetError(string errorMessage)
        {
            Debug.Log("PlayerScore.RpcSetError: errorMessage=" + errorMessage);
            //resultMsg = errorMessage;
            UIManagerObj.txtResult.GetComponent<TMPro.TextMeshProUGUI>().text = errorMessage;
            GameObject.Find("txtResult_f").GetComponent<TMPro.TextMeshProUGUI>().text = errorMessage;
        }

        [TargetRpc]
        void RpcEndGame(string message)
        {
            //Debug.Log("PlayerScore.RpcEndGame: is started !");
            //resultMsg = message;
            //UIManagerObj.LeaderboardSelect.SetActive(false);
            UIManagerObj.InGameUI.SetActive(false);
            UIManagerObj.FinalScoreScreen.SetActive(true);

            UIManagerObj.txtResult.GetComponent<TMPro.TextMeshProUGUI>().text = message;

            GameObject.Find("txtResult_f").GetComponent<TMPro.TextMeshProUGUI>().text = message;

            Debug.Log("PlayerScore.RpcEndGame: is ended !");
        }

        void ScoreResult(HasteServerScoreResult scoreResult)
        {
            if (scoreResult != null)//&& !isScoreResult)
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
                    RpcEndGame($"Your score was {score} and you placed {scoreResult.leaderRank + 1} on the leaderboard.");
                }
            }
            Debug.Log("PlayerScore.ScoreResult: is ended !");
        }

        private GameObject prevObj;
        [ClientCallback]
        void OnTriggerEnter(Collider other)//private void OnCollisionEnter(Collision other)
        {
            Debug.Log("PlayerScore.OnTriggerEnter: Object =" + other.gameObject.ToString());

            if (hasEnded) return;
            if (other.gameObject.CompareTag("AICarCollider") || other.gameObject.CompareTag("RoadBand"))
            {
                //Debug.Log("OnTriggerEnter: velocity =" + gameObject.GetComponent<Rigidbody>().velocity.magnitude);
                if (gameObject.GetComponent<Rigidbody>().velocity.magnitude < 2.0f)
                    return;
               
                int mScore = Mathf.CeilToInt(gameObject.GetComponent<Rigidbody>().velocity.magnitude) * vSpeedTimes;

                UIMananger UImanagerObj = GameObject.Find("UIManager").GetComponent<UIMananger>();
                GameObject txtTimeDec = UImanagerObj.txt_Time_Dec;//Instantiate(txtScoreAddPrefab, txtScoreAddPrefab.transform.position, Quaternion.identity);
                GameObject txtScoreDec = UImanagerObj.txt_Score_Dec ;//Instantiate(txtScoreAddPrefab, txtScoreAddPrefab.transform.position, Quaternion.identity);
                txtScoreDec.GetComponent<DisplayPeriod>().displayPeriod = 3f;
                txtTimeDec.GetComponent<DisplayPeriod>().displayPeriod = 3f;
                txtScoreDec.GetComponent<TMP_Text>().text = (-mScore).ToString();
                txtTimeDec.GetComponent<TMP_Text>().text = (-2).ToString();
                CmdAICarCollideScore(mScore);
                //TargetSetScore(other.gameObject.GetComponent<NetworkIdentity>().connectionToClient);
                //Debug.Log("CarConroller_Collision.Ohers OnCollisoionEnter: other = " + other.gameObject.ToString());
            }
            if (other.gameObject.CompareTag("Coin"))
            {
                // push this away from player...a bit less force for host player
                //if (other.gameObject.GetComponent<NetworkIdentity>().connectionToClient.connectionId == NetworkConnection.LocalConnectionId)

                UIMananger UImanagerObj = GameObject.Find("UIManager").GetComponent<UIMananger>();
                GameObject txtTimeInc = UImanagerObj.txt_Time_Inc;//Instantiate(txtScoreAddPrefab, txtScoreAddPrefab.transform.position, Quaternion.identity);
                GameObject txtScoreInc = UImanagerObj.txt_Score_Inc;//Instantiate(txtScoreAddPrefab, txtScoreAddPrefab.transform.position, Quaternion.identity);

                uint grade = other.gameObject.GetComponent<PhysicsCollision>().grade;
                GetScorebyType(grade);

                string strTimeAdd = txtTimeInc.GetComponent<TMP_Text>().text;
                string strScoreAdd = txtScoreInc.GetComponent<TMP_Text>().text;

                int curTimeAdd = 0;
                int curScoreAdd = 0;
                txtScoreInc.GetComponent<DisplayPeriod>().displayPeriod = 3f;
                txtTimeInc.GetComponent<DisplayPeriod>().displayPeriod = 3f;

                if (int.TryParse(strScoreAdd, out curScoreAdd))
                    txtScoreInc.GetComponent<TMP_Text>().text = "+" + (curScoreAdd + scoreAdd).ToString();
                if (int.TryParse(strTimeAdd, out curTimeAdd))
                    txtTimeInc.GetComponent<TMP_Text>().text = "+" + (curTimeAdd + timeAdd).ToString();

                Debug.Log("PlayerSocre.OnCollision: grade =" + grade + ", strScoreAdd = " + strScoreAdd + ", scoreAdd = " + scoreAdd.ToString());

                CmdCoinGetScore(grade);                

                Instantiate(destoryFX, other.gameObject.transform.position, other.gameObject.transform.rotation);
                Destroy(other.gameObject);// Client Objet
                if (collectCoin != null)
                {
                    AudioSource source = gameObject.AddComponent<AudioSource>();
                    source.clip = collectCoin;
                    source.Play();
                }
            }

        }

        [Command]
        void CmdCoinGetScore(uint grade)
        {
            // The Score of server side            
            GetScorebyType(grade);
            score += scoreAdd;
            countTime += timeAdd;

            //TargetSetScore(other.gameObject.GetComponent<NetworkIdentity>().connectionToClient);
            //Object.Destroy(gameObject);// Server Object
        }

        void GetScorebyType(uint grade)
        {
            switch (grade) // server grade == client grade
            {
                case 1:

                    scoreAdd = 100;
                    timeAdd = 4;
                    break;

                case 2:

                    timeAdd = 80;
                    timeAdd = 3;
                    break;

                case 3:

                    scoreAdd = 50;
                    timeAdd = 2;
                    break;

                default:

                    scoreAdd = 40;
                    timeAdd = 2;
                    break;
            }
        }

        [Command]
        void CmdAICarCollideScore(int mSocre)
        {
            score -= mSocre;
            if (score < 0) score = 0;
            countTime -= 2;

            //Debug.Log("PlayerScore.CmdAICarCollide: Score Down");
        }

        [Command]
        public void CmdReady2Change(int idx)
        {
            NetworkConnection conn = gameObject.GetComponent<NetworkIdentity>().connectionToClient;
             
            //Debug.Log("PlayerScore.CmdReady2Change: mIdx=" + idx.ToString() + ", conn=" + conn.ToString());
            NetworkManager.singleton.ReplacePlayer(idx, conn);                   
        }

    }
    

}