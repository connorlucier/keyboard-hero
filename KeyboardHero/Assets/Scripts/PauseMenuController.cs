using AudioHelm;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour {

    public GameObject pauseMenuUI;
    public AudioHelmClock clock;

    private bool gamePaused = false;

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePaused)
                Resume();
            else
                Pause();
        }
	}

    public void Pause()
    {
        gamePaused = true;
        Time.timeScale = 0;
        clock.pause = true;
        pauseMenuUI.SetActive(true);
        pauseMenuUI.transform.SetAsLastSibling();
    }

    public void Resume()
    {
        gamePaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        clock.pause = false;
        pauseMenuUI.transform.SetAsFirstSibling();
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
