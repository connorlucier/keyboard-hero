using IniParser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SongSelectMenuController : MonoBehaviour {

    [Range(100,200)]
    public int spacing = 100;

    private Dictionary<GameObject, string> songsDict = new Dictionary<GameObject, string>();

    void Start()
    {
        var songDirs = Directory.GetDirectories("Assets/Resources/Songs").ToList();
        var songPrefab = Resources.Load<GameObject>("UI/Song");
        var menuObject = GameObject.Find("Song Select Menu");

        int i = 0;
        foreach (var songDir in songDirs)
        {
            var song = Instantiate(songPrefab);
            song.transform.SetParent(menuObject.transform, false);
            song.transform.position += i++ * Vector3.down * spacing;

            songsDict.Add(song, songDir);
            PopulateFields(song, songDir);

            
            var fileName = Directory.GetFiles(songDir).Where(x => x.EndsWith(".mid") || x.EndsWith(".MID")).FirstOrDefault();

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
