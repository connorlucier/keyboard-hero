using AudioHelm;
using UnityEngine;

public class PracticeModeController : MonoBehaviour {

    public GameObject practiceModeUI;
    public AudioHelmClock clock;
    public MidiStatsController statsController;
    
    private float bpm;


	void Start ()
    {
        if (PlayerPrefs.GetInt("practiceMode") == 1)
        {
            practiceModeUI.SetActive(true);
            practiceModeUI.transform.SetAsLastSibling();
            statsController.scoreDensity = 0;
        }

        bpm = clock.bpm;
    }

    public void ChangeSongBpm(float multiplier)
    {
        clock.bpm = bpm * multiplier;
    }
}
