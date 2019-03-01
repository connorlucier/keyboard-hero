using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour {

    public GameObject noteSpeed;
    public GameObject noteColors;
    public GameObject keyColors;

    void Start()
    {
        var noteSpeedSlider = noteSpeed.GetComponent<Slider>();
        var noteColorsDropdown = noteColors.GetComponent<Dropdown>();
        var keyColorsDropdown = keyColors.GetComponent<Dropdown>();

        noteSpeedSlider.value = PlayerPrefs.GetFloat("noteSpeed", 2.0f);
        noteColorsDropdown.value = PlayerPrefs.GetInt("noteColor", 0);
        keyColorsDropdown.value = PlayerPrefs.GetInt("multicolorKeys", 0);

        if (PlayerPrefs.GetInt("firstRun") == 1)
        {
            SetNoteSpeed(noteSpeedSlider.value);
            SetNoteColors(noteColorsDropdown.value);
            SetKeyColors(keyColorsDropdown.value);
        }
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
