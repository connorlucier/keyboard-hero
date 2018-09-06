using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour {

    [SerializeField]
    float noteSize;

    [SerializeField]
    float noteSpawnY;

    [SerializeField]
    float noteSpawnZ;

    [SerializeField]
    float noteSpeed;

    private List<GameObject> noteBlocks = new List<GameObject>();

    // Use this for initialization
    void Start ()
    {
        SpawnNote();
    }

    private void SpawnNote()
    {
        var noteBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        noteBlock.transform.localScale = new Vector3(3 * noteSize, noteSize, noteSize);
        noteBlock.transform.position = new Vector3(transform.position.x, noteSpawnY, noteSpawnZ);
        noteBlock.AddComponent<Rigidbody>();

        var rigidBody = noteBlock.GetComponent<Rigidbody>();

        rigidBody.useGravity = false;
        rigidBody.velocity = new Vector3(0, -noteSpeed, 0);

        noteBlocks.Add(noteBlock);
    }

    // Update is called once per frame
    void Update () {
    }
}
