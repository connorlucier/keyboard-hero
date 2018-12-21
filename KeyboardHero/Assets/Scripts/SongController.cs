using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Collections;

public class SongController : MonoBehaviour {

    [SerializeField]
    TextAsset songFile;

    [SerializeField]
    float noteSpawnTime;

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
        PlaySong();
    }

    private void PlaySong()
    {
        if (lineNumber < notes.Length)
        {
            if (timer > noteSpawnTime)
            {
                PlayNextSongLine();
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }

        else {
            SongComplete();
        }
    }

    private void SongComplete()
    {
        // TODO create text to alert player that song is over
        StartCoroutine(ReturnToMenu());
    }

    private IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
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
