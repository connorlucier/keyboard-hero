using UnityEngine;

public class NoteRemover : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Note")
        {
            Destroy(other.gameObject);
        }
    }
}