using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.MultipleAdditiveScenes
{
    //[RequireComponent(typeof(Rigidbody))]
    public class PhysicsCollision : NetworkBehaviour
    {
        [Tooltip("how forcefully to push this object")]
        //public float force = 12;
        //public Rigidbody rigidbody3D;

        private GameObject playerObj;
        private float liveTime = 0.0f;
        //public float yPos = 0.0f;

        private GameObject txtScoreAdd;
        private GameObject txtTimeAdd;
        private int timeAdd;
        private int scoreAdd;

        [SyncVar]
        public uint grade = 1;

        void OnValidate()
        {
            //grade = 1;
            //if (rigidbody3D == null)
            //rigidbody3D = GetComponent<Rigidbody>();
        }

        void Start()
        {
            //grade = 1;
            //rigidbody3D.isKinematic = !isServer;            
        }

        void Update()
        {
            liveTime += Time.deltaTime;
            if ( liveTime > 4.0f )
            {
                liveTime = 0;
                Destroy(gameObject);
            }
            //transform.Translate ()
        }

        [ClientCallback]
        void OnCollisionEnter(Collision other)
        { 
            if (other.gameObject.CompareTag("Player"))//.CompareTag("Player")
            {
                // push this away from player...a bit less force for host player
                //if (other.gameObject.GetComponent<NetworkIdentity>().connectionToClient.connectionId == NetworkConnection.LocalConnectionId)

                playerObj = other.gameObject;
                if (playerObj.GetComponent<PlayerScore>().hasEnded)
                    return;

                // Debug.Log("PhysicsCollisonEnter.grade=" + grade + ", playerObj = " + playerObj);
                GameObject txtScoreAdd = GameObject.FindWithTag("UI_TimeAdd");//Instantiate(txtScoreAddPrefab, txtScoreAddPrefab.transform.position, Quaternion.identity);
                GameObject txtTimeAdd = GameObject.FindWithTag("UI_ScoreAdd");//Instantiate(txtScoreAddPrefab, txtScoreAddPrefab.transform.position, Quaternion.identity);
                txtScoreAdd.GetComponent<DisplayPeriod>().displayPeriod = 3.0f;
                txtTimeAdd.GetComponent<DisplayPeriod>().displayPeriod = 3.0f;

                GetScorebyType();

                txtScoreAdd.GetComponent<TMP_Text>().color = Color.green;
                txtTimeAdd.GetComponent<TMP_Text>().color = Color.green;

                string strTimeAdd = txtTimeAdd.GetComponent<TMP_Text>().text;
                string strScoreAdd = txtScoreAdd.GetComponent<TMP_Text>().text;
               
                int curTimeAdd = 0;
                int curScoreAdd = 0;

                if (int.TryParse (strScoreAdd, out curScoreAdd))
                    txtScoreAdd.GetComponent<TMP_Text>().text = "+" + (curScoreAdd + scoreAdd).ToString();
                if (int.TryParse (strTimeAdd, out curTimeAdd))
                    txtTimeAdd.GetComponent<TMP_Text>().text = "+" + (curTimeAdd + timeAdd).ToString();

                CmdCoinGetScore();

                Object.Destroy(gameObject);// Client Objet
            }
        }

        [Command]
        void CmdCoinGetScore()
        {
            // The Score of server side
            GameObject playerObj = GameObject.FindWithTag("Player");
            
            GetScorebyType();

            playerObj.GetComponent<PlayerScore>().score += timeAdd;
            playerObj.GetComponent<PlayerScore>().timeRemaining += scoreAdd;
            //TargetSetScore(other.gameObject.GetComponent<NetworkIdentity>().connectionToClient);
            Object.Destroy(gameObject);// Server Object
        }

        void GetScorebyType()
        {
            switch (grade) // server grade == client grade
            {
                case 1:

                    timeAdd = 100;
                    scoreAdd = 4;
                    break;

                case 2:

                    timeAdd = 80;
                    scoreAdd = 3;
                    break;

                case 3:

                    timeAdd = 50;
                    scoreAdd = 2;
                    break;

                default:

                    timeAdd = 40;
                    scoreAdd = 1;
                    break;

            }
        }

        [TargetRpc]
        public void TargetSetScore(NetworkConnection target)        
        {   
            Object.Destroy(gameObject);
            Debug.Log("TargetRPC is called");                   
        }
    }
}
