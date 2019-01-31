using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using AudioHelm;
using IniParser;

public class MidiSongController : MonoBehaviour {

    [Range(1,3)]
    public float noteSpawnHeight = 2.5f; 

    public AudioHelmClock helmClock;
    public HelmSequencer helmSequencer;

    private List<MidiNoteController> noteControllers;
    private MidiFile midiFile;

    private float songDelay;

	void Start () {

        var songFile = PlayerPrefs.GetString("song");
        var songDir = songFile.Remove(songFile.LastIndexOf('\\'));
        var parser = new FileIniDataParser();
        var songData = parser.ReadFile(songDir + "/song.ini");

        midiFile = gameObject.AddComponent<MidiFile>();
        midiFile.LoadMidiData(songFile);
        helmSequencer.Clear();
        helmSequencer.ReadMidiFile(midiFile);
        helmSequencer.currentIndex = 0;

        helmClock.bpm = float.Parse(songData["song"]["bpm"]);
        songDelay = noteSpawnHeight * (4 * helmClock.bpm / 60) / PlayerPrefs.GetFloat("noteSpeed");

        noteControllers = FindObjectsOfType<MidiNoteController>().ToList();
        foreach (var nc in noteControllers)
        {
            nc.SetSpawnHeight(noteSpawnHeight);
        }

        foreach (var n in helmSequencer.GetAllNotes())
        {
            n.start += songDelay * 2;
            n.end += songDelay * 2;
        }

        helmClock.pause = false;
    }

    void Update () {
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
        var controller = noteControllers.Where(x => x.note == next.note).FirstOrDefault();

        if (controller != null)
        {
            controller.CreateNote(next);
        }
    }

    private IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(6);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }
}
