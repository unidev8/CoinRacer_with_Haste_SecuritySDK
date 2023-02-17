using Mirror;
using Mirror.Examples.MultipleAdditiveScenes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//this script is used at the start of the race to show a 3, 2, 1, Go countdown
public class Countdown : MonoBehaviour
{
    [SerializeField] private GameObject CountDown, LapTimer, CarControls, FinishCamera;//race objects that we will use in the coroutine
    [SerializeField] private AudioClip GetReady, GoAudio;//same with audio objects

    void Start()
    {
        StartCoroutine(CountStart());//when we hit play in the first menu, this scripts activates and start the countdown coroutine
    }
       
    IEnumerator CountStart()
    {
        Debug.Log("Countdown.CountStart!");

        //Sets the race time to 0 when the race starts
        LapTimeManager.MinuteCount = 0;       LapTimeManager.SecondCount = 0;       LapTimeManager.MilliCount = 0;
        //3, 2, 1 Countdown
        yield return new WaitForSeconds(0.5f);
        CountDown.GetComponent<Text>().text = "3";//we start with the 3
        if (GetReady != null)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = GetReady;
            source.Play();
        }        
        CountDown.SetActive(true);//and activate it in the game UI
        yield return new WaitForSeconds(1);//after one second
        CountDown.SetActive(false);//turn off the UI
        CountDown.GetComponent<Text>().text = "2";//changes to 2
        if (GetReady != null)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = GetReady;
            source.Play();
        }
        CountDown.SetActive(true);//and activate it in the game UI
        yield return new WaitForSeconds(1);//same process for the number 1 after another second
        CountDown.SetActive(false);
        CountDown.GetComponent<Text>().text = "1";
        if (GetReady != null)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = GetReady;
            source.Play();
        }
        CountDown.SetActive(true);
        //and now that 3 seconds have passed, it's time to start the race
        yield return new WaitForSeconds(1);
        CountDown.SetActive(false);//turn of the countdown UI numbers
        if (GoAudio != null)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = GoAudio;
            source.Play();
        }

        //LapTimer.SetActive(true);//make the race time start running (LapTimeManager.cs script)

        CarControls.SetActive(true);//and activate player and AI bots cars controls (CarControlActive.cs script)        

        PlayerScore PlayerScore = FindObjectOfType(typeof(PlayerScore)) as PlayerScore;
        PlayerScore.CmdSetSync_hasStart (true); //server side
        //PlayerScore.startTime = NetworkTime.time;  //client side

        FinishCamera.SetActive(false);
        Debug.Log("Countdown.CountStart: Countodown is ended and hasStarted is TRUE !");
    }

}