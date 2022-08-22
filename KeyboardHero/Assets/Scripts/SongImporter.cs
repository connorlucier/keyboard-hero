using AudioHelm;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using IniParser;
using System;
using UnityEngine.SceneManagement;
using Crosstales.FB;

public class SongImporter : MonoBehaviour
{
    public GameObject titleRequired;
    public GameObject artistRequired;
    public GameObject bpmRequired;
    public GameObject fileRequired;

    public InputField titleInput;
    public InputField artistInput;
    public InputField bpmInput;
    public TextMeshProUGUI selectedFile;

    private string title;
    private string artist;
    private string bpm;
    private string file;

    public void SetFile()
    {
        string newFile = FileBrowser.OpenSingleFile("*", "*", "MID", "mid");
        bool invalidFile = string.IsNullOrEmpty(newFile);

        file = newFile;
        selectedFile.text = Path.GetFileName(file);
        fileRequired.SetActive(invalidFile);
    }

    public void SetTitle(string newTitle)
    {
        bool invalidTitle = string.IsNullOrEmpty(newTitle);

        title = newTitle;
        titleRequired.SetActive(invalidTitle);
    }

    public void SetArtist(string newArtist)
    {
        bool invalidArtist = string.IsNullOrEmpty(newArtist);

        artist = newArtist;
        artistRequired.SetActive(invalidArtist);
    }

    public void SetBPM(string newBpm)
    {
        bool invalidBpm = string.IsNullOrEmpty(newBpm);
        int.TryParse(newBpm, out int bpmValue);

        bpm = newBpm;
        bpmRequired.SetActive(invalidBpm || bpmValue <= 0);
    }

    public void ClearAllFields()
    {
        titleRequired.SetActive(false);
        artistRequired.SetActive(false);
        bpmRequired.SetActive(false);
        fileRequired.SetActive(false);
        
        selectedFile.text = "";
        bpmInput.text = "";
        artistInput.text = "";
        titleInput.text = "";

        title = "";
        artist = "";
        bpm = "";
        file = "";
    }

    public void ImportSong()
    {
        if (AnyInvalidFields())
            return;

        string songsDirectory = Application.dataPath + "/Songs";
        if (!Directory.Exists(songsDirectory))
            Directory.CreateDirectory(songsDirectory);

        string songFolder = songsDirectory + "/" + Path.GetFileNameWithoutExtension(file);
        string destFile = songFolder + "/" + Path.GetFileName(file);


        if (!Directory.Exists(songFolder))
            Directory.CreateDirectory(songFolder);

        File.Copy(file, destFile, true);
        string iniFile = songFolder + "/song.ini";

        if (!File.Exists(iniFile))
            File.Create(iniFile).Dispose();

        FileIniDataParser parser = new FileIniDataParser();
        var data = parser.ReadFile(iniFile);

        MidiFile midiFile = gameObject.AddComponent<MidiFile>();
        midiFile.LoadMidiData(file);

        double songSeconds = Math.Ceiling(60.0 * midiFile.midiData.length / (4.0 * int.Parse(bpm)));
        TimeSpan songDuration = TimeSpan.FromSeconds(songSeconds);

        data["song"]["name"] = title;
        data["song"]["artist"] = artist;
        data["song"]["bpm"] = bpm;
        data["song"]["length"] = songDuration.ToString(@"m\:ss");

        data["song"]["isAdjusted"] = "false";
        data["song"]["adjustment"] = "0";

        data["score"]["highscore"] = "0";
        data["score"]["stars"] = "three";

        parser.WriteFile(iniFile, data);
        ClearAllFields();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private bool AnyInvalidFields()
    {
        bool result = false;

        if (string.IsNullOrEmpty(title))
        {
            result = true;
            titleRequired.SetActive(true);
        }

        if (string.IsNullOrEmpty(artist))
        {
            result = true;
            artistRequired.SetActive(true);
        }

        if (string.IsNullOrEmpty(bpm))
        {
            result = true;
            bpmRequired.SetActive(true);
        }

        if (string.IsNullOrEmpty(file))
        {
            result = true;
            fileRequired.SetActive(true);
        }

        return result;
    }
}
