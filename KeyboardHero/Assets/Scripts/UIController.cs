using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject standardUI;
    public GameObject songCompleteUI;

    public Slider progressSlider;
    public TextMeshProUGUI title;

    public MidiSongController songController;
    public MidiStatsController statsController;

    private float bpm;
    private bool gamePaused;
    private bool songComplete;

    void Start()
    {
        if (PlayerPrefs.GetInt("practiceMode") == 1)
        {
            statsController.scoreDensity = 0;
            progressSlider.gameObject.SetActive(false);
        }

        bpm = songController.clock.bpm;
        title.text = PlayerPrefs.GetString("songName");
    }
    
    void Update()
    {
        if (songComplete) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePaused) Resume();
            else Pause();
        }

        UpdateProgress();
    }

    public void Pause()
    {
        gamePaused = true;
        Time.timeScale = 0;
        songController.clock.pause = true;
        pauseMenuUI.SetActive(true);
        pauseMenuUI.transform.SetAsLastSibling();
    }

    public void Resume()
    {
        gamePaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        songController.clock.pause = false;
        pauseMenuUI.transform.SetAsFirstSibling();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        Time.timeScale = 1;
    }

    public void ChangeSongBpm(float multiplier)
    {
        songController.clock.bpm = bpm * multiplier;
    }

    private void UpdateProgress()
    {
        float progress = (float) songController.sequencers[0].currentIndex / songController.sequencers[0].length;
        progressSlider.value = Mathf.Clamp(progress, 0, 1);
    }

    public void EndSong()
    {
        songComplete = true;
        songController.clock.pause = true;
        statsController.SetFinalStats();
        StartCoroutine("FinishSong");
    }

    private IEnumerator FinishSong()
    {
        yield return new WaitForSeconds(1);
        songCompleteUI.SetActive(true);
        songCompleteUI.transform.SetAsLastSibling();
    }
}
