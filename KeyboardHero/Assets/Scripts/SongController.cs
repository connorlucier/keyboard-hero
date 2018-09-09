using UnityEngine;
using System;
using System.Collections.Generic;

public class SongController : MonoBehaviour {

    [SerializeField]
    TextAsset songFile;

    [SerializeField]
    float noteSpawnTime;

    private string[] notes;

    private List<NoteController> noteControllers;

    private bool songComplete;

    private int lineNumber;

    private float timer;

	void Start ()
    {
        noteControllers = new List<NoteController>();
        songComplete = false;
        lineNumber = 0;
        timer = 0;

        foreach (var noteHead in GameObject.FindGameObjectsWithTag("Notehead"))
        {
            noteControllers.Add(noteHead.GetComponent<NoteController>());
        }

        notes = songFile.text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
    }

    private void Update()
    {
        PlaySong();
    }

    private void PlaySong()
    {
        if (!songComplete)
        {
            if (lineNumber < notes.Length && timer > noteSpawnTime)
            {
                PlayNextSongLine();
                timer = 0;
            }
            else if (!songComplete)
            {
                timer += Time.deltaTime;
            }
            else
            {
                SongComplete();
            }
        }
    }

    private void SongComplete()
    {
        songComplete = true;

        // TODO return to the song selection menu
    }

    private void PlayNextSongLine()
    {
        foreach (var c in notes[lineNumber])
        {
            foreach (var controller in noteControllers)
            {
                if (controller.GetKeybind() == c.ToString())
                {
                    controller.SpawnNote();
                }
            }
        }

        lineNumber++;
    }
}
