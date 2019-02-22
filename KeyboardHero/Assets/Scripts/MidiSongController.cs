using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using AudioHelm;
using IniParser;
using IniParser.Model;
using System.IO;
using System;
using UnityEditor;

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
        string songDir = PlayerPrefs.GetString("songDir");
        iniFile = songDir + "/song.ini";

        parser = new FileIniDataParser();
        songData = parser.ReadFile(iniFile);

        // TODO figure out if song has already been adjusted. If an adjusted MIDI exists, use it.
        string songFile = Directory.GetFiles(songDir).Where(x => x.EndsWith(".mid") || x.EndsWith(".MID")).FirstOrDefault();

        // TODO if not, make adjustments and save adjusted copy.

        midiFile = gameObject.AddComponent<MidiFile>();
        midiFile.LoadMidiData(songFile);

        int adjustment = FindSmallestSubdivision(midiFile);
        AdjustMidiFile(midiFile, adjustment);

        noteControllers = FindObjectsOfType<MidiNoteController>().ToList();

        helmSequencer.Clear();
        helmSequencer.ReadMidiFile(midiFile);
        helmSequencer.currentIndex = 0;

        helmClock.bpm = float.Parse(songData["song"]["bpm"]) * adjustment;
        songDelay = noteSpawnHeight * (4 * helmClock.bpm / 60) / PlayerPrefs.GetFloat("noteSpeed");

        foreach (var n in helmSequencer.GetAllNotes())
        {
            n.start += (songDelay * 2 - latency);
            n.end += (songDelay * 2 - latency);
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

    private int FindSmallestSubdivision(MidiFile midiFile)
    {
        if (bool.Parse(songData["song"]["isAdjusted"]))
            return int.Parse(songData["song"]["adjustment"]);

        int result = 1;
        float div = 1.0f;

        foreach (Note note in midiFile.midiData.notes)
        {
            float duration = note.end - note.start;

            int tmp = 1;
            while (duration < div)
            {
                div /= 2;
                tmp *= 2;
            }

            result = Math.Max(result, tmp);
        }

        return result;
    }

    private void AdjustMidiFile(MidiFile midiFile, int adjustment)
    {
        var adjustedNotes = midiFile.midiData.notes;

        foreach (var note in adjustedNotes)
        {
            note.start *= adjustment;
            note.end *= adjustment;
        }

        midiFile.midiData.notes = adjustedNotes;

        songData["song"]["isAdjusted"] = "true";
        songData["song"]["adjustment"] = adjustment.ToString();

        parser.WriteFile(iniFile, songData);

        // TODO save changes to notes...?
        //string songFile = "";
        //FileUtil.CopyFileOrDirectory(songFile, songFile.Substring(0, songFile.IndexOf(".")) + "_adjusted.MID");
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
