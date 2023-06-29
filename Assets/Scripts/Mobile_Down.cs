
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
using UnityEngine.UI;

public class Mobile_Down : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent cEvent;

    private bool isPointerEntered = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        //Debug.Log("plyaerObj=" + playerObj.name + "this=" );
        //if (gameObject.name == "btn_Up")
        cEvent.AddListener(delegate { playerObj.GetComponent<CarUserControl>().Vertical_N(); });
        //else if (gameObject.name == "btn_Down")
        //cEvent.AddListener(delegate { playerObj.GetComponent<CarUserControl>().Vertical_N(); });

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            //gameObject.SetActive(false);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerEntered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerEntered = false;
    }


    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && isPointerEntered)
        {
            cEvent.Invoke();
            //Debug.Log("Button is pressed!");
        }
    }

}
