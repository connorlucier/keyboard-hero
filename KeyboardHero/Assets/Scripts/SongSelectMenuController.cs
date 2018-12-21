using IniParser;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SongSelectMenuController : MonoBehaviour {

    [SerializeField]
    int spacing;

    void Start()
    {
        var songs = Directory.GetDirectories("Assets/Resources/Songs").ToList();
        var songPrefab = Resources.Load<GameObject>("UI/Song");
        var menuObject = GameObject.Find("Song Select Menu");

        int i = 0;
        foreach (var songDir in songs)
        {
            // Create an entry for every song
            var song = Instantiate(songPrefab);
            song.transform.SetParent(menuObject.transform, false);
            song.transform.position += i++ * Vector3.down * spacing;

            PopulateFields(song, songDir);

            // Play selected song on click
            var fileName = Directory.GetFiles(songDir).Where(x => x.EndsWith(".txt")).FirstOrDefault();
            fileName = fileName.Substring(fileName.IndexOf("Songs"));
            fileName = fileName.Remove(fileName.IndexOf(".txt"));
            song.GetComponent<Button>().onClick.AddListener(delegate () { PlaySong(fileName); });
        }
    }

    public void PlaySong(string fileName)
    {
        PlayerPrefs.SetString("song", fileName);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GoBack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    private static void PopulateFields(GameObject song, string songDir)
    {
        var parser = new FileIniDataParser();
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
                    // TODO stored locally after song completion, not in this .ini file
                    field.GetComponent<TextMeshProUGUI>().text = songData["score"]["highscore"];
                    break;

                case "Stars":
                    field.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + songData["score"]["stars"] + "_star");
                    break;

                default:
                    break;
            }
        }
    }
}
