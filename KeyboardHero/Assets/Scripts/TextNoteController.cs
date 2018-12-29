using UnityEngine;

public class TextNoteController : MonoBehaviour {

    [SerializeField]
    float noteSize;

    [SerializeField]
    float noteSpeed;

    [SerializeField]
    float noteSpawnY;

    [SerializeField]
    float noteSpawnZ;

    [SerializeField]
    KeyCode keybind;

    private StatsController statsController;

    private float playTimer;

    void Start ()
    {
        statsController = GameObject.FindWithTag("Stats").GetComponent<StatsController>();
    }

    void Update ()
    {
        HandleMisinputs();
    }

    void OnTriggerStay (Collider other)
    {
        if (other.gameObject.tag == "Note" && Input.GetKey(keybind))
        {
            Destroy(other.gameObject);
            statsController.NoteHitUpdate();
        }
    }

    void OnTriggerExit (Collider other)
    {
        if (other.gameObject.tag == "Note")
        {
            Destroy(other.gameObject);
            statsController.NoteMissUpdate();
        }
    }

    public string GetKeybind()
    {
        return keybind.ToString();
    }

    private void HandleMisinputs()
    {
        // TODO if this key is hit and no note is in trigger volume, send miss update
    }

    public void SpawnNote()
    {
        var noteBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        noteBlock.tag = "Note";
        noteBlock.transform.localScale = new Vector3(3 * noteSize, noteSize, noteSize);
        noteBlock.transform.position = new Vector3(transform.position.x, noteSpawnY, noteSpawnZ);
        noteBlock.GetComponent<Renderer>().material = GetComponent<Renderer>().material;

        var rigidBody = noteBlock.AddComponent<Rigidbody>();

        rigidBody.useGravity = false;
        rigidBody.velocity = new Vector3(0, -noteSpeed, 0);
    }
}
