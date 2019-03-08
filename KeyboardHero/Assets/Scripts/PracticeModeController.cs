using AudioHelm;
using UnityEngine;

public class PracticeModeController : MonoBehaviour {

    public GameObject practiceModeUI;
    public GameObject stats;
    public AudioHelmClock clock;

	void Start () {
        if (PlayerPrefs.GetInt("practiceMode") == 1)
        {
            practiceModeUI.SetActive(true);
            stats.SetActive(false);
        }
        else
        {
            practiceModeUI.SetActive(false);
            stats.SetActive(true);
        }
	}

    public void ChangeSongBpm(float multiplier)
    {
        clock.bpm = PlayerPrefs.GetFloat("bpm") * multiplier;
    }
}
