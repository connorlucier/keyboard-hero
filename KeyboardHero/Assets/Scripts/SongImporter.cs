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
    public GameObject BPMRequired;
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
        //file = EditorUtility.OpenFilePanel("Choose Song File", Application.dataPath, "MID");
        file = FileBrowser.OpenSingleFile("MID");
        selectedFile.text = Path.GetFileName(file);

        if (!string.IsNullOrEmpty(file))
            fileRequired.SetActive(false);
    }

    public void SetTitle(string newTitle)
    {
        title = newTitle;
        if (!string.IsNullOrEmpty(title))
            titleRequired.SetActive(false);
    }

    public void SetArtist(string newArtist)
    {
        artist = newArtist;
        if (!string.IsNullOrEmpty(artist))
            artistRequired.SetActive(false);
    }

    public void SetBPM(string newBpm)
    {
        if (string.IsNullOrEmpty(newBpm)) return;
        if (int.Parse(newBpm) > 0)
        {
            bpm = newBpm;
            if (!string.IsNullOrEmpty(bpm))
                BPMRequired.SetActive(false);
        }
        else
            Debug.LogWarning("BPM was set to a nonpositive value.");
    }

    public void OpenFile(string pathToFile)
    {
        file = pathToFile;
        selectedFile.text = Path.GetFileName(pathToFile);
    }

    public void ClearAllFields()
    {
        titleRequired.SetActive(false);
        artistRequired.SetActive(false);
        BPMRequired.SetActive(false);
        fileRequired.SetActive(false);
        selectedFile.text = "";

        bpmInput.Select();
        bpmInput.text = "";
        artistInput.Select();
        artistInput.text = "";
        titleInput.Select();
        titleInput.text = "";

        title = "";
        artist = "";
        bpm = "";
        file = "";
    }

    public void ImportSong()
    {
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(artist) || string.IsNullOrEmpty(bpm) || string.IsNullOrEmpty(file))
        {
            if (string.IsNullOrEmpty(title)) titleRequired.SetActive(true);
            if (string.IsNullOrEmpty(artist)) artistRequired.SetActive(true);
            if (string.IsNullOrEmpty(bpm)) BPMRequired.SetActive(true);
            if (string.IsNullOrEmpty(file)) fileRequired.SetActive(true);
            return;
        }

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
}
