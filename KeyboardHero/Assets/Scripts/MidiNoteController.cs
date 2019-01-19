using UnityEngine;
using MidiJack;
using AudioHelm;

public class MidiNoteController : MonoBehaviour {

    [Range(21, 108)]
    public int note = 60;

    [Range(0, 3)]
    public float spawnHeight = 2.0f;

    [Range(0, 1)]
    public float beatUnitConversion;

    public bool isWhiteKey = true;

	// Update is called once per frame
	void Update ()
    {
        UpdateKeyColor();
    }

    void OnTriggerExit(Collider other)
    {
        if (int.Parse(other.tag) == note)
        {
            Destroy(other.gameObject);
        }
    }

    private void UpdateKeyColor()
    {
        if (MidiMaster.GetKey(note) > 0.0f)
        {
            GetComponent<Renderer>().material.color = Color.grey;
        }
        else if (isWhiteKey)
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.black;
        }
    }

    public void SpawnNote(Note n)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var noteDuration = (n.end - n.start) * beatUnitConversion;
        var noteSpeed = -PlayerPrefs.GetFloat("noteSpeed", 3.25f);
        var noteScale = gameObject.transform.localScale;

        cube.transform.localScale = new Vector3(noteScale.x, noteDuration, noteScale.z);
        cube.transform.position = gameObject.transform.position + new Vector3(0, spawnHeight + cube.transform.localScale.y / 2.0f, -0.05f);
        cube.tag = note.ToString();

        SetNoteColor(cube, n.note);

        var cubeRigidBody = cube.AddComponent<Rigidbody>();
        cubeRigidBody.useGravity = false;
        cubeRigidBody.constraints = 
            RigidbodyConstraints.FreezeRotation
            | RigidbodyConstraints.FreezePositionZ
            | RigidbodyConstraints.FreezePositionX;

        cubeRigidBody.AddForce(new Vector3(0, noteSpeed, 0), ForceMode.VelocityChange);
    }

    private void SetNoteColor(GameObject cube, int note)
    {
        if (PlayerPrefs.GetInt("multicolorNotes") == 1)
        {
            var c = new Color();
            switch (note % 12)
            {
                case 0: // C
                    c = Color.red;
                    break;
                case 1: // C# / Db
                    c = new Color(255, 29, 0);
                    break;
                case 2: // D
                    c = new Color(255, 76, 0);
                    break;
                case 3: // D# / Eb
                    c = new Color(255, 131, 0);
                    break;
                case 4: // E
                    c = Color.yellow;
                    break;
                case 5: // F
                    c = Color.green;
                    break;
                case 6: // F# / Gb
                    c = Color.cyan;
                    break;
                case 7: // G
                    c = new Color(0, 182, 255);
                    break;
                case 8: // G# / Ab
                    c = new Color(0, 127, 255);
                    break;
                case 9: // A
                    c = Color.blue;
                    break;
                case 10: // A# / Bb
                    c = new Color(114, 0, 255);
                    break;
                case 11: // B
                    c = Color.magenta;
                    break;
                default:
                    break;
            }

            cube.gameObject.GetComponent<Renderer>().material.color = c;
        }

        else
        {
            Color c = new Color();
            switch (PlayerPrefs.GetInt("noteColor"))
            {
                default:
                case 1:
                    c = Color.red;
                    break;
                case 2:
                    c = Color.yellow;
                    break;
                case 3:
                    c = Color.green;
                    break;
                case 4:
                    c = Color.blue;
                    break;
                case 5:
                    c = Color.cyan;
                    break;
                case 6:
                    c = Color.magenta;
                    break;
                case 7:
                    c = Color.grey;
                    break;
            }

            cube.gameObject.GetComponent<Renderer>().material.color = c;
        }
    }
}
