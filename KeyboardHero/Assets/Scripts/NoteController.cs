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

    void Update () {
        SpawnNote();
    }

    void OnTriggerStay (Collider other)
    {
        if (Input.GetKey(keybind) && other.gameObject.tag == "Note")
        {
            Destroy(other.gameObject);

            statsController.NoteHitUpdate();
        }
    }

    private void SpawnNote()
    {
        if (timeElapsed >= spawnTime)
        {
            var noteBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            noteBlock.tag = "Note";
            noteBlock.transform.localScale = new Vector3(3 * noteSize, noteSize, noteSize);
            noteBlock.transform.position = new Vector3(transform.position.x, noteSpawnY, noteSpawnZ);
            noteBlock.AddComponent<Rigidbody>();

            // TODO give note appropriate color

            var rigidBody = noteBlock.GetComponent<Rigidbody>();

            rigidBody.useGravity = false;
            rigidBody.velocity = new Vector3(0, -noteSpeed, 0);

            timeElapsed = 0;
            spawnTime = Random.Range(1, 5);
        }

        else
        {
            timeElapsed += Time.deltaTime;
        }
    }
}
