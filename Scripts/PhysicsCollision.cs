using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.MultipleAdditiveScenes
{
    //[RequireComponent(typeof(Rigidbody))]
    public class PhysicsCollision : NetworkBehaviour
    {        
        private float liveTime = 0.0f;

        private int timeAdd;
        private int scoreAdd;

        //public GameObject collideParticle;
        [SyncVar]
        public uint grade = 1;

        void OnValidate()
        {
            //if (rigidbody3D == null)
            //rigidbody3D = GetComponent<Rigidbody>();
        }

        void Start()
        {
            //rigidbody3D.isKinematic = !isServer;            
        }

        void Update()
        {
            liveTime += Time.deltaTime;
            if ( liveTime > 5.0f )
            {
                liveTime = 0;
                Destroy(gameObject);
            }
        }      
    }
}
