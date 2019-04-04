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
        if (other.tag == note.ToString() || other.tag == "Notehead")
            HandleInput(other.gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == note.ToString() || other.tag == "Notehead")
            HandleInput(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Notehead")
        {
            var attachedNote = other.GetComponent<NoteHeadController>().attachedNote;
            if (attachedNote.tag == note.ToString() && !attachedNote.GetComponent<NoteHitController>().noteHit)
            {
                statsController.NoteMissUpdate();
                attachedNote.GetComponent<Renderer>().material = missedMaterial;
            }
        }
        else if (other.tag == note.ToString())
            Destroy(other.transform.parent.gameObject);
    }

    public int Note() { return note; }

    public void CreateNote(Note n, float spawnHeight)
    {
        var noteObject = Instantiate(notePrefab);
        var noteCube = noteObject.transform.GetChild(0).gameObject;

        var noteDuration = 60 * (n.end - n.start) / (4 * clock.bpm);
        var noteSpeed = PlayerPrefs.GetFloat("noteSpeed", 3.25f);
        var noteScale = noteDuration * noteSpeed;

        var keyOffset = 0.5f * gameObject.transform.localScale.y;

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
            if (obj.tag == "Notehead")
            {
                var attachedNote = obj.GetComponent<NoteHeadController>().attachedNote;
                if (attachedNote.tag == note.ToString() && !attachedNote.GetComponent<NoteHitController>().noteHit)
                {
                    statsController.NoteHitUpdate();
                    attachedNote.GetComponent<NoteHitController>().noteHit = true;
                    attachedNote.GetComponent<Renderer>().material = glowingMaterial;
                }
            }
            else if (obj.tag == note.ToString() && obj.GetComponent<NoteHitController>().noteHit && !obj.GetComponent<NoteHitController>().noteReleased)
                statsController.NoteContinueUpdate();
        }
        else if (obj.tag == note.ToString() && obj.GetComponent<NoteHitController>().noteHit)
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
                    def = materials.Where(m => m.name == "Red").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Red Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Red Miss").FirstOrDefault();
                    break;
                case 1: // C# / Db
                    def = materials.Where(m => m.name == "Red Orange").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Red Orange Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Red Orange Miss").FirstOrDefault();
                    break;
                case 2: // D
                    def = materials.Where(m => m.name == "Orange").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Orange Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Orange Miss").FirstOrDefault();
                    break;
                case 3: // D# / Eb
                    def = materials.Where(m => m.name == "Gold").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Gold Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Gold Miss").FirstOrDefault();
                    break;
                case 4: // E
                    def = materials.Where(m => m.name == "Yellow").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Yellow Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Yellow Miss").FirstOrDefault();
                    break;
                case 5: // F
                    def = materials.Where(m => m.name == "Light Green").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Light Green Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Light Green Miss").FirstOrDefault();
                    break;
                case 6: // F# / Gb
                    def = materials.Where(m => m.name == "Green").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Green Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Green Miss").FirstOrDefault();
                    break;
                case 7: // G
                    def = materials.Where(m => m.name == "Light Blue").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Light Blue Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Light Blue Miss").FirstOrDefault();
                    break;
                case 8: // G# / Ab
                    def = materials.Where(m => m.name == "Blue").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Blue Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Blue Miss").FirstOrDefault();
                    break;
                case 9: // A
                    def = materials.Where(m => m.name == "Indigo").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Indigo Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Indigo Miss").FirstOrDefault();
                    break;
                case 10: // A# / Bb
                    def = materials.Where(m => m.name == "Purple").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Purple Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Purple Miss").FirstOrDefault();
                    break;
                case 11: // B
                    def = materials.Where(m => m.name == "Pink").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Pink Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Pink Miss").FirstOrDefault();
                    break;
                default:
                    def = materials.Where(m => m.name == "Grey").FirstOrDefault();
                    glow = materials.Where(m => m.name == "White").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Black").FirstOrDefault();
                    break;
            }
        }
        else
        {
            switch (PlayerPrefs.GetInt("noteColor"))
            {
                case 1:
                    def = materials.Where(m => m.name == "Red").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Red Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Red Miss").FirstOrDefault();
                    break;
                case 2:
                    def = materials.Where(m => m.name == "Red Orange").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Red Orange Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Red Orange Miss").FirstOrDefault();
                    break;
                case 3:
                    def = materials.Where(m => m.name == "Orange").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Orange Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Orange Miss").FirstOrDefault();
                    break;
                case 4:
                    def = materials.Where(m => m.name == "Gold").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Gold Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Gold Miss").FirstOrDefault();
                    break;
                case 5:
                    def = materials.Where(m => m.name == "Yellow").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Yellow Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Yellow Miss").FirstOrDefault();
                    break;
                case 6:
                    def = materials.Where(m => m.name == "Light Green").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Light Green Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Light Green Miss").FirstOrDefault();
                    break;
                case 7:
                    def = materials.Where(m => m.name == "Green").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Green Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Green Miss").FirstOrDefault();
                    break;
                case 8:
                    def = materials.Where(m => m.name == "Light Blue").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Light Blue Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Light Blue Miss").FirstOrDefault();
                    break;
                case 9:
                    def = materials.Where(m => m.name == "Blue").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Blue Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Blue Miss").FirstOrDefault();
                    break;
                case 10:
                    def = materials.Where(m => m.name == "Indigo").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Indigo Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Indigo Miss").FirstOrDefault();
                    break;
                case 11:
                    def = materials.Where(m => m.name == "Purple").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Purple Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Purple Miss").FirstOrDefault();
                    break;
                case 12:
                    def = materials.Where(m => m.name == "Pink").FirstOrDefault();
                    glow = materials.Where(m => m.name == "Pink Glow").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Pink Miss").FirstOrDefault();
                    break;
                default:
                    def = materials.Where(m => m.name == "Grey").FirstOrDefault();
                    glow = materials.Where(m => m.name == "White").FirstOrDefault();
                    miss = materials.Where(m => m.name == "Black").FirstOrDefault();
                    break;
            }
        }

        defaultMaterial = def;
        glowingMaterial = glow;
        missedMaterial = miss;
    }
}
