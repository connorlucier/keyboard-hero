using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour {

    void Start()
    {
        var noteSpeed = GameObject.FindGameObjectWithTag("Note Speed Slider");
        var noteSpeedSlider = noteSpeed.GetComponent<Slider>();

        var noteColors = GameObject.FindGameObjectWithTag("Note Colors Dropdown");
        var noteColorsDropdown = noteColors.GetComponent<Dropdown>();

        var keyColors = GameObject.FindGameObjectWithTag("Key Colors Dropdown");
        var keyColorsSelector = keyColors.GetComponent<Dropdown>();

        noteSpeedSlider.value = PlayerPrefs.GetFloat("noteSpeed", 1.0f);
        noteColorsDropdown.value = PlayerPrefs.GetInt("noteColor", 0);
        keyColorsSelector.value = PlayerPrefs.GetInt("multicolorKeys", 0);
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
