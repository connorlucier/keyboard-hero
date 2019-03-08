using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    void Start()
    {
        if (PlayerPrefs.GetInt("gameInitDone") == 0)
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Starting for the first time. Removing player prefs.");
            PlayerPrefs.SetInt("gameInitDone", 1);
        }
    }

	public void PlayGame(bool practiceModeEnabled)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        PlayerPrefs.SetInt("practiceMode", practiceModeEnabled ? 1 : 0);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
