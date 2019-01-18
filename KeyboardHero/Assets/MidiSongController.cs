using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AudioHelm;
using IniParser;
using System;

public class MidiSongController : MonoBehaviour {

    private HelmSequencer helmSequencer;
    private AudioHelmClock helmClock;

	// Use this for initialization
	void Start () {

        var songFile = PlayerPrefs.GetString("song");
        var songDir = songFile.Remove(songFile.LastIndexOf('\\'));
        var midiFile = gameObject.AddComponent<MidiFile>();

        var parser = new FileIniDataParser();
        var songData = parser.ReadFile(songDir + "/song.ini");

        midiFile.LoadMidiData(songFile);

        helmClock = GetComponent<AudioHelmClock>();
        helmSequencer = GetComponent<HelmSequencer>();
        helmSequencer.Clear();
        helmSequencer.ReadMidiFile(midiFile);
        helmSequencer.currentIndex = 0;

        helmClock.bpm = float.Parse(songData["song"]["bpm"]);
        helmClock.pause = false;
    }

    // Update is called once per frame
    void Update () {
        if (helmSequencer.currentIndex >= helmSequencer.length - 1)
        {
            StartCoroutine(ReturnToMenu());
        }
	}

    private IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }
}
