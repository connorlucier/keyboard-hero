using UnityEngine;

public class NoteController : MonoBehaviour {

    [SerializeField]
    float noteSize;

    [SerializeField]
    float noteSpawnY;

    [SerializeField]
    float noteSpawnZ;

    [SerializeField]
    float noteSpeed;

    [SerializeField]
    KeyCode keybind;

    private float timeElapsed;

    private float spawnTime;

    private StatsController statsController;

    void Start ()
    {
        timeElapsed = 0;
        spawnTime = Random.Range(1, 5);

        statsController = GameObject.FindWithTag("Stats").GetComponent<StatsController>();
    }

    void Update ()
    {
        HandleMisinputs();
        HandleNoteSpawning();
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

    private void HandleMisinputs()
    {
        // TODO broken
        //if (Input.anyKeyDown && !Input.GetKeyDown(keybind))
        //{
        //    statsController.NoteMissUpdate();
        //}
    }

    private void HandleNoteSpawning()
    {
        if (timeElapsed >= spawnTime)
        {
            SpawnNote();
        }

        else
        {
            timeElapsed += Time.deltaTime;
        }
    }

    private void SpawnNote()
    {
        var noteBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        noteBlock.tag = "Note";
        noteBlock.transform.localScale = new Vector3(3 * noteSize, noteSize, noteSize);
        noteBlock.transform.position = new Vector3(transform.position.x, noteSpawnY, noteSpawnZ);
        noteBlock.GetComponent<Renderer>().material = GetComponent<Renderer>().material;

        var rigidBody = noteBlock.AddComponent<Rigidbody>();

        rigidBody.useGravity = false;
        rigidBody.velocity = new Vector3(0, -noteSpeed, 0);

        timeElapsed = 0;
        spawnTime = Random.Range(1, 5);
    }
}
