using UnityEngine;
using MidiJack;
using AudioHelm;
using System.Linq;
using System.Collections.Generic;

public class MidiNoteController : MonoBehaviour {

    public AudioHelmClock clock;
    public MidiStatsController statsController;

    private int note;
    private bool noteHit;

    private GameObject notePrefab;
    private Material noteMaterial;

    void Start()
    {
        note = gameObject.GetComponent<MidiKeyController>().note;
        notePrefab = Resources.Load<GameObject>("Physical/Note");
        SetNoteMaterial();
    }

    void OnTriggerEnter(Collider other)
    {
        if (note == int.Parse(other.tag))
        {
            noteHit = false;
            HandleInput();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (note == int.Parse(other.tag))
            HandleInput();
    }

    void OnTriggerExit(Collider other)
    {
        if (note == int.Parse(other.tag))
        {
            if (!noteHit)
                statsController.NoteMissUpdate();

            Destroy(other.gameObject.transform.parent.gameObject);
        }
    }

    public int Note() { return note; }

    public void CreateNote(Note n, float spawnHeight)
    {
        var noteObject = Instantiate(notePrefab);
        var cube = noteObject.transform.GetChild(0).gameObject;

        var noteDuration = 60 * (n.end - n.start) / (4 * clock.bpm);
        var noteSpeed = PlayerPrefs.GetFloat("noteSpeed", 3.25f);
        var noteScale = noteDuration * noteSpeed;

        var keyOffset = 0.5f * gameObject.transform.localScale.y;

        noteObject.transform.localScale = new Vector3(0.95f * gameObject.transform.localScale.x, noteScale, gameObject.transform.localScale.z);
        noteObject.transform.position = gameObject.transform.position + new Vector3(-0.475f * gameObject.transform.localScale.x, spawnHeight + keyOffset, 0);

        cube.tag = note.ToString();
        cube.GetComponent<Renderer>().material = noteMaterial;

        var noteRigidBody = noteObject.AddComponent<Rigidbody>();
        noteRigidBody.useGravity = false;
        noteRigidBody.constraints =
            RigidbodyConstraints.FreezeRotation
            | RigidbodyConstraints.FreezePositionZ
            | RigidbodyConstraints.FreezePositionX;

        noteRigidBody.velocity = Vector3.down * noteSpeed;
    }

    private void HandleInput()
    {
        if (!noteHit && MidiMaster.GetKey(note) > 0.0f) // TODO how to balance?
        {
            statsController.NoteHitUpdate();
            noteHit = true;
        }

        else if (noteHit && MidiMaster.GetKey(note) > 0.0f)
        {
            statsController.NoteContinueUpdate();
        }
    }

    private void SetNoteMaterial()
    {
        List<Material> materials = Resources.LoadAll<Material>("Materials/Note Colors/").ToList();
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

            noteMaterial = mat;
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

            noteMaterial = mat;
        }
    }
}
