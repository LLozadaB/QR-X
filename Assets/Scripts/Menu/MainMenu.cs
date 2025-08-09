using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu; // Menu to activate when returning from a race
    public GameObject carMenu;
    private int returnFromRace;
    void Start()
    {
        returnFromRace = PlayerPrefs.GetInt("returnFromRace");
        if (returnFromRace == 1)
        {
            PlayerPrefs.SetInt("returnFromRace", 0);
            carMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
    }
    public void quitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}