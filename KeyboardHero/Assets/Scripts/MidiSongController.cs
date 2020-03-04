using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AudioHelm;
using IniParser;
using IniParser.Model;
using System.IO;

public class MidiSongController : MonoBehaviour {

    [Range(1, 3)]
    public float noteSpawnHeight = 2.5f;

    public AudioHelmClock clock;
    public MidiStatsController statsController;
    public UIController uIController;
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
        clock.pause = true;
        notes = new List<Note>();

        string songDir = PlayerPrefs.GetString("songDir");
        iniFile = songDir + "/song.ini";

        parser = new FileIniDataParser();
        songData = parser.ReadFile(iniFile);

        string songFile = Directory.GetFiles(songDir).Where(x => x.EndsWith(".mid") || x.EndsWith(".MID")).FirstOrDefault();

        midiFile = gameObject.AddComponent<MidiFile>();
        midiFile.LoadMidiData(songFile);

        adjustment = FindSmallestSubdivision();
        clock.bpm = float.Parse(songData["song"]["bpm"]) * adjustment;

        if (PlayerPrefs.GetInt("practiceMode") == 1)
            clock.bpm *= PlayerPrefs.GetFloat("practiceSpeed");

        songDelay = Mathf.FloorToInt(noteSpawnHeight / PlayerPrefs.GetFloat("noteSpeed", 3.25f) * (4.0f * clock.bpm / 60.0f));

        AdjustMidiFile();
        SetUpSequencers();

        notes = sequencers[0].GetAllNotes();
        noteControllers = FindObjectsOfType<MidiNoteController>().ToList();

        statsController.scoreDensity /= adjustment;
        StartCoroutine(StartSong());
    }

    void Update()
    {
        if (!clock.pause && notes.Count > 0)
        {
            var sequencer = sequencers[0];
            var nextNotesToSpawn = notes.Where(n => sequencer.currentIndex + songDelay > n.start).ToList();
            foreach (var next in nextNotesToSpawn)
            {
                SpawnNote(next);
                notes.Remove(next);
            }
        }
        
        else if (!clock.pause && GameObject.FindGameObjectsWithTag("Note").Length == 0)
            EndSong();
	}

    private void SpawnNote(Note next)
    {
        var controller = noteControllers.Where(c => c.Note() == next.note).FirstOrDefault();
        if (controller != null)
            controller.CreateNote(next, noteSpawnHeight);
    }

    private int FindSmallestSubdivision()
    {
        if (bool.Parse(songData["song"]["isAdjusted"]))
            return int.Parse(songData["song"]["adjustment"]);

        int result = 1;
        float div = 1.0f;

        float minDuration = midiFile.midiData.notes.Select(n => n.end - n.start).Min();
        while (minDuration < div)
        {
            div /= 2.0f;
            result *= 2;
        }

        songData["song"]["isAdjusted"] = "true";
        songData["song"]["adjustment"] = result.ToString();

        parser.WriteFile(iniFile, songData);

        return result;
    }

    private void AdjustMidiFile()
    {
        int startPos = PlayerPrefs.GetInt("practiceMode", 0) == 1 ? (int)(midiFile.midiData.length * PlayerPrefs.GetFloat("practiceStartPosition", 0)) : 0;

        midiFile.midiData.notes = midiFile.midiData.notes
            .Where(n => n.start >= startPos)
            .Select(n => new Note
            {
                note = n.note,
                velocity = n.velocity,
                start = (n.start - startPos) * adjustment + songDelay,
                end = (n.end - startPos) * adjustment + songDelay
            })
            .ToList();

        midiFile.midiData.length = (midiFile.midiData.length - startPos) * adjustment + songDelay;
    }

    private void SetUpSequencers()
    {
        sequencers[0].ReadMidiFile(midiFile);

        foreach (var seq in sequencers.GetRange(1, sequencers.Count - 1))
            seq.length = sequencers[0].length;

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

    }

    private void SaveSongStats()
    {
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

    private void EndSong()
    {
        if (PlayerPrefs.GetInt("practiceMode") == 0)
            SaveSongStats();

        uIController.EndSong();
    }
}
