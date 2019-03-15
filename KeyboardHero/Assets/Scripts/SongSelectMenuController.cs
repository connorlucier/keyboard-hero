﻿using IniParser;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SongSelectMenuController : MonoBehaviour {

    public GameObject listArea;

    [Range(100,200)]
    public int spacing = 100;

    void Start()
    {
        SetSongsDirectory();
        var songsDirectory = PlayerPrefs.GetString("songsDirectory");
        var songDirs = Directory.GetDirectories(songsDirectory).ToList();
        var songPrefab = Resources.Load<GameObject>("UI/Song");

        foreach (var songDir in songDirs)
        {
            var song = Instantiate(songPrefab);
            song.transform.SetParent(listArea.transform, false);
            PopulateFields(song, songDir);
            song.GetComponent<Button>().onClick.AddListener(delegate () { PlaySong(songDir); });
        }
    }

    public void PlaySong(string songDir)
    {
        PlayerPrefs.SetString("songDir", songDir);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);          
    }

    public void GoBack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    private static void SetSongsDirectory()
    {
        if (PlayerPrefs.GetString("songsDirectory", "") == "")
        {
            Debug.Log("Songs directory not set.");
            string defaultSongsDir = Application.dataPath + "/Songs";
            if (!Directory.Exists(defaultSongsDir))
            {
                Debug.Log("Songs directory not found. Creating default at " + defaultSongsDir);
                Directory.CreateDirectory(defaultSongsDir);
            }

            PlayerPrefs.SetString("songsDirectory", defaultSongsDir);
        }
    }

    private static void PopulateFields(GameObject song, string songDir)
    {
        var parser = new FileIniDataParser();

        if (!File.Exists(songDir + "/song.ini"))
        {
            File.Create(songDir + "/song.ini");
        }

        var songData = parser.ReadFile(songDir + "/song.ini");

        for (int i = 0; i < song.transform.childCount; i++)
        {
            var field = song.transform.GetChild(i);
            switch (field.name)
            {
                case "Title":
                    field.GetComponent<TextMeshProUGUI>().text = songData["song"]["name"];
                    break;

                case "Artist":
                    field.GetComponent<TextMeshProUGUI>().text = songData["song"]["artist"];
                    break;

                case "Length":
                    field.GetComponent<TextMeshProUGUI>().text = songData["song"]["length"];
                    break;

                case "Score":
                    field.GetComponent<TextMeshProUGUI>().text = songData["score"]["highscore"];
                    break;

                case "Stars":
                    var img = field.GetComponent<Image>();
                    var color = img.color;
                    color.a = 1.0f;

                    if (int.Parse(songData["score"]["highscore"]) > 0) {
                        img.sprite = Resources.Load<Sprite>("Images/" + songData["score"]["stars"] + "_star");
                        img.color = color;
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
