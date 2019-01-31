using System;
using TMPro;
using UnityEngine;

public class StatsController : MonoBehaviour {

    [Range(4,10)]
    public int maxMultiplier = 8;

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

        UpdateStatsText();
    }

    public void NoteMissUpdate()
    {
        streak = 0;
        multiplier = 1;
        totalNotes++;

        UpdateStatsText();
    }    

    public void NoteHitUpdate()
    {
        score += multiplier * 100;
        streak++;
        notesHit++;
        totalNotes++;
        multiplier = Mathf.Min(1 + streak / 10, maxMultiplier);
        accuracy = Math.Round(notesHit / totalNotes * 100.0, 2);

        UpdateStatsText();
    }

    private void UpdateStatsText()
    {
        scoreText.text = score.ToString();
        streakText.text = streak.ToString();
        accuracyText.text = accuracy.ToString() + "%";
    }
}
