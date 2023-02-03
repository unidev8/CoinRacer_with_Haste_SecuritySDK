using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayPeriod : MonoBehaviour
{
    public float displayPeriod = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (displayPeriod > 0)
        {
            displayPeriod -= Time.deltaTime;
        }
        else
            GetComponent<TMP_Text>().text = "0";
    }
}
