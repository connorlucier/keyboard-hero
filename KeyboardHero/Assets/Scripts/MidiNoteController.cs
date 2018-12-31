using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

public class MidiNoteController : MonoBehaviour {

    [Range(21, 108)]
    public int noteNumber = 60;

    [SerializeField]
    bool isWhiteKey = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (MidiMaster.GetKey(noteNumber) > 0.0f)
        {
            GetComponent<Renderer>().material.color = Color.red;
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
}
