using UnityEngine;
using MidiJack;
using AudioHelm;
using System.Linq;
using System.Collections.Generic;

public class MidiNoteController : MonoBehaviour {

    [Range(21, 108)]
    public int note = 60;

    [Range(0, 1)]
    public float beatUnitConversion = 1.0f;

    public AudioHelmClock clock;

    public StatsController statsController;

    public bool isWhiteKey = true;

    private GameObject notePrefab;

    private List<Material> materials;

    private float spawnHeight = 2.5f;

    void Start ()
    {
        notePrefab = Resources.Load<GameObject>("Physical/Note");
        materials = Resources.LoadAll<Material>("Materials/Note Colors/").ToList();
        GetComponent<Renderer>().material.color = Color.red;
    }

    void Update ()
    {
        UpdateKeyColor();
    }

    void OnTriggerExit(Collider other)
    {
        if (int.Parse(other.tag) == note)
        {
            Destroy(other.gameObject.transform.parent.gameObject);
        }
    }

    public void CreateNote(Note n)
    {
        var noteObject = Instantiate(notePrefab);
        var cube = noteObject.transform.GetChild(0).gameObject;

        var noteDuration = 60 * (n.end - n.start) / (4 * clock.bpm);
        var noteSpeed = PlayerPrefs.GetFloat("noteSpeed", 3.25f);
        var noteScale = noteDuration * noteSpeed * beatUnitConversion;

        noteObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, noteScale, gameObject.transform.localScale.z);
        noteObject.transform.position = gameObject.transform.position + new Vector3(-0.5f * gameObject.transform.localScale.x, spawnHeight, -0.05f);
        cube.tag = note.ToString();

        SetDefaultNoteMaterial(cube, n.note);

        var noteRigidBody = noteObject.AddComponent<Rigidbody>();
        noteRigidBody.useGravity = false;
        noteRigidBody.constraints =
            RigidbodyConstraints.FreezeRotation
            | RigidbodyConstraints.FreezePositionZ
            | RigidbodyConstraints.FreezePositionX;

        noteRigidBody.AddForce(Vector3.down * noteSpeed, ForceMode.VelocityChange);
    }

    public void SetSpawnHeight(float newHeight)
    {
        spawnHeight = newHeight;
    }

    private void UpdateKeyColor()
    {
        if (MidiMaster.GetKey(note) > 0.0f)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            SetDefaultKeyMaterial();
        }
    }

    private void SetDefaultNoteMaterial(GameObject obj, int note)
    {
        Material mat;

        if (PlayerPrefs.GetInt("multicolorNotes") == 1)
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
                    mat = materials.Where(m => m.name == "White").FirstOrDefault();
                    break;
            }

            obj.GetComponent<Renderer>().material = mat;
        }

        else
        {
            switch (PlayerPrefs.GetInt("noteColor"))
            {
                case 1:
                    mat = materials.Where(m => m.name == "Red").FirstOrDefault();
                    break;
                case 2:
                    mat = materials.Where(m => m.name == "Red Orange").FirstOrDefault();
                    break;
                case 3:
                    mat = materials.Where(m => m.name == "Orange").FirstOrDefault();
                    break;
                case 4:
                    mat = materials.Where(m => m.name == "Gold").FirstOrDefault();
                    break;
                case 5:
                    mat = materials.Where(m => m.name == "Yellow").FirstOrDefault();
                    break;
                case 6:
                    mat = materials.Where(m => m.name == "Light Green").FirstOrDefault();
                    break;
                case 7:
                    mat = materials.Where(m => m.name == "Green").FirstOrDefault();
                    break;
                case 8:
                    mat = materials.Where(m => m.name == "Light Blue").FirstOrDefault();
                    break;
                case 9:
                    mat = materials.Where(m => m.name == "Blue").FirstOrDefault();
                    break;
                case 10:
                    mat = materials.Where(m => m.name == "Indigo").FirstOrDefault();
                    break;
                case 11:
                    mat = materials.Where(m => m.name == "Purple").FirstOrDefault();
                    break;
                case 12:
                    mat = materials.Where(m => m.name == "Pink").FirstOrDefault();
                    break;
                default:
                    mat = materials.Where(m => m.name == "White").FirstOrDefault();
                    break;
            }

            obj.GetComponent<Renderer>().material = mat;
        }
    }

    private void SetDefaultKeyMaterial()
    {
        if (PlayerPrefs.GetInt("multicolorKeys") == 1)
        {
            Material mat;
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
                    mat = materials.Where(m => m.name == "White").FirstOrDefault();
                    break;
            }

            GetComponent<Renderer>().material = mat;
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
        }
    }
}
