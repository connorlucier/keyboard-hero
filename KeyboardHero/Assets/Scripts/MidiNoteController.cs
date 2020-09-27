using UnityEngine;
using MidiJack;
using AudioHelm;
using System.Linq;
using System.Collections.Generic;

public class MidiNoteController : MonoBehaviour {

    public AudioHelmClock clock;
    public MidiStatsController statsController;

    private int note;

    private GameObject notePrefab;
    private GameObject noteHeadPrefab;

    private Material defaultMaterial;
    private Material glowingMaterial;
    private Material missedMaterial;

    void Start()
    {
        note = gameObject.GetComponent<MidiKeyController>().note;
        notePrefab = Resources.Load<GameObject>("Physical/Note");
        noteHeadPrefab = Resources.Load<GameObject>("Physical/Notehead");
        SetNoteMaterials();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(note.ToString()) || other.CompareTag("Notehead"))
            HandleInput(other.gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(note.ToString()) || other.CompareTag("Notehead"))
            HandleInput(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Notehead"))
        {
            var attachedNote = other.GetComponent<NoteHeadController>().attachedNote;
            if (attachedNote.CompareTag(note.ToString()) && !attachedNote.GetComponent<NoteHitController>().noteHit)
            {
                statsController.NoteMissUpdate();
                attachedNote.GetComponent<Renderer>().material = missedMaterial;
            }
        }
        else if (other.CompareTag(note.ToString()))
            Destroy(other.transform.parent.gameObject);
    }

    public int Note() { return note; }

    public void CreateNote(Note n, float spawnHeight)
    {
        var noteObject = Instantiate(notePrefab);
        var noteCube = noteObject.transform.GetChild(0).gameObject;

        float noteDuration = 60 * (n.end - n.start) / (4 * clock.bpm);
        float noteSpeed = PlayerPrefs.GetFloat("noteSpeed", 3.25f);
        float noteScale = noteDuration * noteSpeed;

        float keyOffset = 0.5f * gameObject.transform.localScale.y;

        noteObject.transform.localScale = new Vector3(0.95f * gameObject.transform.localScale.x, 0.99f * noteScale - 0.025f, gameObject.transform.localScale.z);
        noteObject.transform.position = gameObject.transform.position + new Vector3(-0.475f * gameObject.transform.localScale.x, spawnHeight + keyOffset, 0);

        noteCube.tag = note.ToString();
        noteCube.GetComponent<Renderer>().material = defaultMaterial;

        var noteHead = Instantiate(noteHeadPrefab);
        var noteHeadCube = noteHead.transform.GetChild(0).gameObject;

        noteHead.transform.position = noteObject.transform.position;
        noteHead.transform.SetParent(noteObject.transform);
        noteHead.transform.localScale = new Vector3(1, noteHead.transform.localScale.y, noteHead.transform.localScale.z);

        noteHeadCube.GetComponent<Renderer>().material = defaultMaterial;
        noteHeadCube.GetComponent<NoteHeadController>().attachedNote = noteCube;

        noteObject.GetComponent<Rigidbody>().velocity = Vector3.down * noteSpeed;
    }

    private void HandleInput(GameObject obj)
    {
        if (MidiMaster.GetKey(note) > 0.0f)
        {
            if (obj.CompareTag("Notehead"))
            {
                var attachedNote = obj.GetComponent<NoteHeadController>().attachedNote;
                if (attachedNote.CompareTag(note.ToString()) && !attachedNote.GetComponent<NoteHitController>().noteHit)
                {
                    statsController.NoteHitUpdate();
                    attachedNote.GetComponent<NoteHitController>().noteHit = true;
                    attachedNote.GetComponent<Renderer>().material = glowingMaterial;
                }
            }
            else if (obj.CompareTag(note.ToString()) && obj.GetComponent<NoteHitController>().noteHit && !obj.GetComponent<NoteHitController>().noteReleased)
                statsController.NoteContinueUpdate();
        }
        else if (obj.CompareTag(note.ToString()) && obj.GetComponent<NoteHitController>().noteHit)
        {
            obj.GetComponent<NoteHitController>().noteReleased = true;
            obj.GetComponent<Renderer>().material = missedMaterial;
        }
    }

    private void SetNoteMaterials()
    {
        List<Material> materials = Resources.LoadAll<Material>("Materials/Note Colors/").ToList();
        Material def, glow, miss;

        if (PlayerPrefs.GetInt("multicolorNotes") == 1)
        {
            switch (note % 12)
            {
                case 0: // C
                    def = materials.FirstOrDefault(m => m.name == "Red");
                    glow = materials.FirstOrDefault(m => m.name == "Red Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Red Miss");
                    break;
                case 1: // C# / Db
                    def = materials.FirstOrDefault(m => m.name == "Red Orange");
                    glow = materials.FirstOrDefault(m => m.name == "Red Orange Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Red Orange Miss");
                    break;
                case 2: // D
                    def = materials.FirstOrDefault(m => m.name == "Orange");
                    glow = materials.FirstOrDefault(m => m.name == "Orange Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Orange Miss");
                    break;
                case 3: // D# / Eb
                    def = materials.FirstOrDefault(m => m.name == "Gold");
                    glow = materials.FirstOrDefault(m => m.name == "Gold Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Gold Miss");
                    break;
                case 4: // E
                    def = materials.FirstOrDefault(m => m.name == "Yellow");
                    glow = materials.FirstOrDefault(m => m.name == "Yellow Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Yellow Miss");
                    break;
                case 5: // F
                    def = materials.FirstOrDefault(m => m.name == "Light Green");
                    glow = materials.FirstOrDefault(m => m.name == "Light Green Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Light Green Miss");
                    break;
                case 6: // F# / Gb
                    def = materials.FirstOrDefault(m => m.name == "Green");
                    glow = materials.FirstOrDefault(m => m.name == "Green Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Green Miss");
                    break;
                case 7: // G
                    def = materials.FirstOrDefault(m => m.name == "Light Blue");
                    glow = materials.FirstOrDefault(m => m.name == "Light Blue Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Light Blue Miss");
                    break;
                case 8: // G# / Ab
                    def = materials.FirstOrDefault(m => m.name == "Blue");
                    glow = materials.FirstOrDefault(m => m.name == "Blue Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Blue Miss");
                    break;
                case 9: // A
                    def = materials.FirstOrDefault(m => m.name == "Indigo");
                    glow = materials.FirstOrDefault(m => m.name == "Indigo Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Indigo Miss");
                    break;
                case 10: // A# / Bb
                    def = materials.FirstOrDefault(m => m.name == "Purple");
                    glow = materials.FirstOrDefault(m => m.name == "Purple Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Purple Miss");
                    break;
                case 11: // B
                    def = materials.FirstOrDefault(m => m.name == "Pink");
                    glow = materials.FirstOrDefault(m => m.name == "Pink Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Pink Miss");
                    break;
                default:
                    def = materials.FirstOrDefault(m => m.name == "Grey");
                    glow = materials.FirstOrDefault(m => m.name == "White");
                    miss = materials.FirstOrDefault(m => m.name == "Black");
                    break;
            }
        }
        else
        {
            switch (PlayerPrefs.GetInt("noteColor"))
            {
                case 1:
                    def = materials.FirstOrDefault(m => m.name == "Red");
                    glow = materials.FirstOrDefault(m => m.name == "Red Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Red Miss");
                    break;
                case 2:
                    def = materials.FirstOrDefault(m => m.name == "Red Orange");
                    glow = materials.FirstOrDefault(m => m.name == "Red Orange Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Red Orange Miss");
                    break;
                case 3:
                    def = materials.FirstOrDefault(m => m.name == "Orange");
                    glow = materials.FirstOrDefault(m => m.name == "Orange Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Orange Miss");
                    break;
                case 4:
                    def = materials.FirstOrDefault(m => m.name == "Gold");
                    glow = materials.FirstOrDefault(m => m.name == "Gold Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Gold Miss");
                    break;
                case 5:
                    def = materials.FirstOrDefault(m => m.name == "Yellow");
                    glow = materials.FirstOrDefault(m => m.name == "Yellow Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Yellow Miss");
                    break;
                case 6:
                    def = materials.FirstOrDefault(m => m.name == "Light Green");
                    glow = materials.FirstOrDefault(m => m.name == "Light Green Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Light Green Miss");
                    break;
                case 7:
                    def = materials.FirstOrDefault(m => m.name == "Green");
                    glow = materials.FirstOrDefault(m => m.name == "Green Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Green Miss");
                    break;
                case 8:
                    def = materials.FirstOrDefault(m => m.name == "Light Blue");
                    glow = materials.FirstOrDefault(m => m.name == "Light Blue Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Light Blue Miss");
                    break;
                case 9:
                    def = materials.FirstOrDefault(m => m.name == "Blue");
                    glow = materials.FirstOrDefault(m => m.name == "Blue Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Blue Miss");
                    break;
                case 10:
                    def = materials.FirstOrDefault(m => m.name == "Indigo");
                    glow = materials.FirstOrDefault(m => m.name == "Indigo Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Indigo Miss");
                    break;
                case 11:
                    def = materials.FirstOrDefault(m => m.name == "Purple");
                    glow = materials.FirstOrDefault(m => m.name == "Purple Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Purple Miss");
                    break;
                case 12:
                    def = materials.FirstOrDefault(m => m.name == "Pink");
                    glow = materials.FirstOrDefault(m => m.name == "Pink Glow");
                    miss = materials.FirstOrDefault(m => m.name == "Pink Miss");
                    break;
                default:
                    def = materials.FirstOrDefault(m => m.name == "Grey");
                    glow = materials.FirstOrDefault(m => m.name == "White");
                    miss = materials.FirstOrDefault(m => m.name == "Black");
                    break;
            }
        }

        defaultMaterial = def;
        glowingMaterial = glow;
        missedMaterial = miss;
    }
}
