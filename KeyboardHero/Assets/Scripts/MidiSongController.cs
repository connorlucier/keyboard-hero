using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using AudioHelm;
using IniParser;
using IniParser.Model;
using System.IO;

public class MidiSongController : MonoBehaviour {

    [Range(1,3)]
    public float noteSpawnHeight = 2.5f;

    public AudioHelmClock clock;
    public MidiStatsController statsController;
    public List<HelmSequencer> sequencers;

    private List<MidiNoteController> noteControllers;
    private List<Note> notes;
    private MidiFile midiFile;

    private int adjustment;
    private int songDelay;

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

        adjustment = FindSmallestSubdivision();

        clock.bpm = float.Parse(songData["song"]["bpm"]) * adjustment;
        PlayerPrefs.SetFloat("bpm", clock.bpm);

        songDelay = Mathf.RoundToInt(noteSpawnHeight / PlayerPrefs.GetFloat("noteSpeed", 3.25f) * (4 * clock.bpm / 60));

        AdjustMidiFile();

        noteControllers = FindObjectsOfType<MidiNoteController>().ToList();

        clock.pause = true;
        SetUpSequencers();

        notes = sequencers[0].GetAllNotes();

        statsController.scoreDensity /= adjustment;
        StartCoroutine(StartSong());
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("practiceMode") == 1)
        {
            songDelay = Mathf.RoundToInt(noteSpawnHeight / PlayerPrefs.GetFloat("noteSpeed", 3.25f) * (4 * clock.bpm / 60));
        }

        if (notes.Count > 0)
        {
            var sequencer = sequencers[0];
            var nextNotesToSpawn = notes.Where(n => sequencer.currentIndex + songDelay > n.start).ToList();
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

    private int FindSmallestSubdivision()
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

    private void AdjustMidiFile()
    {
        foreach (var note in midiFile.midiData.notes)
        {
            note.start = note.start * adjustment + songDelay;
            note.end = note.end * adjustment + songDelay;
        }

        songData["song"]["isAdjusted"] = "true";
        songData["song"]["adjustment"] = adjustment.ToString();

        parser.WriteFile(iniFile, songData);
    }

    private void SetUpSequencers()
    {
        foreach (var seq in sequencers)
        {
            seq.length = midiFile.midiData.length + songDelay;
            seq.currentIndex = 0;
        }

        foreach (var note in midiFile.midiData.notes)
        {
            if (note.note < 36)
            {
                sequencers[1].AddNote(note.note, note.start, note.end, note.velocity);
            }
            else if (note.note < 48)
            {
                sequencers[2].AddNote(note.note, note.start, note.end, note.velocity);
            }
            else if (note.note < 60)
            {
                sequencers[3].AddNote(note.note, note.start, note.end, note.velocity);
            }
            else if (note.note < 72)
            {
                sequencers[4].AddNote(note.note, note.start, note.end, note.velocity);
            }
            else if (note.note < 84)
            {
                sequencers[5].AddNote(note.note, note.start, note.end, note.velocity);
            }
            else if (note.note < 96)
            {
                sequencers[6].AddNote(note.note, note.start, note.end, note.velocity);
            }
            else if (note.note <= 108)
            {
                sequencers[7].AddNote(note.note, note.start, note.end, note.velocity);
            }
        }

        sequencers[0].ReadMidiFile(midiFile);
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

    private IEnumerator StartSong()
    {
        yield return new WaitForSeconds(1);
        clock.pause = false;
    }

    private IEnumerator ReturnToMenu()
    {
        PlayerPrefs.DeleteKey("bpm");
        SaveSongStats();
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
