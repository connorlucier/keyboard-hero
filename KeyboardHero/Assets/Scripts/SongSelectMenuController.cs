using IniParser;
using IniParser.Model;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SongSelectMenuController : MonoBehaviour {

    [SerializeField]
    int spacing;

    void Start()
    {
        var songs = Directory.GetDirectories("Assets/Songs").ToList();
        var songPrefab = Resources.Load("UI/Song") as GameObject;
        var menuObject = GameObject.Find("Song Select Menu");

        int i = 0;
        foreach (var songDir in songs)
        {
            var song = Instantiate(songPrefab);
            song.transform.SetParent(menuObject.transform, false);
            song.transform.position += i * Vector3.down * spacing;
            i++;

            // TODO use the song's config file to populate text fields (and score, though scores will be stored in local appdata)
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(songDir + "/song.ini");

            if (data != null)
            {
                Debug.Log(data["song"]["name"]);
            }
        }
    }

    public void PlaySong()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GoBack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
