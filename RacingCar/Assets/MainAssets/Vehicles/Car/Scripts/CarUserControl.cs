using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using TMPro;
using TouchControlsKit;
using static UnityEngine.AudioSettings;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        //[SerializeField]
        private TMP_Text textObj; 

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }
        
        private void FixedUpdate()
        {
            float h, v, v_mobile, h_mobile;
            
            //if (SystemInfo.deviceType == DeviceType.Desktop )
            {
                h = CrossPlatformInputManager.GetAxis("Horizontal");
                v = CrossPlatformInputManager.GetAxis("Vertical");
            }
            //else
            {
                v_mobile = TCKInput.GetAxis("Touchpad_Y").y * 30f;
                h_mobile = TCKInput.GetAxis("Touchpad_X").x * 2f;
            }

            if (v_mobile != 0f || h_mobile != 0f)
            {
                h = h_mobile;
                v = v_mobile;
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
