using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using TMPro;
//using UnityEngine.iOS;
//using TouchControlsKit;
using static UnityEngine.AudioSettings;


namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        //[SerializeField]
        private TMP_Text textObj;
        private float mV, mH;
        private const float vSensitive = 3f;
        private const float hSensitive = 5f;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }

        public void Horizontal_P()
        {
            mH = hSensitive;
        }

        public void Horizontal_N()
        {
            mH = -hSensitive;
        }

        public void Vertical_P()
        {
            mV = vSensitive;
        }

        public void Vertical_N()
        {
            mV = -vSensitive;
        }

        private void FixedUpdate()
        {
            float h, v, v_mobile, h_mobile;

            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                h = CrossPlatformInputManager.GetAxis("Horizontal");
                v = CrossPlatformInputManager.GetAxis("Vertical");
                //if (h > 0 || v > 0)
                    //Debug.Log("Horizontal = " + h + "Vertical =" + v);
            }
            else
            {
                /* if (Input.touchCount >0)
                 {
                     Touch touch = Input.GetTouch(0);
                     Debug.Log("touchCount=" + Input.touchCount);
                 }
                 h = Input.touchCount;
                 v = Input.touchCount;
                */
                //v = TCKInput.GetAxis("Touchpad_Y").y * 30f;
                //h = TCKInput.GetAxis("Touchpad_X").x * 2f;
                h = mH;
                v = mV;
                mH = 0f;
                mV = 0f;

            }
            

#if !MOBILE_INPUT

            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif            
        }

        private void Update()
        {
            if (textObj == null)
            {
                GameObject UIManangerObj = GameObject.Find("UIManager");
                if (!UIManangerObj.GetComponent<UIMananger>().InGameUI.activeSelf) return;

                if (GameObject.Find("txtSpeed"))
                    textObj = GameObject.Find("txtSpeed").GetComponent<TMP_Text>();
            }
            else
            {
                float fVelocity = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
                //Debug.Log("CarUsreControl.Update: velocity =" + fVelocity);
                textObj.text = ( Mathf.FloorToInt (fVelocity) * 2 ).ToString();
            }           
        }
    }    
}
