using Mirror;
using Mirror.Examples.MultipleAdditiveScenes;
using UnityEngine;
using UnityEngine.SceneManagement;
//this script is used in the Play button from the first menu and the Continue button when you finish the race
public class Continue : MonoBehaviour
{
    [SerializeField] private GameObject Countdown, CarControlActive, FinishCamera;//, RaceUI, LapsBotsPanel, LapsSelected, BotsSelected;
    //also, it is used if we hit restart in the pause menu
    public void Restart()
    {   //void restart is used in continue buttons when we finish the race and in return button of the pause menu
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);//so it will restart the game's scene
    }
    //if we hit play in the menu at the start of the scene we will use the Play void:

    private void Start()
    {
        //CarControlActive.SetActive(false);
    }
    public void Play()
    {
        Debug.Log("Contune.Play: Client ");
        GameObject.FindWithTag("Player").GetComponent <Rigidbody>().useGravity = true; // Sometime Player is fall if is generate before ground is generated.

        //all the racing stuff turns on         
        Countdown.SetActive(true);  //countdown UI (3,2,1,go)
        //FinishCamera.SetActive(false); //and the camera goes off too, to use the one in the player car    

    }
}
