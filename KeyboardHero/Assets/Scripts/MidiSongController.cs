using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using AudioHelm;
using IniParser;
using IniParser.Model;
using System;

public class MidiSongController : MonoBehaviour {

    [Range(1,3)]
    public float noteSpawnHeight = 2.5f;

    [Range(0,2)]
    public float latency = 0.5f;

    public AudioHelmClock helmClock;
    public HelmSequencer helmSequencer;

    private List<MidiNoteController> noteControllers;
    private MidiFile midiFile;

    private float songDelay;

    private string iniFile;
    private FileIniDataParser parser;
    private IniData songData;

	void Start()
    {
        var songFile = PlayerPrefs.GetString("song");
        var songDir = songFile.Remove(songFile.LastIndexOf('\\'));

        parser = new FileIniDataParser();
        iniFile = songDir + "/song.ini";
        songData = parser.ReadFile(iniFile);

        midiFile = gameObject.AddComponent<MidiFile>();
        midiFile.LoadMidiData(songFile);

        noteControllers = FindObjectsOfType<MidiNoteController>().ToList();

        helmSequencer.Clear();
        helmSequencer.ReadMidiFile(midiFile);
        helmSequencer.currentIndex = 0;

        helmClock.bpm = float.Parse(songData["song"]["bpm"]);
        songDelay = noteSpawnHeight * (4 * helmClock.bpm / 60) / PlayerPrefs.GetFloat("noteSpeed");

        foreach (var n in helmSequencer.GetAllNotes())
        {
            n.start += songDelay * 2 - latency;
            n.end += songDelay * 2 - latency;
        }

        helmClock.pause = false;
    }

    void Update()
    {
        if (midiFile.midiData.notes.Count > 0)
        {
            var nextNotesToSpawn = midiFile.midiData.notes.Where(n => helmSequencer.currentIndex >= n.start + songDelay).ToList();
            foreach (var next in nextNotesToSpawn)
            {
                SpawnNote(next);
                midiFile.midiData.notes.Remove(next);
            }
        }

        else if (helmSequencer.currentIndex >= helmSequencer.length - 1)
        {
            StartCoroutine(ReturnToMenu());
        }
	}

    private void SpawnNote(Note next)
    {
        var controller = noteControllers.Where(x => x.Note() == next.note).FirstOrDefault();

        if (controller != null)
        {
            controller.CreateNote(next, noteSpawnHeight);
        }
    }

    private void SaveSongStats()
    {
        var statsController = GameObject.FindGameObjectWithTag("Stats Controller").GetComponent<MidiStatsController>();
        var highScore = long.Parse(songData["score"]["highscore"]);

        if (statsController.Score() > highScore)
        {
            songData["score"]["highscore"] = statsController.Score().ToString();

            var accuracy = statsController.Accuracy();

            if (accuracy < 75.0)
                songData["score"]["stars"] = "three";
            else if (accuracy < 90.0)
                songData["score"]["stars"] = "four";
            else
                songData["score"]["stars"] = "five";

            parser.WriteFile(iniFile, songData);
        }
    }

    private IEnumerator ReturnToMenu()
    {
        SaveSongStats();
        yield return new WaitForSeconds(6);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
