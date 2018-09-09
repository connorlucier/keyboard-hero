using UnityEngine;
using System;
using System.Collections.Generic;

public class SongController : MonoBehaviour {

    [SerializeField]
    TextAsset songFile;

    private string[] notes;

    private List<NoteController> noteControllers;

    private int lineNumber;

    private float timer;

	void Start ()
    {
        noteControllers = new List<NoteController>();
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
        if(lineNumber < notes.Length && timer > 1)
        {
            PlayNextSongLine();
            timer = 0;
        }
        else if (lineNumber == notes.Length)
        {
            SongComplete();
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    private static void SongComplete()
    {
        print("Song over");
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
