using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    void Start()
    {
        if (PlayerPrefs.GetInt("firstRun", 0) == 0)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("firstRun", 1);
        }

        if (PlayerPrefs.GetString("songsDirectory", "") == "")
        {
            string defaultSongsDir = Application.dataPath + "/Songs";
            if (!Directory.Exists(defaultSongsDir))
            {
                Directory.CreateDirectory(defaultSongsDir);
            }

            PlayerPrefs.SetString("songsDirectory", defaultSongsDir);
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
