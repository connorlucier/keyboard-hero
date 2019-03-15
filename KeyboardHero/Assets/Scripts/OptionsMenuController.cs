using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour {

    public GameObject masterVolume;
    public GameObject noteSpeed;
    public GameObject noteColors;
    public GameObject keyColors;

    public List<AudioMixer> audioMixers;

    void Start()
    {
        var masterVolumeSlider = masterVolume.GetComponent<Slider>();
        var noteSpeedSlider = noteSpeed.GetComponent<Slider>();
        var noteColorsDropdown = noteColors.GetComponent<Dropdown>();
        var keyColorsDropdown = keyColors.GetComponent<Dropdown>();

        masterVolumeSlider.value = PlayerPrefs.GetFloat("masterVolume", 0.0f);
        noteSpeedSlider.value = PlayerPrefs.GetFloat("noteSpeed", 2.0f);
        noteColorsDropdown.value = PlayerPrefs.GetInt("noteColor", 0);
        keyColorsDropdown.value = PlayerPrefs.GetInt("multicolorKeys", 0);

        if (PlayerPrefs.GetInt("optionsInitialized") == 0)
        {
            Debug.Log("Options not set, initializing to default values.");
            SetMasterVolume(masterVolumeSlider.value);
            SetNoteSpeed(noteSpeedSlider.value);
            SetNoteColors(noteColorsDropdown.value);
            SetKeyColors(keyColorsDropdown.value);
            PlayerPrefs.SetInt("optionsInitialized", 1);
        }
    }

    public void SetMasterVolume(float vol)
    {
        foreach (var mixer in audioMixers)
        {
            mixer.SetFloat("volume", vol);
        }

        PlayerPrefs.SetFloat("masterVolume", vol);
    }

    public void SetNoteSpeed(float val)
    {
        PlayerPrefs.SetFloat("noteSpeed", val);
    }

    public void SetNoteColors(int option)
    {
        if (option == 0)
        {
            PlayerPrefs.SetInt("multicolorNotes", 1);
            PlayerPrefs.SetInt("noteColor", 0);
        }
        else
        {
            PlayerPrefs.SetInt("multicolorNotes", 0);
            PlayerPrefs.SetInt("noteColor", option);
        }
    }

    public void SetKeyColors(int option)
    {
        PlayerPrefs.SetInt("multicolorKeys", option);
    }
}
