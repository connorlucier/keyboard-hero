using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using AudioHelm;
using IniParser;

public class MidiSongController : MonoBehaviour {

    private HelmSequencer helmSequencer;
    private AudioHelmClock helmClock;
    private List<MidiNoteController> noteControllers;
    private MidiFile midiFile;

	// Use this for initialization
	void Start () {

        var songFile = PlayerPrefs.GetString("song");
        var songDir = songFile.Remove(songFile.LastIndexOf('\\'));
        var parser = new FileIniDataParser();
        var songData = parser.ReadFile(songDir + "/song.ini");

        midiFile = gameObject.AddComponent<MidiFile>();
        midiFile.LoadMidiData(songFile);

        noteControllers = FindObjectsOfType<MidiNoteController>().ToList();
        
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

        if (midiFile.midiData.notes.Count > 0)
        {
            var next = midiFile.midiData.notes.First();
            var controller = noteControllers.Where(x => x.note == next.note).FirstOrDefault();

            if (controller != null && helmSequencer.currentIndex >= next.start - 3)
            {
                controller.SpawnNote(next);
                midiFile.midiData.notes.RemoveAt(0);
            }
        }

        else if (helmSequencer.currentIndex >= helmSequencer.length - 1)
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
