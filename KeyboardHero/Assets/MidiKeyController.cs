using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MidiJack;

public class MidiKeyController : MonoBehaviour {

    [Range(21, 108)]
    public int note = 60;

    public bool isWhiteKey;

    private Material defaultMaterial;
    private Material pressedMaterial;

	// Use this for initialization
	void Start()
    {
        SetKeyMaterials();
    }

    // Update is called once per frame
    void Update()
    {
        if (MidiMaster.GetKey(note) > 0.0f)
        {
            gameObject.GetComponent<Renderer>().material = pressedMaterial;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material = defaultMaterial;
        }
	}

    private void SetKeyMaterials()
    {
        List<Material> materials = Resources.LoadAll<Material>("Materials/Note Colors/").ToList();
        Material mat = gameObject.GetComponent<Renderer>().material;

        if (PlayerPrefs.GetInt("multicolorKeys") == 1)
        {
            switch (note % 12)
            {
                case 0: // C
                    mat = materials.Where(m => m.name == "Red").FirstOrDefault();
                    break;
                case 1: // C# / Db
                    mat = materials.Where(m => m.name == "Red Orange").FirstOrDefault();
                    break;
                case 2: // D
                    mat = materials.Where(m => m.name == "Orange").FirstOrDefault();
                    break;
                case 3: // D# / Eb
                    mat = materials.Where(m => m.name == "Gold").FirstOrDefault();
                    break;
                case 4: // E
                    mat = materials.Where(m => m.name == "Yellow").FirstOrDefault();
                    break;
                case 5: // F
                    mat = materials.Where(m => m.name == "Light Green").FirstOrDefault();
                    break;
                case 6: // F# / Gb
                    mat = materials.Where(m => m.name == "Green").FirstOrDefault();
                    break;
                case 7: // G
                    mat = materials.Where(m => m.name == "Light Blue").FirstOrDefault();
                    break;
                case 8: // G# / Ab
                    mat = materials.Where(m => m.name == "Blue").FirstOrDefault();
                    break;
                case 9: // A
                    mat = materials.Where(m => m.name == "Indigo").FirstOrDefault();
                    break;
                case 10: // A# / Bb
                    mat = materials.Where(m => m.name == "Purple").FirstOrDefault();
                    break;
                case 11: // B
                    mat = materials.Where(m => m.name == "Pink").FirstOrDefault();
                    break;
                default:
                    break;
            }

            defaultMaterial = mat;
        }

        else
        {
            switch (isWhiteKey)
            {
                case true:
                    GetComponent<Renderer>().material = materials.Where(m => m.name == "White").FirstOrDefault();
                    break;
                case false:
                    GetComponent<Renderer>().material = materials.Where(m => m.name == "Black").FirstOrDefault();
                    break;
            }

            defaultMaterial = mat;
        }

        pressedMaterial = materials.Where(x => x.name == "Grey").FirstOrDefault();
        gameObject.GetComponent<Renderer>().material = defaultMaterial;
    }
}
