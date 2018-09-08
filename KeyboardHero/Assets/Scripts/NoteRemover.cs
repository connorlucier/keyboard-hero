using UnityEngine;

public class NoteRemover : MonoBehaviour
{
    private StatsController statsController;

    void Start()
    {
        statsController = GameObject.FindWithTag("Stats").GetComponent<StatsController>();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Note")
        {
            Destroy(other.gameObject);
            statsController.NoteMissUpdate();
        }
    }
}