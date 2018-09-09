using System;
using TMPro;
using UnityEngine;

public class StatsController : MonoBehaviour {

    private int notesHit;

    private int totalNotes;

    private int score;

    private int streak;

    private int multiplier;

    private double accuracy;

    private TextMeshPro scoreText;

    private TextMeshPro streakText;

    private TextMeshPro accuracyText;
    
    void Start ()
    {
        scoreText = GameObject.FindWithTag("Score").GetComponent<TextMeshPro>();
        streakText = GameObject.FindWithTag("Streak").GetComponent<TextMeshPro>();
        accuracyText = GameObject.FindWithTag("Accuracy").GetComponent<TextMeshPro>();

        score = 0;
        streak = 0;
        notesHit = 0;
        totalNotes = 0;
        multiplier = 1;
        accuracy = 100.0;
    }

    public void NoteMissUpdate()
    {
        streak = 0;
        multiplier = 1;

        totalNotes++;

        UpdateAccuracy();
        UpdateStatsText();
    }    

    public void NoteHitUpdate()
    {
        score += multiplier * 100;
        streak++;
        notesHit++;
        totalNotes++;
        multiplier = 1 + streak / 10;


        UpdateAccuracy();
        UpdateStatsText();
    }

    private void UpdateAccuracy()
    {
        accuracy = Math.Round((double)notesHit / totalNotes * 100.0, 2);
    }

    private void UpdateStatsText()
    {
        scoreText.text = score.ToString();
        streakText.text = streak.ToString();
        accuracyText.text = accuracy.ToString() + "%";
    }
}
