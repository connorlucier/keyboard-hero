using AudioHelm;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour {

    public static bool gamePaused = false;

    public GameObject pauseMenuUI;
    public AudioHelmClock clock;

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePaused)
                Resume();
            else
                Pause();

            gamePaused = !gamePaused;
        }
	}

    public void Pause()
    {
        Time.timeScale = 0;
        clock.pause = true;
        pauseMenuUI.SetActive(true);
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        clock.pause = false;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
