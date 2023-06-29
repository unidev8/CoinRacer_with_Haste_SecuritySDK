using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public Toggle tglSound;
    // Start is called before the first frame update
    void Start()
    {
        UIMananger UImanagerObj = GameObject.FindWithTag("UIManager").GetComponent<UIMananger>();
        if (UImanagerObj.isSound)
        {
            tglSound.isOn = true;            
        }            
        else
            tglSound.isOn = false;
        UImanagerObj.SoundToggle(tglSound);

    }

    // Update is called once per frame
    void Update()
    {
         
    }
}
