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

public class MidiSongController : MonoBehaviour {

    [Range(1,3)]
    public float noteSpawnHeight = 2.5f;

    public AudioHelmClock helmClock;
    public HelmSequencer helmSequencer;

    private List<MidiNoteController> noteControllers;
    private List<Note> notes;
    private MidiFile midiFile;

    private int adjustment;
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

        adjustment = FindSmallestSubdivision(midiFile);
        AdjustMidiFile(midiFile, adjustment);

        helmClock.bpm = float.Parse(songData["song"]["bpm"]) * adjustment;
        PlayerPrefs.SetFloat("bpm", helmClock.bpm);

        songDelay = Mathf.Floor(noteSpawnHeight / PlayerPrefs.GetFloat("noteSpeed", 3.25f) * (4 * helmClock.bpm / 60));

        foreach (var n in midiFile.midiData.notes)
        {
            n.start += songDelay;
            n.end += songDelay;
        }

        noteControllers = FindObjectsOfType<MidiNoteController>().ToList();

        if (noteControllers.First() != null)
            noteControllers.First().statsController.scoreDensity /= adjustment;

        helmSequencer.length += (int)songDelay;
        helmSequencer.ReadMidiFile(midiFile);
        helmSequencer.currentIndex = 0;

        notes = helmSequencer.GetAllNotes();
        helmClock.pause = false;
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("practiceMode") == 1)
        {
            songDelay = Mathf.Ceil(noteSpawnHeight / PlayerPrefs.GetFloat("noteSpeed", 3.25f) * (4 * helmClock.bpm / 60));
        }

        if (notes.Count > 0)
        {
            var nextNotesToSpawn = notes.Where(n => helmSequencer.currentIndex + songDelay >= n.start).ToList();
            foreach (var next in nextNotesToSpawn)
            {
                SpawnNote(next);
                notes.Remove(next);
            }
        }

        else if (GameObject.FindGameObjectsWithTag("Note").Length == 0)
        {
            StartCoroutine(ReturnToMenu());
        }
	}

    private void SpawnNote(Note next)
    {
        var controller = noteControllers.Where(c => c.Note() == next.note).FirstOrDefault();

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

            while (duration < div)
            {
                div /= 2;
                result *= 2;
            }
        }

        return result;
    }

    private void AdjustMidiFile(MidiFile midiFile, int adjustment)
    {
        foreach (var note in midiFile.midiData.notes)
        {
            note.start *= adjustment;
            note.end *= adjustment;
        }

        songData["song"]["isAdjusted"] = "true";
        songData["song"]["adjustment"] = adjustment.ToString();

        parser.WriteFile(iniFile, songData);
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
        PlayerPrefs.DeleteKey("bpm");
        SaveSongStats();
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
